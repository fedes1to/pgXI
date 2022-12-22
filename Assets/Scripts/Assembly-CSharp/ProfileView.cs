using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

internal sealed class ProfileView : MonoBehaviour
{
	public class InputEventArgs : EventArgs
	{
		public string Input { get; private set; }

		public InputEventArgs(string input)
		{
			Input = input ?? string.Empty;
		}
	}

	public UILabel pixelgunFriendsID;

	public GameObject interfaceHolder;

	public UIRoot interfaceHolder2d;

	public GameObject worldHolder3d;

	public UILabel totalWeeklyWinsCount;

	public UILabel deathmatchWinCount;

	public UILabel teamBattleWinCount;

	public UILabel capturePointCount;

	public UILabel deadlyGamesWinCount;

	public UILabel flagCaptureWinCount;

	public UILabel duelCount;

	public UILabel totalWinCount;

	public UILabel lbGameTotalKills;

	public UILabel lbGameKillrate;

	public UILabel lbGameAccuracy;

	public UILabel lbGameLikes;

	public UILabel coopTimeSurvivalPointCount;

	public UILabel waveCountLabel;

	public UILabel killedCountLabel;

	public UILabel survivalScoreLabel;

	public UILabel box1StarsLabel;

	public UILabel box2StarsLabel;

	public UILabel box3StarsLabel;

	public UILabel secretCoinsLabel;

	public UILabel secretGemsLabel;

	public UIInputRilisoft nicknameInput;

	public UITexture clanLogo;

	public ButtonHandler backButton;

	public UIButton achievementsButton;

	public UIButton leaderboardsButton;

	public UIButton copyIdButton;

	public CharacterView characterView;

	public string DeathmatchWinCount
	{
		get
		{
			return GetText(deathmatchWinCount);
		}
		set
		{
			SetText(deathmatchWinCount, value);
		}
	}

	public string TotalWeeklyWinCount
	{
		get
		{
			return GetText(totalWeeklyWinsCount);
		}
		set
		{
			SetText(totalWeeklyWinsCount, value);
		}
	}

	public string TeamBattleWinCount
	{
		get
		{
			return GetText(teamBattleWinCount);
		}
		set
		{
			SetText(teamBattleWinCount, value);
		}
	}

	public string CapturePointWinCount
	{
		get
		{
			return GetText(capturePointCount);
		}
		set
		{
			SetText(capturePointCount, value);
		}
	}

	public string DuelWinCount
	{
		get
		{
			return GetText(duelCount);
		}
		set
		{
			SetText(duelCount, value);
		}
	}

	public string DeadlyGamesWinCount
	{
		get
		{
			return GetText(deadlyGamesWinCount);
		}
		set
		{
			SetText(deadlyGamesWinCount, value);
		}
	}

	public string FlagCaptureWinCount
	{
		get
		{
			return GetText(flagCaptureWinCount);
		}
		set
		{
			SetText(flagCaptureWinCount, value);
		}
	}

	public string TotalWinCount
	{
		get
		{
			return GetText(totalWinCount);
		}
		set
		{
			SetText(totalWinCount, value);
		}
	}

	public string PixelgunFriendsID
	{
		get
		{
			return GetText(pixelgunFriendsID);
		}
		set
		{
			SetText(pixelgunFriendsID, "ID: " + value);
		}
	}

	public string CoopTimeSurvivalPointCount
	{
		get
		{
			return GetText(coopTimeSurvivalPointCount);
		}
		set
		{
			SetText(coopTimeSurvivalPointCount, value);
		}
	}

	public string GameTotalKills
	{
		get
		{
			return GetText(lbGameTotalKills);
		}
		set
		{
			SetText(lbGameTotalKills, value);
		}
	}

	public string GameKillrate
	{
		get
		{
			return GetText(lbGameKillrate);
		}
		set
		{
			SetText(lbGameKillrate, value);
		}
	}

	public string GameAccuracy
	{
		get
		{
			return GetText(lbGameAccuracy);
		}
		set
		{
			SetText(lbGameAccuracy, value);
		}
	}

	public string GameLikes
	{
		get
		{
			return GetText(lbGameLikes);
		}
		set
		{
			SetText(lbGameLikes, value);
		}
	}

	public string WaveCountLabel
	{
		get
		{
			return GetText(waveCountLabel);
		}
		set
		{
			SetText(waveCountLabel, value);
		}
	}

	public string KilledCountLabel
	{
		get
		{
			return GetText(killedCountLabel);
		}
		set
		{
			SetText(killedCountLabel, value);
		}
	}

	public string SurvivalScoreLabel
	{
		get
		{
			return GetText(survivalScoreLabel);
		}
		set
		{
			SetText(survivalScoreLabel, value);
		}
	}

	public string Box1StarsLabel
	{
		get
		{
			return GetText(box1StarsLabel);
		}
		set
		{
			SetText(box1StarsLabel, value);
			if (box1StarsLabel != null)
			{
				UISprite componentInChildren = box1StarsLabel.GetComponentInChildren<UISprite>();
				if (componentInChildren != null)
				{
					Vector3 localPosition = box1StarsLabel.transform.localPosition;
					componentInChildren.transform.localPosition = new Vector3(-box1StarsLabel.width, localPosition.y, localPosition.z);
				}
			}
		}
	}

	public string Box2StarsLabel
	{
		get
		{
			return GetText(box2StarsLabel);
		}
		set
		{
			SetText(box2StarsLabel, value);
			if (box2StarsLabel != null)
			{
				UISprite componentInChildren = box2StarsLabel.GetComponentInChildren<UISprite>();
				if (componentInChildren != null)
				{
					Vector3 localPosition = box2StarsLabel.transform.localPosition;
					componentInChildren.transform.localPosition = new Vector3(-box2StarsLabel.width, localPosition.y, localPosition.z);
				}
			}
		}
	}

