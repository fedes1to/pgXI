using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class PetsSynchronizer
	{
		private const int AttemptCountMax = 3;

		private static readonly WaitForRealSeconds s_delay = new WaitForRealSeconds(30f);

		private static readonly PetsSynchronizer s_instance = new PetsSynchronizer();

		public static PetsSynchronizer Instance
		{
			get
			{
				return s_instance;
			}
		}

		private PetsSynchronizer()
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
				SyncPetsIos();
				return null;
			}
			return null;
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
					using (AGSSyncableString aGSSyncableString = aGSGameDataMap.GetLatestString("petsJson"))
					{
						EnsureNotNull(aGSSyncableString, "petsJson");
						string json = aGSSyncableString.GetValue() ?? JsonUtility.ToJson(new PlayerPets());
						PlayerPets playerPets = PetsManager.LoadPlayerPetsMemento();
						PlayerPets playerPets2 = JsonUtility.FromJson<PlayerPets>(json);
						PlayerPets playerPets3 = PlayerPets.Merge(playerPets, playerPets2);
						if (IsDirtyComparedToMergedMemento(playerPets, playerPets3))
						{
							PetsManager.OverwritePlayerPetsMemento(playerPets3);
							PetsManager.LoadPetsToMemory();
						}
						if (IsDirtyComparedToMergedMemento(playerPets2, playerPets3) || playerPets2.Conflicted)
						{
							string val = JsonUtility.ToJson(playerPets3);
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

		private void EnsureNotNull(object value, string name)
		{
			if (value == null)
			{
				throw new InvalidOperationException(name ?? string.Empty);
			}
		}

		private IEnumerator SyncGoogleCoroutine(bool pullOnly)
		{
			string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.SyncGoogleCoroutine('{1}')", GetType().Name, (!pullOnly) ? "sync" : "pull");
			ScopeLogger scopeLogger = new ScopeLogger(thisMethod, false);
			try
			{
				PetsSynchronizerGpgFacade googleSavedGamesFacade = default(PetsSynchronizerGpgFacade);
				for (int i = 0; i != 3; i++)
				{
					Task<GoogleSavedGameRequestResult<PlayerPets>> future = googleSavedGamesFacade.Pull();
					while (!future.IsCompleted)
					{
						yield return null;
					}
					if (future.IsFaulted)
					{
						Exception ex = future.Exception.InnerExceptions.FirstOrDefault() ?? future.Exception;
						Debug.LogWarning("Failed to pull pets with exception: " + ex.Message);
						yield return s_delay;
						continue;
					}
					SavedGameRequestStatus requestStatus = future.Result.RequestStatus;
					if (requestStatus != SavedGameRequestStatus.Success)
					{
						Debug.LogWarning("Failed to pull pets with status: " + requestStatus);
						yield return s_delay;
						continue;
					}
					PlayerPets localPetsMemento = PetsManager.LoadPlayerPetsMemento();
					PlayerPets cloudPetsMemento = future.Result.Value;
					PlayerPets mergedPetsMemento = PlayerPets.Merge(localPetsMemento, cloudPetsMemento);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("Local pets: {0}", JsonUtility.ToJson(localPetsMemento));
						Debug.LogFormat("Cloud pets: {0}", JsonUtility.ToJson(cloudPetsMemento));
						Debug.LogFormat("Merged pets: {0}", JsonUtility.ToJson(mergedPetsMemento));
					}
					bool localDirty = IsDirtyComparedToMergedMemento(localPetsMemento, mergedPetsMemento);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("Local pets are dirty: {0}", localDirty);
					}
					if (localDirty)
					{
						PetsManager.OverwritePlayerPetsMemento(mergedPetsMemento);
						PetsManager.LoadPetsToMemory();
					}
					if (pullOnly)
					{
						break;
					}
					bool cloudDirty = IsDirtyComparedToMergedMemento(cloudPetsMemento, mergedPetsMemento);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("Cloud pets are dirty: {0}, conflicted: {1}", cloudDirty, cloudPetsMemento.Conflicted);
					}
					if (cloudDirty || cloudPetsMemento.Conflicted)
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

		private IEnumerator PushGoogleCoroutine()
		{
			string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.PushGoogleCoroutine()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(thisMethod, false);
			try
			{
				PetsSynchronizerGpgFacade googleSavedGamesFacade = default(PetsSynchronizerGpgFacade);
				for (int i = 0; i != 3; i++)
				{
					PlayerPets localPetsMemento = PetsManager.LoadPlayerPetsMemento();
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("Local pets: {0}", JsonUtility.ToJson(localPetsMemento));
					}
					Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> future = googleSavedGamesFacade.Push(localPetsMemento);
					while (!future.IsCompleted)
					{
						yield return null;
					}
					if (future.IsFaulted)
					{
						Exception ex = future.Exception.InnerExceptions.FirstOrDefault() ?? future.Exception;
						Debug.LogWarning("Failed to push pets with exception: " + ex.Message);
						yield return s_delay;
						continue;
					}
					SavedGameRequestStatus requestStatus = future.Result.RequestStatus;
					if (requestStatus != SavedGameRequestStatus.Success)
					{
						Debug.LogWarning("Failed to push pets with status: " + requestStatus);
						yield return s_delay;
						continue;
					}
					if (Defs.IsDeveloperBuild)
					{
						ISavedGameMetadata metadata = future.Result.Value;
						string description = ((metadata == null) ? string.Empty : metadata.Description);
						Debug.LogFormat("[Pets] Succeeded to push pets {0}: '{1}'", localPetsMemento, description);
					}
					break;
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void SyncPetsIos()
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer && !Storager.ICloudAvailable)
			{
			}
		}

		private static bool IsDirtyComparedToMergedMemento(PlayerPets otherPetsMemento, PlayerPets mergedMemento)
		{
			//Discarded unreachable code: IL_0165
			try
			{
				if (mergedMemento.Pets.Select((PlayerPet pet) => pet.InfoId).Except(otherPetsMemento.Pets.Select((PlayerPet pet) => pet.InfoId)).Any())
				{
					return true;
				}
				IOrderedEnumerable<PlayerPet> source = mergedMemento.Pets.OrderBy((PlayerPet pet) => pet.InfoId);
				IOrderedEnumerable<PlayerPet> source2 = otherPetsMemento.Pets.OrderBy((PlayerPet pet) => pet.InfoId);
				return !source.Select((PlayerPet pet) => pet.PetName).SequenceEqual(source2.Select((PlayerPet pet) => pet.PetName)) || !source.Select((PlayerPet pet) => pet.NameTimestamp).SequenceEqual(source2.Select((PlayerPet pet) => pet.NameTimestamp));
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in SyncPetsIos dirtyCondition: {0}", ex);
			}
			return false;
		}
	}
}
