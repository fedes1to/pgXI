using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rilisoft
{
	[Obsolete]
	internal sealed class AnalyticsLegacy
	{
		private static string GetFlurryApiKey()
		{
			return GetFlurryApiKeyRelease();
		}

		private static string GetFlurryApiKeyDebug()
		{
			return string.Empty;
		}

		private static string GetFlurryApiKeyRelease()
		{
			return "J8K92PR3VD22BX8ZSZ7W";
		}

		private static string GetPlayingMode()
		{
			StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
			if (ordinalIgnoreCase.Equals(SceneManager.GetActiveScene().name, Defs.MainMenuScene))
			{
				return "Main Menu";
			}
			if (ordinalIgnoreCase.Equals(SceneLoader.ActiveSceneName, "ConnectScene"))
			{
				return "Connect Scene";
			}
			if (ordinalIgnoreCase.Equals(SceneLoader.ActiveSceneName, "ConnectSceneSandbox"))
			{
				return "Connect Scene Sandbox";
			}
			if (!Defs.IsSurvival && !Defs.isMulti)
			{
				return "Campaign";
			}
			if (Defs.IsSurvival)
			{
				return (!Defs.isMulti) ? "Survival" : "Time Survival";
			}
			if (Defs.isCompany)
			{
				return "Team Battle";
			}
			if (Defs.isFlag)
			{
				return "Flag Capture";
			}
			if (Defs.isHunger)
			{
				return "Deadly Games";
			}
			if (Defs.isCapturePoints)
			{
				return "Capture Points";
			}
			return (!Defs.isInet) ? "Deathmatch Local" : "Deathmatch Worldwide";
		}

		private static string CurrentContextForNonePlaceInBecomePaying()
		{
			string text = string.Empty;
			try
			{
				if (Defs.inRespawnWindow)
				{
					text += " Killcam";
				}
				if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
				{
					text += " PlayerExists";
				}
				if (NetworkStartTableNGUIController.IsEndInterfaceShown())
				{
					text += " NetworkStartTable_End";
				}
				if (NetworkStartTableNGUIController.IsStartInterfaceShown())
				{
					text += " NetworkStartTable_Start";
				}
				if (ShopNGUIController.GuiActive)
				{
					text += " InShop";
				}
				string text2 = (ModeNameForPurchasesAnalytics(false) ?? string.Empty).Replace(" ", string.Empty);
				if (text2 != string.Empty)
				{
					text = text + " " + text2;
					return text;
				}
				return text;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in CurrentContextForNonePlaceInBecomePaying: " + ex);
				return text;
			}
		}

		private static string ModeNameForPurchasesAnalytics(bool forNormalMultyModesUseMultyplayer = false)
		{
			//Discarded unreachable code: IL_013a
			try
			{
				if (!Defs.IsSurvival && !Defs.isMulti)
				{
					return "Campaign";
				}
				if (Defs.IsSurvival && !Defs.isMulti)
				{
					return "Arena";
				}
				string name = SceneManager.GetActiveScene().name;
				if (Defs.isMulti && name != Defs.MainMenuScene && name != "Clans")
				{
					if (Defs.isDaterRegim)
					{
						return "Sandbox";
					}
					if (forNormalMultyModesUseMultyplayer)
					{
						return "Multiplayer";
					}
					if (Defs.isCompany)
					{
						return "Team Battle";
					}
					if (Defs.isCapturePoints)
					{
						return "Point Capture";
					}
					if (Defs.isCOOP)
					{
						return "COOP Survival";
					}
					if (Defs.isFlag)
					{
						return "Flag Capture";
					}
					if (Defs.isHunger)
					{
						return "Deadly Games";
					}
					return "Deathmatch";
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in ModeNameForPurchasesAnalytics: " + ex);
				return string.Empty;
			}
			return string.Empty;
		}

		private static bool IsAdditionalLoggingAvailable()
		{
			//Discarded unreachable code: IL_008e, IL_00ab
			try
			{
				if (BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer)
				{
					return false;
				}
				return File.Exists(ConvertFromBase64("L0FwcGxpY2F0aW9ucy9DeWRpYS5hcHA=")) || File.Exists(ConvertFromBase64("L0xpYnJhcnkvTW9iaWxlU3Vic3RyYXRlL01vYmlsZVN1YnN0cmF0ZS5keWxpYg==")) || File.Exists(ConvertFromBase64("L2Jpbi9iYXNo")) || File.Exists(ConvertFromBase64("L3Vzci9zYmluL3NzaGQ=")) || File.Exists(ConvertFromBase64("L2V0Yy9hcHQ=")) || Directory.Exists(ConvertFromBase64("L3ByaXZhdGUvdmFyL2xpYi9hcHQv"));
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Exception in IsAdditionalLoggingAvailable: " + ex);
				return false;
			}
		}

		private static string ConvertFromBase64(string s)
		{
			byte[] array = Convert.FromBase64String(s);
			return Encoding.UTF8.GetString(array, 0, array.Length);
		}

		private static void CheckForEdnermanApp()
		{
			Defs.EnderManAvailable = false;
			HttpWebRequest request = RequestAppWithID(MainMenu.iTunesEnderManID);
			DoWithResponse(request, delegate(HttpWebResponse response)
			{
				string text = new StreamReader(response.GetResponseStream()).ReadToEnd();
				if (text.Contains("Slender") && text.Length > 250)
				{
					Defs.EnderManAvailable = true;
				}
			});
		}

		private static HttpWebRequest RequestAppWithID(string id)
		{
			return (HttpWebRequest)WebRequest.Create("http://itunes.apple.com/lookup?id=" + id);
		}

		private static void DoWithResponse(HttpWebRequest request, Action<HttpWebResponse> responseAction)
		{
			Action action = delegate
			{
				request.BeginGetResponse(delegate(IAsyncResult iar)
				{
					HttpWebResponse obj = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
					responseAction(obj);
				}, request);
			};
			action.BeginInvoke(delegate(IAsyncResult iar)
			{
				Action action2 = (Action)iar.AsyncState;
				action2.EndInvoke(iar);
			}, action);
		}
	}
}
