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
	internal sealed class SkinsSynchronizer
	{
		private static readonly SkinsSynchronizer s_instance = new SkinsSynchronizer();

		public static SkinsSynchronizer Instance
		{
			get
			{
				return s_instance;
			}
		}

		internal bool Ready
		{
			get
			{
				return true;
			}
		}

		public event EventHandler Updated;

		private SkinsSynchronizer()
		{
		}

		public Coroutine Push()
		{
			if (!Ready)
			{
				return null;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return CoroutineRunner.Instance.StartCoroutine(PushGoogleCoroutine());
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					SyncAmazon();
				}
			}
			return null;
		}

		public Coroutine Sync()
		{
			if (!Ready)
			{
				return null;
			}
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
				SyncSkinsAndCapeIos();
				return null;
			}
			return null;
		}

		private void SyncAmazon()
		{
			string callee = string.Format(CultureInfo.InvariantCulture, "{0}.SyncAmazon()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
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
					using (AGSSyncableString aGSSyncableString = aGSGameDataMap.GetLatestString("skinsJson"))
					{
						EnsureNotNull(aGSSyncableString, "skinsJson");
						string json = aGSSyncableString.GetValue() ?? "{}";
						SkinsMemento skinsMemento = JsonUtility.FromJson<SkinsMemento>(json);
						SkinsMemento skinsMemento2 = LoadLocalSkins();
						SkinsMemento skinsMemento3 = SkinsMemento.Merge(skinsMemento2, skinsMemento);
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("Local skins: {0}", skinsMemento2);
							Debug.LogFormat("Cloud skins: {0}", skinsMemento);
							Debug.LogFormat("Merged skins: {0}", skinsMemento3);
						}
						int num = skinsMemento3.DeletedSkins.Distinct().Count();
						int num2 = skinsMemento3.Skins.Select((SkinMemento s) => s.Id).Distinct().Count();
						long id = skinsMemento3.Cape.Id;
						int num3 = skinsMemento2.DeletedSkins.Distinct().Count();
						int num4 = skinsMemento2.Skins.Select((SkinMemento s) => s.Id).Distinct().Count();
						long id2 = skinsMemento2.Cape.Id;
						if (num3 < num || num4 < num2 || id2 < id)
						{
							if (num4 < num2)
							{
								Storager.setInt(Defs.SkinsMakerInProfileBought, 1, true);
							}
							OverwriteLocalSkins(skinsMemento2, skinsMemento3);
							EventHandler updated = this.Updated;
							if (updated != null)
							{
								updated(this, EventArgs.Empty);
							}
						}
						int num5 = skinsMemento.DeletedSkins.Distinct().Count();
						int num6 = skinsMemento.Skins.Select((SkinMemento s) => s.Id).Distinct().Count();
						long id3 = skinsMemento.Cape.Id;
						if (num5 < num || num6 < num2 || id3 < id)
						{
							string val = JsonUtility.ToJson(skinsMemento3);
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
			if (!Ready)
			{
				yield break;
			}
			string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.PullGoogleCoroutine('{1}')", GetType().Name, (!pullOnly) ? "sync" : "pull");
			ScopeLogger scopeLogger = new ScopeLogger(thisMethod, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				SkinsSynchronizerGoogleSavedGameFacade googleSavedGamesFacade = default(SkinsSynchronizerGoogleSavedGameFacade);
				WaitForSeconds delay = new WaitForSeconds(30f);
				int i = 0;
				while (true)
				{
					string callee = string.Format(CultureInfo.InvariantCulture, "Pull and wait ({0})", i);
					using (ScopeLogger logger = new ScopeLogger(thisMethod, callee, Defs.IsDeveloperBuild && !Application.isEditor))
					{
						Task<GoogleSavedGameRequestResult<SkinsMemento>> future = googleSavedGamesFacade.Pull();
						while (!future.IsCompleted)
						{
							yield return null;
						}
						logger.Dispose();
						if (future.IsFaulted)
						{
							Exception ex = future.Exception.InnerExceptions.FirstOrDefault() ?? future.Exception;
							Debug.LogWarning("Failed to pull skins with exception: " + ex.Message);
							yield return delay;
						}
						else
						{
							SavedGameRequestStatus requestStatus = future.Result.RequestStatus;
							if (requestStatus == SavedGameRequestStatus.Success)
							{
								SkinsMemento cloudSkins = future.Result.Value;
								SkinsMemento localSkins = LoadLocalSkins();
								HashSet<string> cloudDeletedSkins = new HashSet<string>(cloudSkins.DeletedSkins);
								HashSet<string> localDeletedSkins = new HashSet<string>(localSkins.DeletedSkins);
								HashSet<string> mergedDeletedSkins = new HashSet<string>(cloudDeletedSkins);
								mergedDeletedSkins.UnionWith(localDeletedSkins);
								HashSet<string> cloudSkinIds = new HashSet<string>(cloudSkins.Skins.Select((SkinMemento s) => s.Id));
								HashSet<string> localSkinIds = new HashSet<string>(localSkins.Skins.Select((SkinMemento s) => s.Id));
								HashSet<string> mergedSkinIds = new HashSet<string>(cloudSkinIds);
								mergedSkinIds.UnionWith(localSkinIds);
								CapeMemento chosenCape = CapeMemento.ChooseCape(localSkins.Cape, cloudSkins.Cape);
								if (localDeletedSkins.Count < mergedDeletedSkins.Count || localSkinIds.Count < mergedSkinIds.Count || localSkins.Cape.Id < chosenCape.Id)
								{
									if (localSkinIds.Count < mergedSkinIds.Count)
									{
										Storager.setInt(Defs.SkinsMakerInProfileBought, 1, true);
									}
									OverwriteLocalSkins(localSkins, cloudSkins);
									EventHandler handler = this.Updated;
									if (handler != null)
									{
										handler(this, EventArgs.Empty);
									}
								}
								bool cloudDirty = cloudDeletedSkins.Count < mergedDeletedSkins.Count || cloudSkinIds.Count < mergedSkinIds.Count || cloudSkins.Cape.Id < chosenCape.Id;
								if (Defs.IsDeveloperBuild)
								{
									Debug.LogFormat("[Skins] Succeeded to pull skins: {0}, 'pullOnly':{1}, 'conflicted':{2}, 'cloudDirty':{3}", cloudSkins, pullOnly, cloudSkins.Conflicted, cloudDirty);
								}
								if (pullOnly || (!cloudSkins.Conflicted && !cloudDirty))
								{
									break;
								}
								ScopeLogger scopeLogger2 = new ScopeLogger(thisMethod, "PushGoogleCoroutine(conflict)", Defs.IsDeveloperBuild && !Application.isEditor);
								try
								{
									IEnumerator enumerator = PushGoogleCoroutine();
									while (enumerator.MoveNext())
									{
										yield return null;
									}
									break;
								}
								finally
								{
									scopeLogger2.Dispose();
								}
							}
							Debug.LogWarning("Failed to push skins with status: " + requestStatus);
							yield return delay;
						}
					}
					i++;
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		internal IEnumerator PushGoogleCoroutine()
		{
			if (!Ready)
			{
				yield break;
			}
			string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.PushGoogleCoroutine()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(thisMethod, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				SkinsSynchronizerGoogleSavedGameFacade googleSavedGamesFacade = default(SkinsSynchronizerGoogleSavedGameFacade);
				WaitForSeconds delay = new WaitForSeconds(30f);
				int i = 0;
				while (true)
				{
					SkinsMemento localSkins = LoadLocalSkins();
					string localSkinsAsString = localSkins.ToString();
					string callee = string.Format(CultureInfo.InvariantCulture, "Push and wait {0} ({1})", localSkinsAsString, i);
					using (ScopeLogger logger = new ScopeLogger(thisMethod, callee, Defs.IsDeveloperBuild && !Application.isEditor))
					{
						Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> future = googleSavedGamesFacade.Push(localSkins);
						while (!future.IsCompleted)
						{
							yield return null;
						}
						logger.Dispose();
						if (future.IsFaulted)
						{
							Exception ex = future.Exception.InnerExceptions.FirstOrDefault() ?? future.Exception;
							Debug.LogWarning("Failed to push skins with exception: " + ex.Message);
							yield return delay;
						}
						else
						{
							SavedGameRequestStatus requestStatus = future.Result.RequestStatus;
							if (requestStatus == SavedGameRequestStatus.Success)
							{
								if (Defs.IsDeveloperBuild)
								{
									ISavedGameMetadata metadata = future.Result.Value;
									string description = ((metadata == null) ? string.Empty : metadata.Description);
									Debug.LogFormat("[Skins] Succeeded to push skins {0}: '{1}'", localSkinsAsString, description);
								}
								break;
							}
							Debug.LogWarning("Failed to push skins with status: " + requestStatus);
							yield return delay;
						}
					}
					i++;
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void OverwriteLocalSkins(SkinsMemento localSkins, SkinsMemento cloudSkins)
		{
			HashSet<string> hashSet = new HashSet<string>(localSkins.DeletedSkins.Concat(cloudSkins.DeletedSkins));
			Dictionary<string, string> dictionary = new Dictionary<string, string>(localSkins.Skins.Count);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>(localSkins.Skins.Count);
			foreach (SkinMemento skin in cloudSkins.Skins)
			{
				if (!hashSet.Contains(skin.Id))
				{
					dictionary[skin.Id] = skin.Skin;
					dictionary2[skin.Id] = skin.Name;
				}
			}
			foreach (SkinMemento skin2 in localSkins.Skins)
			{
				if (!hashSet.Contains(skin2.Id))
				{
					dictionary[skin2.Id] = skin2.Skin;
					dictionary2[skin2.Id] = skin2.Name;
				}
			}
			string value = Json.Serialize(dictionary);
			PlayerPrefs.SetString("User Skins", value);
			string value2 = Json.Serialize(dictionary2);
			PlayerPrefs.SetString("User Name Skins", value2);
			CapeMemento capeMemento = CapeMemento.ChooseCape(localSkins.Cape, cloudSkins.Cape);
			string value3 = JsonUtility.ToJson(capeMemento);
			PlayerPrefs.SetString("NewUserCape", value3);
			RefreshGui(dictionary, dictionary2, capeMemento);
			PlayerPrefs.Save();
		}

		private void RefreshGui(Dictionary<string, string> skins, Dictionary<string, string> skinNames, CapeMemento cape)
		{
			if (ShopNGUIController.sharedShop == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> skin in skins)
			{
				if (!SkinsController.skinsForPers.ContainsKey(skin.Key))
				{
					Texture2D value = SkinsController.TextureFromString(skin.Value);
					SkinsController.skinsForPers.Add(skin.Key, value);
					SkinsController.customSkinIds.Add(skin.Key);
				}
			}
			foreach (KeyValuePair<string, string> skinName in skinNames)
			{
				SkinsController.skinsNamesForPers[skinName.Key] = skinName.Value;
			}
			SkinsController.capeUserTexture = SkinsController.TextureFromString(cape.Cape, 32);
			if (ShopNGUIController.GuiActive)
			{
				ShopNGUIController.sharedShop.ReloadGridOrCarousel(ShopNGUIController.sharedShop.CurrentItem);
				ShopNGUIController.sharedShop.ShowLockOrPropertiesAndButtons();
			}
		}

		private SkinsMemento LoadLocalSkins()
		{
			//Discarded unreachable code: IL_01b6
			string callee = string.Format(CultureInfo.InvariantCulture, "{0}.LoadLocalSkins()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
			try
			{
				string @string = PlayerPrefs.GetString("DeletedSkins", string.Empty);
				List<object> list = Json.Deserialize(@string) as List<object>;
				List<string> deletedSkins = ((list == null) ? new List<string>() : list.OfType<string>().ToList());
				string string2 = PlayerPrefs.GetString("User Skins", string.Empty);
				string string3 = PlayerPrefs.GetString("NewUserCape", string.Empty);
				CapeMemento cape = Tools.DeserializeJson<CapeMemento>(string3);
				Dictionary<string, object> dictionary = (Json.Deserialize(string2) as Dictionary<string, object>) ?? new Dictionary<string, object>();
				List<SkinMemento> list2 = new List<SkinMemento>(dictionary.Count);
				if (dictionary.Count == 0)
				{
					Debug.LogFormat("Deserialized skins are empty: {0}", string2);
					return new SkinsMemento(list2, deletedSkins, cape);
				}
				string string4 = PlayerPrefs.GetString("User Name Skins", string.Empty);
				Dictionary<string, object> dict = (Json.Deserialize(string4) as Dictionary<string, object>) ?? new Dictionary<string, object>();
				foreach (KeyValuePair<string, object> item2 in dictionary)
				{
					string key = item2.Key;
					string value;
					if (!dict.TryGetValue<string>(key, out value))
					{
						value = string.Empty;
					}
					string skin = (item2.Value as string) ?? string.Empty;
					SkinMemento item = new SkinMemento(key, value, skin);
					list2.Add(item);
				}
				return new SkinsMemento(list2, deletedSkins, cape);
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void SyncSkinsAndCapeIos()
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer && !Storager.ICloudAvailable)
			{
			}
		}
	}
}
