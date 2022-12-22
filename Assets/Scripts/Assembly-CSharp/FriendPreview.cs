using System;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public sealed class FriendPreview : MonoBehaviour
{
	public UILabel nm;

	public UITexture preview;

	public Texture2D mySkin;

	public UISprite rank;

	public bool facebookFriend;

	public bool ClanMember;

	public bool ClanInvite;

	public GameObject avatarButton;

	public GameObject join;

	public GameObject delete;

	public GameObject addFacebookFriend;

	public GameObject cancel;

	public GameObject leader;

	public string id;

	public bool isInviteFromUs;

	public bool IsClanLeader;

	public UITexture ClanLogo;

	public UILabel clanName;

	public GameObject onlineStateContainer;

	public UILabel offline;

	public UILabel onlineLab;

	public UILabel playing;

	private float timeLastCheck;

	private readonly Lazy<ClansGUIController> _clansGuiController;

	private float inactivityStartTm;

	private bool _disableButtons;

	public FriendPreview()
	{
		_clansGuiController = new Lazy<ClansGUIController>(base.GetComponentInParent<ClansGUIController>);
	}

	public void HandleAvatarClick()
	{
		Action<bool> onCloseEvent = delegate
		{
		};
		FriendsController.ShowProfile(id, ProfileWindowType.other, onCloseEvent);
	}

	private void Start()
	{
		if (!facebookFriend && !ClanMember && !ClanInvite)
		{
			FriendsGUIController.UpdaeOnlineEvent = (Action)Delegate.Combine(FriendsGUIController.UpdaeOnlineEvent, new Action(UpdateOnline));
		}
		if (isInviteFromUs && preview != null)
		{
			preview.alpha = 0.4f;
		}
		join.SetActive(join.activeSelf && (!facebookFriend || ClanMember) && !ClanInvite);
		delete.SetActive(delete.activeSelf && !facebookFriend && !ClanInvite);
		join.GetComponent<UIButton>().isEnabled = false;
		if (onlineStateContainer != null)
		{
			onlineStateContainer.SetActive(!facebookFriend);
		}
		addFacebookFriend.SetActive(facebookFriend || ClanInvite);
		if (ClanInvite)
		{
			addFacebookFriend.GetComponent<UIButton>().isEnabled = !FriendsController.sharedController.ClanLimitReached;
			UIButton component = avatarButton.GetComponent<UIButton>();
			if (component != null)
			{
				EventDelegate.Callback call = delegate
				{
					if (_clansGuiController.Value != null)
					{
						ClansGUIController.State previousState = _clansGuiController.Value.CurrentState;
						Action<bool> onCloseEvent = delegate
						{
							if (_clansGuiController.Value != null)
							{
								_clansGuiController.Value.CurrentState = previousState;
								_clansGuiController.Value.ShowAddMembersScreen();
							}
						};
						FriendsController.ShowProfile(id, ProfileWindowType.other, onCloseEvent);
						_clansGuiController.Value.CurrentState = ClansGUIController.State.ProfileDetails;
					}
				};
				EventDelegate item = new EventDelegate(call);
				component.onClick.Add(item);
			}
		}
		bool flag = true;
		cancel.SetActive(ClanInvite && flag);
		avatarButton.GetComponent<UIButton>().enabled = !facebookFriend;
		if (facebookFriend || ClanInvite)
		{
			UnityEngine.Object.Destroy(avatarButton.GetComponent<FriendPreviewClicker>());
		}
		UpdateInfo();
	}

	public void SetSkin(string skinStr)
	{
		bool flag = true;
		if (!string.IsNullOrEmpty(skinStr) && !skinStr.Equals("empty"))
		{
			byte[] data = Convert.FromBase64String(skinStr);
			Texture2D texture2D = new Texture2D(64, 32);
			texture2D.LoadImage(data);
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			mySkin = texture2D;
		}
		else
		{
			mySkin = Resources.Load(ResPath.Combine(Defs.MultSkinsDirectoryName, "multi_skin_1")) as Texture2D;
			flag = false;
		}
		Texture2D texture2D2 = new Texture2D(20, 20, TextureFormat.ARGB32, false);
		for (int i = 0; i < 20; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				texture2D2.SetPixel(i, j, Color.clear);
			}
		}
		texture2D2.SetPixels(6, 6, 8, 8, GetPixelsByRect(mySkin, new Rect(8f, 16f, 8f, 8f)));
		texture2D2.SetPixels(6, 0, 8, 6, GetPixelsByRect(mySkin, new Rect(20f, 6f, 8f, 6f)));
		texture2D2.SetPixels(2, 0, 4, 6, GetPixelsByRect(mySkin, new Rect(44f, 6f, 4f, 6f)));
		texture2D2.SetPixels(14, 0, 4, 6, GetPixelsByRect(mySkin, new Rect(44f, 6f, 4f, 6f)));
		texture2D2.anisoLevel = 1;
		texture2D2.mipMapBias = -0.5f;
		texture2D2.Apply();
		texture2D2.filterMode = FilterMode.Point;
		if (flag)
		{
			UnityEngine.Object.Destroy(mySkin);
		}
		Texture mainTexture = preview.mainTexture;
		preview.mainTexture = texture2D2;
		if (mainTexture != null && !mainTexture.name.Equals("dude") && !mainTexture.name.Equals("multi_skin_1"))
		{
			UnityEngine.Object.Destroy(mainTexture);
		}
	}

	private void UpdateInfo()
	{
		if (facebookFriend || id == null)
		{
			return;
		}
		if (!ClanMember)
		{
			Dictionary<string, object> value;
			object value2;
			if (!FriendsController.sharedController.playersInfo.TryGetValue(id, out value) || !value.TryGetValue("player", out value2))
			{
				return;
			}
			Dictionary<string, object> dictionary = value2 as Dictionary<string, object>;
			nm.text = dictionary["nick"] as string;
			rank.spriteName = "Rank_" + Convert.ToString(dictionary["rank"]);
			string skin = dictionary["skin"] as string;
			SetSkin(skin);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				dictionary2.Add(item.Key, Convert.ToString(item.Value));
			}
			FillClanAttrs(dictionary2);
			return;
		}
		foreach (Dictionary<string, string> clanMember in FriendsController.sharedController.clanMembers)
		{
			string value3;
			if (!clanMember.TryGetValue("id", out value3) || !id.Equals(value3))
			{
				continue;
			}
			if (clanMember.ContainsKey("nick"))
			{
				nm.text = clanMember["nick"];
			}
			if (clanMember.ContainsKey("rank"))
			{
				rank.spriteName = "Rank_" + clanMember["rank"];
			}
			if (clanMember.ContainsKey("skin"))
			{
				string skin2 = clanMember["skin"];
				SetSkin(skin2);
			}
			Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
			object value4;
			if (FriendsController.sharedController.playersInfo.ContainsKey(value3) && FriendsController.sharedController.playersInfo[value3].TryGetValue("player", out value4))
			{
				Dictionary<string, object> dictionary4 = value4 as Dictionary<string, object>;
				foreach (KeyValuePair<string, object> item2 in dictionary4)
				{
					dictionary3.Add(item2.Key, Convert.ToString(item2.Value));
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(FriendsController.sharedController.clanName))
				{
					dictionary3.Add("clan_name", FriendsController.sharedController.clanName);
				}
				if (!string.IsNullOrEmpty(FriendsController.sharedController.clanLeaderID))
				{
					dictionary3.Add("clan_creator_id", FriendsController.sharedController.clanLeaderID);
				}
			}
			FillClanAttrs(dictionary3);
			break;
		}
	}

	public void FillClanAttrs(Dictionary<string, string> plDict)
	{
		if (plDict == null)
		{
			return;
		}
		if (ClanMember)
		{
			string text = null;
			if (!string.IsNullOrEmpty(FriendsController.sharedController.clanLogo))
			{
				text = FriendsController.sharedController.clanLogo;
			}
			else if (plDict.ContainsKey("clan_logo") && plDict["clan_logo"] != null && plDict["clan_logo"] != null && !plDict["clan_logo"].Equals("null"))
			{
				text = plDict["clan_logo"];
			}
			if (text != null)
			{
				ClanLogo.gameObject.SetActive(true);
				try
				{
					byte[] data = Convert.FromBase64String(text);
					Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
					texture2D.LoadImage(data);
					texture2D.filterMode = FilterMode.Point;
					texture2D.Apply();
					Texture mainTexture = ClanLogo.mainTexture;
					ClanLogo.mainTexture = texture2D;
					if (mainTexture != null)
					{
						UnityEngine.Object.Destroy(mainTexture);
					}
				}
				catch (Exception)
				{
					Texture mainTexture2 = ClanLogo.mainTexture;
					ClanLogo.mainTexture = null;
					if (mainTexture2 != null)
					{
						UnityEngine.Object.Destroy(mainTexture2);
					}
				}
			}
		}
		else if (plDict.ContainsKey("clan_logo") && !string.IsNullOrEmpty(plDict["clan_logo"]) && !plDict["clan_logo"].Equals("null"))
		{
			ClanLogo.gameObject.SetActive(true);
			try
			{
				byte[] data2 = Convert.FromBase64String(plDict["clan_logo"]);
				Texture2D texture2D2 = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
				texture2D2.LoadImage(data2);
				texture2D2.filterMode = FilterMode.Point;
				texture2D2.Apply();
				Texture mainTexture3 = ClanLogo.mainTexture;
				ClanLogo.mainTexture = texture2D2;
				if (mainTexture3 != null)
				{
					UnityEngine.Object.Destroy(mainTexture3);
				}
			}
			catch (Exception)
			{
				Texture mainTexture4 = ClanLogo.mainTexture;
				ClanLogo.mainTexture = null;
				if (mainTexture4 != null)
				{
					UnityEngine.Object.Destroy(mainTexture4);
				}
			}
		}
		else
		{
			ClanLogo.gameObject.SetActive(false);
		}
		if (plDict.ContainsKey("clan_name") && plDict["clan_name"] != null && !plDict["clan_name"].Equals("null"))
		{
			clanName.gameObject.SetActive(true);
			string text2 = plDict["clan_name"];
			int num = 12;
			if (text2 != null && text2.Length > num)
			{
				text2 = string.Format("{0}..{1}", text2.Substring(0, (num - 2) / 2), text2.Substring(text2.Length - (num - 2) / 2, (num - 2) / 2));
			}
			if (text2 != null)
			{
				clanName.text = text2;
			}
		}
		else
		{
			clanName.gameObject.SetActive(false);
		}
		if (plDict.ContainsKey("clan_creator_id") && plDict["clan_creator_id"] != null && id != null)
		{
			bool flag = plDict["clan_creator_id"].Equals(id);
			leader.SetActive(flag);
			avatarButton.GetComponent<UIButton>().normalSprite = ((!flag) ? "avatar_frame" : "avatar_leader_frame");
		}
	}

	private Texture2D getTexFromTexByRect(Texture2D texForCut, Rect rectForCut)
	{
		Color[] pixels = texForCut.GetPixels((int)rectForCut.x, (int)rectForCut.y, (int)rectForCut.width, (int)rectForCut.height);
		Texture2D texture2D = new Texture2D((int)rectForCut.width, (int)rectForCut.height);
		texture2D.filterMode = FilterMode.Point;
		texture2D.SetPixels(pixels);
		texture2D.Apply();
		return texture2D;
	}

	private Color[] GetPixelsByRect(Texture2D texture, Rect rect)
	{
		return texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
	}

	private void UpdateOnline()
	{
		if (facebookFriend)
		{
			return;
		}
		if (FriendsController.sharedController.onlineInfo.ContainsKey(id))
		{
			string text = FriendsController.sharedController.onlineInfo[id]["game_mode"];
			string s = FriendsController.sharedController.onlineInfo[id]["delta"];
			string text2 = FriendsController.sharedController.onlineInfo[id]["protocol"];
			int num = int.Parse(text);
			num = ((num <= 99) ? (-1) : (num / 100));
			int result;
			if (!int.TryParse(text, out result))
			{
				result = -1;
			}
			else
			{
				if (result > 99)
				{
					result -= num * 100;
				}
				result /= 10;
			}
			int result2;
			if (!int.TryParse(s, out result2))
			{
				return;
			}
			if ((float)result2 > FriendsController.onlineDelta || (num != 3 && ((num != (int)ConnectSceneNGUIController.myPlatformConnect && num != -1) || ExpController.GetOurTier() != result)))
			{
				offline.gameObject.SetActive(true);
				onlineLab.gameObject.SetActive(false);
				playing.gameObject.SetActive(false);
				join.GetComponent<UIButton>().isEnabled = false;
				return;
			}
			string text3 = text2;
			string multiplayerProtocolVersion = GlobalGameController.MultiplayerProtocolVersion;
			int result3;
			if (text == null || !int.TryParse(text, out result3))
			{
				return;
			}
			if (result3 == -1)
			{
				offline.gameObject.SetActive(false);
				onlineLab.gameObject.SetActive(true);
				playing.gameObject.SetActive(false);
				join.GetComponent<UIButton>().isEnabled = false;
				return;
			}
			offline.gameObject.SetActive(false);
			onlineLab.gameObject.SetActive(false);
			playing.gameObject.SetActive(true);
			join.GetComponent<UIButton>().isEnabled = !_disableButtons && multiplayerProtocolVersion == text3;
			string value;
			if (FriendsController.sharedController.onlineInfo[id].TryGetValue("map", out value))
			{
				SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(value));
				if (infoScene == null)
				{
					join.GetComponent<UIButton>().isEnabled = false;
				}
			}
		}
		else
		{
			offline.gameObject.SetActive(true);
			onlineLab.gameObject.SetActive(false);
			playing.gameObject.SetActive(false);
			join.GetComponent<UIButton>().isEnabled = false;
		}
	}

	public void DisableButtons()
	{
		_disableButtons = true;
		delete.SetActive(false);
		if (facebookFriend)
		{
			addFacebookFriend.SetActive(false);
		}
		inactivityStartTm = Time.realtimeSinceStartup;
		UpdateOnline();
	}

	private bool IsFriendInClan()
	{
		Dictionary<string, Dictionary<string, object>> playersInfo = FriendsController.sharedController.playersInfo;
		if (playersInfo == null)
		{
			return false;
		}
		if (!playersInfo.ContainsKey(id))
		{
			return false;
		}
		if (!playersInfo[id].ContainsKey("player"))
		{
			return false;
		}
		Dictionary<string, object> dictionary = playersInfo[id]["player"] as Dictionary<string, object>;
		if (dictionary == null)
		{
			return false;
		}
		if (!dictionary.ContainsKey("clan_creator_id"))
		{
			return false;
		}
		string value = Convert.ToString(dictionary["clan_creator_id"]);
		return !string.IsNullOrEmpty(value);
	}

	private void Update()
	{
		if (ClanInvite)
		{
			addFacebookFriend.SetActive(!FriendsController.sharedController.ClanSentInvites.Contains(id) && !FriendsController.sharedController.clanSentInvitesLocal.Contains(id) && !FriendsController.sharedController.friendsDeletedLocal.Contains(id));
			addFacebookFriend.GetComponent<UIButton>().isEnabled = !FriendsController.sharedController.ClanLimitReached;
			cancel.SetActive(FriendsController.sharedController.ClanSentInvites.Contains(id) && !FriendsController.sharedController.clanCancelledInvitesLocal.Contains(id) && !FriendsController.sharedController.friendsDeletedLocal.Contains(id));
		}
		if (ClanMember)
		{
			bool flag = false;
			foreach (Dictionary<string, string> clanMember in FriendsController.sharedController.clanMembers)
			{
				if (clanMember.ContainsKey("id") && clanMember["id"].Equals(id))
				{
					flag = true;
					break;
				}
			}
			delete.SetActive(flag && !FriendsController.sharedController.clanDeletedLocal.Contains(id) && FriendsController.sharedController.id != null && FriendsController.sharedController.id.Equals(FriendsController.sharedController.clanLeaderID));
		}
		if (Time.realtimeSinceStartup - inactivityStartTm > 25f)
		{
			inactivityStartTm = float.PositiveInfinity;
			if (!isInviteFromUs)
			{
				_disableButtons = false;
				UpdateOnline();
			}
			delete.SetActive(!facebookFriend && !ClanInvite);
		}
		if (Time.realtimeSinceStartup - timeLastCheck > 1f)
		{
			timeLastCheck = Time.realtimeSinceStartup;
			UpdateOnline();
			UpdateInfo();
		}
		if (!facebookFriend || _disableButtons)
		{
			return;
		}
		bool flag2 = false;
		if (FriendsController.sharedController.friends != null)
		{
			foreach (string friend in FriendsController.sharedController.friends)
			{
				if (friend.Equals(id))
				{
					flag2 = true;
					break;
				}
			}
		}
		addFacebookFriend.SetActive(!flag2);
	}

	private void OnDestroy()
	{
		FriendsGUIController.UpdaeOnlineEvent = (Action)Delegate.Remove(FriendsGUIController.UpdaeOnlineEvent, new Action(UpdateOnline));
	}
}
