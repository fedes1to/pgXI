using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class ProgressSynchronizer
	{
		public const string Filename = "Progress";

		private static ProgressSynchronizer _instance;

		public static ProgressSynchronizer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ProgressSynchronizer();
				}
				return _instance;
			}
		}

		public void SynchronizeIosProgress()
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer && !Storager.ICloudAvailable)
			{
			}
		}

		public void SynchronizeAmazonProgress()
		{
			if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android || Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
			{
				Debug.LogWarning("SynchronizeAmazonProgress() is not implemented for current target.");
				return;
			}
			AGSWhispersyncClient.Synchronize();
			using (AGSGameDataMap aGSGameDataMap = AGSWhispersyncClient.GetGameData())
			{
				if (aGSGameDataMap == null)
				{
					Debug.LogWarning("dataMap == null");
					return;
				}
				using (AGSGameDataMap aGSGameDataMap2 = aGSGameDataMap.GetMap("progressMap"))
				{
					if (aGSGameDataMap2 == null)
					{
						Debug.LogWarning("syncableProgressMap == null");
						return;
					}
					string[] array = (from k in aGSGameDataMap2.GetMapKeys()
						where !string.IsNullOrEmpty(k)
						select k).ToArray();
					string message = string.Format("Trying to sync progress.    Local: {0}    Cloud keys: {1}", CampaignProgress.GetCampaignProgressString(), Json.Serialize(array));
					Debug.Log(message);
					string[] array2 = array;
					foreach (string text in array2)
					{
						Dictionary<string, int> value;
						if (!CampaignProgress.boxesLevelsAndStars.TryGetValue(text, out value))
						{
							Debug.LogWarning("boxesLevelsAndStars doesn't contain “" + text + "”");
							value = new Dictionary<string, int>();
							CampaignProgress.boxesLevelsAndStars.Add(text, value);
						}
						else if (value == null)
						{
							Debug.LogWarning("localBox == null");
							value = new Dictionary<string, int>();
							CampaignProgress.boxesLevelsAndStars[text] = value;
						}
						using (AGSGameDataMap aGSGameDataMap3 = aGSGameDataMap2.GetMap(text))
						{
							if (aGSGameDataMap3 == null)
							{
								Debug.LogWarning("boxMap == null");
								continue;
							}
							string[] array3 = aGSGameDataMap3.GetHighestNumberKeys().ToArray();
							string message2 = string.Format("“{0}” levels: {1}", text, Json.Serialize(array3));
							Debug.Log(message2);
							string[] array4 = array3;
							foreach (string text2 in array4)
							{
								using (AGSSyncableNumber aGSSyncableNumber = aGSGameDataMap3.GetHighestNumber(text2))
								{
									if (aGSSyncableNumber == null)
									{
										Debug.LogWarning("syncableCloudValue == null");
										continue;
									}
									if (Debug.isDebugBuild)
									{
										Debug.Log("Synchronizing from cloud “" + text2 + "”...");
									}
									int num = aGSSyncableNumber.AsInt();
									int value2 = 0;
									if (value.TryGetValue(text2, out value2))
									{
										value[text2] = Math.Max(value2, num);
									}
									else
									{
										value.Add(text2, num);
									}
									if (Debug.isDebugBuild)
									{
										Debug.Log("Synchronized from cloud “" + text2 + "”...");
									}
								}
							}
						}
					}
					CampaignProgress.OpenNewBoxIfPossible();
					CampaignProgress.ActualizeComicsViews();
					WeaponManager.ActualizeWeaponsForCampaignProgress();
					Debug.Log("Trying to sync progress.    Merged: " + CampaignProgress.GetCampaignProgressString());
					foreach (KeyValuePair<string, Dictionary<string, int>> boxesLevelsAndStar in CampaignProgress.boxesLevelsAndStars)
					{
						if (Debug.isDebugBuild)
						{
							string message3 = string.Format("Synchronizing to cloud: “{0}”", boxesLevelsAndStar);
							Debug.Log(message3);
						}
						using (AGSGameDataMap aGSGameDataMap4 = aGSGameDataMap2.GetMap(boxesLevelsAndStar.Key))
						{
							if (aGSGameDataMap4 == null)
							{
								Debug.LogWarning("boxMap == null");
								continue;
							}
							Dictionary<string, int> dictionary = boxesLevelsAndStar.Value ?? new Dictionary<string, int>();
							foreach (KeyValuePair<string, int> item in dictionary)
							{
								using (AGSSyncableNumber aGSSyncableNumber2 = aGSGameDataMap4.GetHighestNumber(item.Key))
								{
									if (aGSSyncableNumber2 == null)
									{
										Debug.LogWarning("syncableCloudValue == null");
									}
									else
									{
										aGSSyncableNumber2.Set(item.Value);
									}
								}
							}
						}
					}
					AGSWhispersyncClient.Synchronize();
				}
			}
		}

		public void AuthenticateAndSynchronize(Action callback, bool silent)
		{
			if (GpgFacade.Instance.IsAuthenticated())
			{
				Debug.LogFormat("Already authenticated: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
				SynchronizeIfAuthenticated(callback);
				return;
			}
			Action<bool> callback2 = delegate(bool succeeded)
			{
				bool value = !silent && !succeeded;
				PlayerPrefs.SetInt("GoogleSignInDenied", Convert.ToInt32(value));
				if (succeeded)
				{
					string message = string.Format("Authentication succeeded: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
					Debug.Log(message);
					SynchronizeIfAuthenticated(callback);
				}
				else if (!Application.isEditor)
				{
					Debug.LogWarning("Authentication failed.");
				}
			};
			GpgFacade.Instance.Authenticate(callback2, silent);
		}

		private void SynchronizeIfAuthenticatedWithSavedGamesService(Action callback)
		{
			if (GpgFacade.Instance.SavedGame == null)
			{
				Debug.LogWarning("Saved game client is null.");
				return;
			}
			List<string> list = new List<string>();
			Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback = delegate(SavedGameRequestStatus openStatus, ISavedGameMetadata openMetadata)
			{
				Debug.LogFormat("****** Open '{0}': {1} '{2}'", "Progress", openStatus, openMetadata.GetDescription());
				if (openStatus == SavedGameRequestStatus.Success)
				{
					Debug.LogFormat("****** Trying to read '{0}' '{1}'...", "Progress", openMetadata.GetDescription());
					GpgFacade.Instance.SavedGame.ReadBinaryData(openMetadata, delegate(SavedGameRequestStatus readStatus, byte[] data)
					{
						string string3 = Encoding.UTF8.GetString(data ?? new byte[0]);
						Debug.Log(string.Format("****** Read '{0}': {1} '{2}'    '{3}'", "Progress", readStatus, openMetadata.GetDescription(), string3));
						if (readStatus == SavedGameRequestStatus.Success)
						{
							Dictionary<string, Dictionary<string, int>> dictionary2 = CampaignProgress.DeserializeProgress(string3);
							if (dictionary2 == null)
							{
								Debug.LogWarning("serverProgress == null");
							}
							else
							{
								HashSet<string> hashSet = new HashSet<string>(CampaignProgress.boxesLevelsAndStars.SelectMany((KeyValuePair<string, Dictionary<string, int>> kv) => kv.Value.Keys));
								hashSet.ExceptWith(dictionary2.SelectMany((KeyValuePair<string, Dictionary<string, int>> kv) => kv.Value.Keys));
								string text = Json.Serialize(hashSet.ToArray());
								MergeUpdateLocalProgress(dictionary2);
								CampaignProgress.ActualizeComicsViews();
								WeaponManager.ActualizeWeaponsForCampaignProgress();
								string outgoingProgressString = CampaignProgress.GetCampaignProgressString();
								Debug.Log(string.Format("****** Trying to write '{0}': '{1}'...", "Progress", outgoingProgressString));
								byte[] bytes2 = Encoding.UTF8.GetBytes(outgoingProgressString);
								string description2 = string.Format("Added levels by '{0}': {1}", SystemInfo.deviceModel, text.Substring(0, Math.Min(32, text.Length)));
								SavedGameMetadataUpdate updateForMetadata2 = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(description2).Build();
								GpgFacade.Instance.SavedGame.CommitUpdate(openMetadata, updateForMetadata2, bytes2, delegate(SavedGameRequestStatus writeStatus, ISavedGameMetadata closeMetadata)
								{
									Debug.Log(string.Format("****** Written '{0}': {1} '{2}'    '{3}'", "Progress", writeStatus, closeMetadata.GetDescription(), outgoingProgressString));
									callback();
								});
							}
						}
					});
				}
			};
			ConflictCallback conflictCallback = delegate(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
			{
				string @string = Encoding.UTF8.GetString(originalData);
				string string2 = Encoding.UTF8.GetString(unmergedData);
				ISavedGameMetadata savedGameMetadata = null;
				if (@string.Length > string2.Length)
				{
					savedGameMetadata = original;
					resolver.ChooseMetadata(savedGameMetadata);
					Debug.Log(string.Format("****** Partially resolved using original metadata '{0}': '{1}'", "Progress", original.GetDescription()));
				}
				else
				{
					savedGameMetadata = unmerged;
					resolver.ChooseMetadata(savedGameMetadata);
					Debug.Log(string.Format("****** Partially resolved using unmerged metadata '{0}': '{1}'", "Progress", unmerged.GetDescription()));
				}
				string mergedString = DictionaryLoadedListener.MergeProgress(@string, string2);
				Dictionary<string, Dictionary<string, int>> dictionary = CampaignProgress.DeserializeProgress(mergedString);
				if (dictionary == null)
				{
					Debug.LogWarning("mergedProgress == null");
				}
				else
				{
					MergeUpdateLocalProgress(dictionary);
					CampaignProgress.ActualizeComicsViews();
					WeaponManager.ActualizeWeaponsForCampaignProgress();
					string description = string.Format("Merged by '{0}': '{1}' and '{2}'", SystemInfo.deviceModel, original.GetDescription(), unmerged.GetDescription());
					SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(description).Build();
					byte[] bytes = Encoding.UTF8.GetBytes(mergedString);
					GpgFacade.Instance.SavedGame.CommitUpdate(savedGameMetadata, updateForMetadata, bytes, delegate(SavedGameRequestStatus writeStatus, ISavedGameMetadata closeMetadata)
					{
						Debug.LogFormat("****** Written '{0}': {1} '{2}'    '{3}'", "Progress", writeStatus, closeMetadata.GetDescription(), mergedString);
						callback();
					});
				}
			};
			Debug.LogFormat("****** Trying to open '{0}'...", "Progress");
			GpgFacade.Instance.SavedGame.OpenWithManualConflictResolution("Progress", DataSource.ReadNetworkOnly, true, conflictCallback, completedCallback);
		}

		private static void MergeUpdateLocalProgress(IDictionary<string, Dictionary<string, int>> incomingProgress)
		{
			foreach (KeyValuePair<string, Dictionary<string, int>> item in incomingProgress)
			{
				Dictionary<string, int> value;
				if (CampaignProgress.boxesLevelsAndStars.TryGetValue(item.Key, out value))
				{
					foreach (KeyValuePair<string, int> item2 in item.Value)
					{
						int value2;
						if (value.TryGetValue(item2.Key, out value2))
						{
							value[item2.Key] = Math.Max(value2, item2.Value);
						}
						else
						{
							value.Add(item2.Key, item2.Value);
						}
					}
				}
				else
				{
					CampaignProgress.boxesLevelsAndStars.Add(item.Key, item.Value);
				}
			}
			CampaignProgress.OpenNewBoxIfPossible();
		}

		public void SynchronizeIfAuthenticated(Action callback)
		{
			if (!GpgFacade.Instance.IsAuthenticated())
			{
				return;
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			using (new StopwatchLogger("SynchronizeIfAuthenticated(...)"))
			{
				SynchronizeIfAuthenticatedWithSavedGamesService(callback);
			}
		}
	}
}
