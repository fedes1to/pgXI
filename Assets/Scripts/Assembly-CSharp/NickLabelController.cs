using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

public sealed class NickLabelController : MonoBehaviour
{
	public enum TypeNickLabel
	{
		None,
		Player,
		Turret,
		Point,
		PlayerLobby,
		FreeCoins,
		GetGift,
		Nest,
		InAppBonus,
		Leprechaunt
	}

	public static Camera currentCamera;

	public Transform target;

	public TypeNickLabel currentType;

	[Header("Player label")]
	public UILabel nickLabel;

	public UISprite rankTexture;

	public UILabel clanName;

	public UITexture clanTexture;

	public GameObject placeMarker;

	public UISprite multyKill;

	[Header("Lobby Exp.")]
	public GameObject expFrameLobby;

	public UISprite expProgressSprite;

	public UILabel expLabel;

	public UISprite ranksSpriteForLobby;

	[Header("Point")]
	public UISprite pointSprite;

	public UISprite pointFillSprite;

	[Header("Turret")]
	public GameObject isEnemySprite;

	public GameObject healthObj;

	public UISprite healthSprite;

	[Header("Free award")]
	public GameObject freeAwardTitle;

	public GameObject freeAwardGemsLabel;

	public GameObject freeAwardCoinsLabel;

	[Header("Gacha")]
	public GameObject GiftObject;

	public GameObject GiftLabelObject;

	public GameObject GiftTimerObject;

	public UILabel GiftTimerLabel;

	public Vector3 GiftLabelPosWithoutTimer;

	[ReadOnly]
	public Vector3 GiftLabelPos;

	[Header("Nest")]
	public GameObject NestGO;

	public UILabel NestTimerLabel;

	public Vector3 NestLabelPosWithoutTimer;

	[ReadOnly]
	public Vector3 NestLabelPos;

	[Header("InAppBonus")]
	public GameObject InAppBonusGO;

	public UILabel InAppBonusTimerLabel;

	[ReadOnly]
	public Vector3 InAppBonusLabelPos;

	[Header("Leprechaunt")]
	public GameObject LeprechauntGO;

	public UILabel LeprechauntDaysLeft;

	public GameObject LeprechauntGemsRewardAvailableGO;

	public UILabel LeprechauntGemsRewardAvailable;

	public UILabel LeprechauntRewardTimeLeft;

	public GameObject LeprechauntGemsIcon;

	public GameObject LeprechauntCoinsIcon;

	[NonSerialized]
	public Player_move_c playerScript;

	private Transform thisTransform;

	private BasePointController pointScript;

	private TurretController turretScript;

	private Vector3 offset = Vector3.up;

	private Vector3 offsetMech = new Vector3(0f, 0.5f, 0f);

	private float timeShow;

	private Vector3 posLabel;

	private int maxHeathWidth = 134;

	private float timerShowMyltyKill;

	private float maxTimerShowMultyKill = 5f;

	private float minScale = 0.5f;

	private float minDist = 10f;

	private float maxDist = 30f;

	private Vector2 coefScreen = new Vector2((float)Screen.width * 768f / (float)Screen.height, 768f);

	private string myLobbyNickname;

	private bool isHideLabel;

	private void Awake()
	{
		thisTransform = base.transform;
		HideLabel();
	}

	public void StartShow(TypeNickLabel _type, Transform _target)
	{
		thisTransform = base.transform;
		for (int i = 0; i < thisTransform.childCount; i++)
		{
			thisTransform.GetChild(i).gameObject.SetActive(false);
		}
		currentType = _type;
		target = _target;
		base.gameObject.SetActive(true);
		placeMarker.SetActive(false);
		nickLabel.color = Color.white;
		healthSprite.enabled = true;
		offset = new Vector3(0f, 0.6f, 0f);
		float num = 1f;
		SetCommandColor(0);
		if (currentType == TypeNickLabel.Player)
		{
			nickLabel.gameObject.SetActive(true);
			playerScript = target.GetComponent<Player_move_c>();
		}
		if (currentType == TypeNickLabel.PlayerLobby)
		{
			num = 1f;
			expFrameLobby.SetActive(true);
			nickLabel.gameObject.SetActive(true);
			clanName.gameObject.SetActive(true);
			clanTexture.gameObject.SetActive(true);
			offset = new Vector3(0f, 2.26f, 0f);
		}
		if (currentType == TypeNickLabel.Point)
		{
			pointScript = target.GetComponent<BasePointController>();
			pointSprite.spriteName = "Point_" + pointScript.nameBase;
			pointFillSprite.spriteName = pointSprite.spriteName;
			pointSprite.gameObject.SetActive(true);
			offset = new Vector3(0f, 2.25f, 0f);
		}
		if (currentType == TypeNickLabel.Turret)
		{
			turretScript = target.GetComponent<TurretController>();
			playerScript = ((!(turretScript.myPlayer != null)) ? null : turretScript.myPlayer.GetComponent<SkinName>().playerMoveC);
			nickLabel.gameObject.SetActive(true);
			if (!Defs.isDaterRegim)
			{
				healthObj.SetActive(true);
			}
			offset = new Vector3(0f, 1.76f, 0f);
		}
		if (currentType == TypeNickLabel.FreeCoins)
		{
			thisTransform.localScale = new Vector3(num, num, num);
			freeAwardTitle.gameObject.SetActive(true);
		}
		if (currentType == TypeNickLabel.GetGift)
		{
			GiftLabelPos = GiftObject.transform.localPosition;
			thisTransform.localScale = new Vector3(num, num, num);
			GiftObject.SetActiveSafe(true);
			CoroutineRunner.Instance.StartCoroutine(UpdateGachaNickLabel());
		}
		if (currentType == TypeNickLabel.Nest)
		{
			NestLabelPos = NestGO.transform.localPosition;
			thisTransform.localScale = new Vector3(num, num, num);
			NestGO.SetActive(true);
		}
		if (currentType == TypeNickLabel.InAppBonus)
		{
			InAppBonusLabelPos = NestGO.transform.localPosition;
			thisTransform.localScale = new Vector3(num, num, num);
			InAppBonusGO.SetActive(true);
		}
		if (currentType == TypeNickLabel.Leprechaunt)
		{
			thisTransform.localScale = new Vector3(num, num, num);
			LeprechauntGO.SetActive(true);
		}
		thisTransform.localScale = new Vector3(num, num, num);
		if (currentType == TypeNickLabel.PlayerLobby)
		{
			UpdateNickInLobby();
		}
		UpdateInfo();
		HideLabel();
	}