	public string Box3StarsLabel
	{
		get
		{
			return GetText(box3StarsLabel);
		}
		set
		{
			SetText(box3StarsLabel, value);
			if (box3StarsLabel != null)
			{
				UISprite componentInChildren = box3StarsLabel.GetComponentInChildren<UISprite>();
				if (componentInChildren != null)
				{
					Vector3 localPosition = box3StarsLabel.transform.localPosition;
					componentInChildren.transform.localPosition = new Vector3(-box3StarsLabel.width, localPosition.y, localPosition.z);
				}
			}
		}
	}

	public string SecretCoinsLabel
	{
		get
		{
			return GetText(secretCoinsLabel);
		}
		set
		{
			if (!(secretCoinsLabel == null))
			{
				SetText(secretCoinsLabel, value);
				UISprite componentInChildren = secretCoinsLabel.GetComponentInChildren<UISprite>();
				if (!(componentInChildren == null))
				{
					Vector3 localPosition = secretCoinsLabel.transform.localPosition;
					componentInChildren.transform.localPosition = new Vector3(-secretCoinsLabel.width, localPosition.y, localPosition.z);
				}
			}
		}
	}

	public string SecretGemsLabel
	{
		get
		{
			return GetText(secretGemsLabel);
		}
		set
		{
			if (!(secretGemsLabel == null))
			{
				SetText(secretGemsLabel, value);
				UISprite componentInChildren = secretGemsLabel.GetComponentInChildren<UISprite>();
				if (!(componentInChildren == null))
				{
					Vector3 localPosition = secretGemsLabel.transform.localPosition;
					componentInChildren.transform.localPosition = new Vector3(-secretGemsLabel.width, localPosition.y, localPosition.z);
				}
			}
		}
	}

	public string Nickname
	{
		get
		{
			return (!(nicknameInput != null)) ? string.Empty : (nicknameInput.value ?? string.Empty);
		}
		set
		{
			if (nicknameInput != null)
			{
				nicknameInput.value = value ?? string.Empty;
			}
		}
	}

	private bool IdPlayerExist
	{
		get
		{
			return FriendsController.sharedController != null && !string.IsNullOrEmpty(FriendsController.sharedController.id);
		}
	}

	public event EventHandler BackButtonPressed
	{
		add
		{
			if (backButton != null)
			{
				backButton.Clicked += value;
			}
		}
		remove
		{
			if (backButton != null)
			{
				backButton.Clicked -= value;
			}
		}
	}

	public event EventHandler<InputEventArgs> NicknameInput;

	private void Awake()
	{
		if (copyIdButton != null && BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
		{
			copyIdButton.gameObject.SetActive(false);
		}
	}

	public void OnSubmit()
	{
		if (!(nicknameInput == null))
		{
			EventHandler<InputEventArgs> eventHandler = this.NicknameInput;
			if (eventHandler != null)
			{
				eventHandler(this, new InputEventArgs(nicknameInput.value));
			}
		}
	}

	public void OnCopyIdClick()
	{
		FriendsController.CopyMyIdToClipboard();
	}

	public void SetClanLogo(string logoBase64)
	{
		if (clanLogo == null)
		{
			Debug.LogWarning("clanLogo == null");
			return;
		}
		Texture2D texture2D = CharacterView.GetClanLogo(logoBase64);
		if (texture2D == null)
		{
			clanLogo.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			clanLogo.mainTexture = texture2D;
		}
	}

	public void SetWeaponAndSkin(string tg, bool replaceRemovedWeapons)
	{
		characterView.SetWeaponAndSkin(tg, SkinsController.currentSkinForPers, replaceRemovedWeapons);
		ShopNGUIController.DisableLightProbesRecursively(characterView.gameObject);
	}

	public void UpdateHat(string hat)
	{
		characterView.UpdateHat(hat);
		ShopNGUIController.SetPersHatVisible(characterView.hatPoint);
	}

	public void RemoveHat()
	{
		characterView.RemoveHat();
	}

	public void UpdateMask(string mask)
	{
		characterView.UpdateMask(mask);
	}

	public void RemoveMask()
	{
		characterView.RemoveMask();
	}

	public void UpdateCape(string cape)
	{
		characterView.UpdateCape(cape);
	}

	public void RemoveCape()
	{
		characterView.RemoveCape();
	}

	public void UpdateBoots(string bs)
	{
		characterView.UpdateBoots(bs);
	}

	public void RemoveBoots()
	{
		characterView.RemoveBoots();
	}

	public void UpdateArmor(string armor)
	{
		characterView.UpdateArmor(armor);
		ShopNGUIController.SetPersArmorVisible(characterView.armorPoint);
	}

	public void RemoveArmor()
	{
		characterView.RemoveArmor();
	}

	private static string GetText(UILabel label)
	{
		if (label == null)
		{
			return string.Empty;
		}
		return label.text ?? string.Empty;
	}

	private static void SetText(UILabel label, string value)
	{
		if (label != null)
		{
			label.text = value ?? string.Empty;
		}
	}

	public void CheckBtnCopy()
	{
		StartCoroutine(Crt_CheckBtnCopy());
	}

	private IEnumerator Crt_CheckBtnCopy()
	{
		if ((bool)copyIdButton)
		{
			copyIdButton.gameObject.SetActive(false);
		}
		while (!IdPlayerExist)
		{
			yield return new WaitForSeconds(1f);
		}
		if ((bool)copyIdButton)
		{
			copyIdButton.gameObject.SetActive(true);
		}
	}
}
