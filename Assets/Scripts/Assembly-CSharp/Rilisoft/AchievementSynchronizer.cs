using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi.SavedGame;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class AchievementSynchronizer
	{
		private const int AttemptCountMax = 3;

		private static readonly WaitForRealSeconds s_delay = new WaitForRealSeconds(30f);

		private static readonly AchievementSynchronizer s_instance = new AchievementSynchronizer();

		public static AchievementSynchronizer Instance
		{
			get
			{
				return s_instance;
			}
		}

		private AchievementSynchronizer()
		{
		}

		public Coroutine Sync()
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return CoroutineRunner.Instance.StartCoroutine(SyncGoogleCoroutine(false));
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					SyncAmazon();
				}
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				SyncIos();
			}
			return null;
		}

		private IEnumerator PushGoogleCoroutine()
		{
			string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.PushGoogleCoroutine()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(thisMethod, false);
			try
			{
				if (!Application.isEditor && GpgFacade.Instance.SavedGame == null)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("{0}: Waiting while `SavedGame == null`...", GetType().Name);
					}
					while (GpgFacade.Instance.SavedGame == null)
					{
						yield return null;
					}
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("{0}: `SavedGame != null`...", GetType().Name);
					}
				}
				AchievementSynchronizerGpgFacade googleSavedGamesFacade = default(AchievementSynchronizerGpgFacade);
				for (int i = 0; i != 3; i++)
				{
					AchievementProgressSyncObject localData = ReadLocalData();
					Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> future = googleSavedGamesFacade.Push(localData);
					while (!future.IsCompleted)
					{
						yield return null;
					}
					if (future.IsFaulted)
					{
						Exception ex = future.Exception.InnerExceptions.FirstOrDefault() ?? future.Exception;
						Debug.LogWarningFormat("[{0}] Failed to push with exception: '{1}'", GetType().Name, ex.Message);
						yield return s_delay;
						continue;
					}
					SavedGameRequestStatus requestStatus = future.Result.RequestStatus;
					if (requestStatus != SavedGameRequestStatus.Success)
					{
						Debug.LogWarningFormat("[{0}] Failed to push with status: '{1}'", GetType().Name, requestStatus);
						yield return s_delay;
						continue;
					}
					if (Defs.IsDeveloperBuild)
					{
						ISavedGameMetadata metadata = future.Result.Value;
						string description = ((metadata == null) ? string.Empty : metadata.Description);
						Debug.LogFormat("[{0}] Succeeded to push: '{1}'", GetType().Name, description);
					}
					break;
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private IEnumerator SyncGoogleCoroutine(bool pullOnly)
		{
			string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.SyncGoogleCoroutine('{1}')", GetType().Name, (!pullOnly) ? "sync" : "pull");
			ScopeLogger scopeLogger = new ScopeLogger(thisMethod, false);
			try
			{
				if (!Application.isEditor && GpgFacade.Instance.SavedGame == null)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("{0}: Waiting while `SavedGame == null`...", GetType().Name);
					}
					while (GpgFacade.Instance.SavedGame == null)
					{
						yield return null;
					}
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("{0}: `SavedGame != null`...", GetType().Name);
					}
				}
				AchievementSynchronizerGpgFacade googleSavedGamesFacade = default(AchievementSynchronizerGpgFacade);
				for (int i = 0; i != 3; i++)
				{
					Task<GoogleSavedGameRequestResult<AchievementProgressSyncObject>> future = googleSavedGamesFacade.Pull();
					while (!future.IsCompleted)
					{
						yield return null;
					}
					if (future.IsFaulted)
					{
						Exception ex = future.Exception.InnerExceptions.FirstOrDefault() ?? future.Exception;
						Debug.LogWarningFormat("[{0}] Failed to pull with exception: '{1}'", GetType().Name, ex.Message);
						yield return s_delay;
						continue;
					}
					SavedGameRequestStatus requestStatus = future.Result.RequestStatus;
					if (requestStatus != SavedGameRequestStatus.Success)
					{
						Debug.LogWarningFormat("[{0}] Failed to pull with status: '{1}'", GetType().Name, requestStatus);
						yield return s_delay;
						continue;
					}
					AchievementProgressSyncObject localData = ReadLocalData();
					AchievementProgressSyncObject cloudData = future.Result.Value;
					AchievementProgressSyncObject mergedData = AchievementProgressData.Merge(localData, cloudData);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("Local progress: {0}", Json.Serialize(localData.ProgressData.Select((AchievementProgressData p) => p.AchievementId).ToList()));
						Debug.LogFormat("Cloud progress: {0}", Json.Serialize(cloudData.ProgressData.Select((AchievementProgressData p) => p.AchievementId).ToList()));
						Debug.LogFormat("Merged progress: {0}", Json.Serialize(mergedData.ProgressData.Select((AchievementProgressData p) => p.AchievementId).ToList()));
					}
					if (!localData.Equals(mergedData))
					{
						SaveLocalData(mergedData);
					}
					if (!cloudData.Equals(mergedData) || cloudData.Conflicted)
					{
						IEnumerator enumerator = PushGoogleCoroutine();
						while (enumerator.MoveNext())
						{
							yield return null;
						}
					}
					break;
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void SyncAmazon()
		{
			string callee = string.Format(CultureInfo.InvariantCulture, "{0}.SyncAmazon()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, false);
			try
			{
				if (Application.isEditor)
				{
					return;
				}
				AGSWhispersyncClient.Synchronize();
				using (AGSGameDataMap aGSGameDataMap = AGSWhispersyncClient.GetGameData())
				{
					EnsureNotNull(aGSGameDataMap, "dataMap");
					using (AGSSyncableString aGSSyncableString = aGSGameDataMap.GetLatestString("achievementsJson"))
					{
						EnsureNotNull(aGSSyncableString, "achievementsJson");
						string json = aGSSyncableString.GetValue() ?? "{}";
						AchievementProgressSyncObject achievementProgressSyncObject = ReadLocalData();
						AchievementProgressSyncObject achievementProgressSyncObject2 = AchievementProgressSyncObject.FromJson(json);
						AchievementProgressSyncObject achievementProgressSyncObject3 = AchievementProgressData.Merge(achievementProgressSyncObject, achievementProgressSyncObject2);
						if (!achievementProgressSyncObject.Equals(achievementProgressSyncObject3))
						{
							SaveLocalData(achievementProgressSyncObject3);
						}
						if (!achievementProgressSyncObject2.Equals(achievementProgressSyncObject3) || achievementProgressSyncObject2.Conflicted)
						{
							string val = AchievementProgressSyncObject.ToJson(achievementProgressSyncObject3);
							aGSSyncableString.Set(val);
							AGSWhispersyncClient.Synchronize();
						}
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void SyncIos()
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer && !Storager.ICloudAvailable)
			{
			}
		}

		private void EnsureNotNull(object value, string name)
		{
			if (value == null)
			{
				throw new InvalidOperationException(name ?? string.Empty);
			}
		}

		private AchievementProgressSyncObject ReadLocalData()
		{
			List<AchievementProgressData> progressData = AchievementsManager.ReadLocalProgress();
			return new AchievementProgressSyncObject(progressData);
		}

		internal void SaveLocalData(AchievementProgressSyncObject achievementMemento)
		{
			if (!(Singleton<AchievementsManager>.Instance != null))
			{
				return;
			}
			foreach (AchievementProgressData progressDatum in achievementMemento.ProgressData)
			{
				Singleton<AchievementsManager>.Instance.SetProgress(progressDatum);
			}
			Singleton<AchievementsManager>.Instance.SaveProgresses();
		}
	}
}