	private IEnumerator UpdateGachaNickLabel()
	{
		UIPanel panel = GetComponent<UIPanel>();
		while (true)
		{
			if (GiftObject.activeInHierarchy && GiftController.Instance != null)
			{
				if (GiftController.Instance.CanGetTimerGift)
				{
					GiftObject.transform.localPosition = GiftLabelPosWithoutTimer;
					GiftTimerObject.SetActiveSafe(false);
				}
				else
				{
					GiftObject.transform.localPosition = GiftLabelPos;
					string timeStr = RiliExtensions.GetTimeString((long)GiftController.Instance.TimeLeft);
					if (timeStr.IsNullOrEmpty())
					{
						GiftTimerObject.SetActiveSafe(false);
						GiftObject.transform.localPosition = GiftLabelPosWithoutTimer;
						panel.SetDirty();
						panel.Refresh();
					}
					else
					{
						GiftTimerObject.SetActiveSafe(true);
						GiftTimerLabel.text = timeStr;
					}
				}
				panel.SetDirty();
				panel.Refresh();
			}
			yield return null;
		}
	}

	public void UpdateInfo()
	{
		if (currentType == TypeNickLabel.Player)
		{
			nickLabel.text = FilterBadWorld.FilterString(playerScript.mySkinName.NickName);
		}
		if (currentType == TypeNickLabel.Turret && playerScript != null)
		{
			if (Defs.isMulti)
			{
				nickLabel.text = FilterBadWorld.FilterString(playerScript.mySkinName.NickName);
			}
			else
			{
				nickLabel.text = FilterBadWorld.FilterString(ProfileController.GetPlayerNameOrDefault());
			}
			if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
			{
				SetCommandColor((!(WeaponManager.sharedManager.myPlayerMoveC == null)) ? ((WeaponManager.sharedManager.myPlayerMoveC.myCommand == playerScript.myCommand) ? 1 : 2) : 0);
			}
			else
			{
				SetCommandColor((!playerScript.Equals(WeaponManager.sharedManager.myPlayerMoveC) && !Defs.isDaterRegim && !Defs.isCOOP) ? 2 : 0);
			}
		}
		if (currentType != TypeNickLabel.PlayerLobby)
		{
			return;
		}
		nickLabel.text = myLobbyNickname;
		clanName.text = FriendsController.sharedController.clanName;
		if (!string.IsNullOrEmpty(FriendsController.sharedController.clanLogo))
		{
			if (clanTexture.mainTexture == null)
			{
				byte[] data = Convert.FromBase64String(FriendsController.sharedController.clanLogo);
				Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoWidth);
				texture2D.LoadImage(data);
				texture2D.filterMode = FilterMode.Point;
				texture2D.Apply();
				clanTexture.mainTexture = texture2D;
				Transform transform = clanTexture.transform;
				transform.localPosition = new Vector3((float)(-clanName.width) * 0.5f - 16f, transform.localPosition.y, transform.localPosition.z);
			}
		}
		else
		{
			clanTexture.mainTexture = null;
		}
		UpdateExpFrameInLobby();
	}

	public void UpdateNickInLobby()
	{
		myLobbyNickname = FilterBadWorld.FilterString(ProfileController.GetPlayerNameOrDefault());
	}

	private void UpdateExpFrameInLobby()
	{
		int currentLevel = ExperienceController.sharedController.currentLevel;
		ranksSpriteForLobby.spriteName = "Rank_" + currentLevel;
		int num = ExperienceController.MaxExpLevels[ExperienceController.sharedController.currentLevel];
		int num2 = Mathf.Clamp(ExperienceController.sharedController.CurrentExperience, 0, num);
		string text = string.Format("{0} {1}/{2}", LocalizationStore.Get("Key_0204"), num2, num);
		if (ExperienceController.sharedController.currentLevel == 31)
		{
			text = LocalizationStore.Get("Key_0928");
		}
		expLabel.text = text;
		expProgressSprite.width = Mathf.RoundToInt(146f * ((ExperienceController.sharedController.currentLevel != 31) ? ((float)num2 / (float)num) : 1f));
	}

	public void SetCommandColor(int _command = 0)
	{
		switch (_command)
		{
		case 1:
			nickLabel.color = Color.blue;
			break;
		case 2:
			nickLabel.color = Color.red;
			break;
		default:
			nickLabel.color = Color.white;
			break;
		}
	}

	public void ResetTimeShow(float _time = 0.1f)
	{
		timeShow = _time;
	}

	private void StopShow()
	{
		currentType = TypeNickLabel.None;
		HideLabel();
		base.gameObject.SetActive(false);
		playerScript = null;
		pointScript = null;
		turretScript = null;
	}

	private void CheckShow()
	{
		if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
		{
			ResetTimeShow(-1f);
		}
		else if (Defs.isDaterRegim)
		{
			ResetTimeShow(0.1f);
		}
		else if (currentType == TypeNickLabel.Point)
		{
			if (pointScript != null && pointScript.isBaseActive)
			{
				ResetTimeShow(0.1f);
			}
			else
			{
				ResetTimeShow(-1f);
			}
		}
		else if (currentType == TypeNickLabel.PlayerLobby || currentType == TypeNickLabel.FreeCoins || currentType == TypeNickLabel.GetGift || currentType == TypeNickLabel.Nest || currentType == TypeNickLabel.InAppBonus || currentType == TypeNickLabel.Leprechaunt)
		{
			if (AskNameManager.isComplete)
			{
				ResetTimeShow(1f);
			}
		}
		else if ((Defs.isHunger && HungerGameController.Instance != null && !HungerGameController.Instance.isGo) || WeaponManager.sharedManager.myPlayer == null)
		{
			ResetTimeShow(0.1f);
		}
		else if (WeaponManager.sharedManager.myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC.isKilled)
		{
			ResetTimeShow(0.1f);
		}
		else if ((ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints) && WeaponManager.sharedManager.myPlayer != null && WeaponManager.sharedManager.myPlayerMoveC != null && playerScript != null && WeaponManager.sharedManager.myPlayerMoveC.myCommand == playerScript.myCommand)
		{
			ResetTimeShow(0.1f);
		}
	}

	private void UpdateTurrethealthSprite()
	{
		float num = Mathf.RoundToInt((float)maxHeathWidth * (((!(turretScript.health < 0f)) ? turretScript.health : 0f) / turretScript.maxHealth));
		if (num < 0.1f)
		{
			num = 0f;
			healthSprite.enabled = false;
		}
		else if (!healthSprite.enabled)
		{
			healthSprite.enabled = true;
		}
		healthSprite.width = Mathf.RoundToInt(num);
	}

	public void LateUpdate()
	{
		if (target == null)
		{
			StopShow();
			return;
		}
		CheckShow();
		if (timeShow > 0f)
		{
			timeShow -= Time.deltaTime;
		}
		if (timeShow > 0f && target.position.y > -1000f && currentCamera != null)
		{
			posLabel = currentCamera.WorldToViewportPoint(target.position + offset + ((currentType != TypeNickLabel.Player || !(playerScript != null) || !playerScript.isMechActive) ? Vector3.zero : offsetMech));
			if (posLabel.z >= 0f)
			{
				if (currentType == TypeNickLabel.Turret)
				{
					UpdateTurrethealthSprite();
				}
				if (currentType == TypeNickLabel.PlayerLobby)
				{
					UpdateInfo();
				}
				if (currentType == TypeNickLabel.Player || currentType == TypeNickLabel.Turret)
				{
					if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
					{
						if (WeaponManager.sharedManager.myPlayerMoveC == null)
						{
							SetCommandColor(0);
						}
						else
						{
							SetCommandColor((WeaponManager.sharedManager.myPlayerMoveC.myCommand == playerScript.myCommand) ? 1 : 2);
						}
					}
					else
					{
						SetCommandColor((!Defs.isDaterRegim && !Defs.isCOOP) ? 2 : 0);
					}
				}
				thisTransform.localPosition = new Vector3((posLabel.x - 0.5f) * coefScreen.x, (posLabel.y - 0.5f) * coefScreen.y, 0f);
				isHideLabel = false;
			}
			else
			{
				HideLabel();
			}
		}
		else
		{
			HideLabel();
		}
	}

	private void HideLabel()
	{
		if (!isHideLabel)
		{
			isHideLabel = true;
			thisTransform.localPosition = new Vector3(0f, -10000f, 0f);
		}
	}
}
