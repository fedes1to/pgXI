using UnityEngine;

public sealed class ActionInTableButton : MonoBehaviour
{
	public UIButton buttonScript;

	public UISprite backgroundSprite;

	public UISprite rankSprite;

	public GameObject check;

	public UILabel namesPlayers;

	public Vector3 defaultNameLabelPos;

	public UILabel scorePlayers;

	public UILabel countKillsPlayers;

	public UITexture clanTexture;

	public string pixelbookID;

	public string nickPlayer;

	public NetworkStartTableNGUIController networkStartTableNGUIController;

	public bool isMine;

	public GameObject isMineSprite;

	private void Start()
	{
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
		{
			float x = countKillsPlayers.transform.position.x;
			countKillsPlayers.transform.position = new Vector3(scorePlayers.transform.position.x, countKillsPlayers.transform.position.y, countKillsPlayers.transform.position.z);
			scorePlayers.transform.position = new Vector3(x, scorePlayers.transform.position.y, scorePlayers.transform.position.z);
		}
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle)
		{
			scorePlayers.transform.position = new Vector3(countKillsPlayers.transform.position.x, scorePlayers.transform.position.y, scorePlayers.transform.position.z);
			countKillsPlayers.gameObject.SetActive(false);
		}
		if (Defs.isDaterRegim)
		{
			countKillsPlayers.gameObject.SetActive(true);
			scorePlayers.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		UpdateAddState();
	}

	public void UpdateAddState()
	{
		if (string.IsNullOrEmpty(pixelbookID) || !FriendsController.sharedController.IsShowAdd(pixelbookID))
		{
			if (string.IsNullOrEmpty(pixelbookID) || pixelbookID.Equals("0") || pixelbookID.Equals("-1") || pixelbookID.Equals(FriendsController.sharedController.id) || !Defs2.IsAvalibleAddFrends() || string.IsNullOrEmpty(FriendsController.sharedController.id))
			{
				if (buttonScript.enabled)
				{
					buttonScript.enabled = false;
					buttonScript.tweenTarget.SetActive(false);
					check.SetActive(false);
				}
				if (check.activeSelf)
				{
					check.SetActive(false);
				}
			}
			else
			{
				if (buttonScript.enabled)
				{
					buttonScript.enabled = false;
					buttonScript.tweenTarget.SetActive(false);
				}
				if (!check.activeSelf)
				{
					check.SetActive(true);
				}
			}
		}
		else
		{
			if (!buttonScript.enabled)
			{
				buttonScript.enabled = true;
				buttonScript.tweenTarget.SetActive(true);
				check.SetActive(true);
			}
			if (!check.activeSelf)
			{
				check.SetActive(true);
			}
		}
	}

	public void UpdateState(bool isActive, int placeIndex = 0, bool _isMine = false, int command = 0, string nick = "", string score = "", string countKills = "", int rank = 1, Texture clanLogo = null, string _pixelbookID = "")
	{
		pixelbookID = _pixelbookID;
		nickPlayer = nick;
		isMine = _isMine;
		if (!isActive)
		{
			base.gameObject.SetActive(false);
			return;
		}
		Color color = Color.white;
		if (isMine)
		{
			isMineSprite.SetActive(true);
			if (buttonScript.enabled)
			{
				buttonScript.enabled = false;
				buttonScript.tweenTarget.SetActive(false);
				check.SetActive(false);
			}
			if (check.activeSelf)
			{
				check.SetActive(false);
			}
			color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			isMineSprite.SetActive(false);
			if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints)
			{
				switch (command)
				{
				case 0:
					color = new Color(0.6f, 0.6f, 0.6f, 1f);
					break;
				case 1:
					color = new Color(0.6f, 0.8f, 1f, 1f);
					break;
				default:
					color = new Color(1f, 0.7f, 0.7f, 1f);
					break;
				}
			}
		}
		base.gameObject.SetActive(true);
		namesPlayers.text = nick;
		if (defaultNameLabelPos == Vector3.zero)
		{
			defaultNameLabelPos = namesPlayers.transform.localPosition;
		}
		if (clanLogo == null)
		{
			namesPlayers.transform.localPosition = defaultNameLabelPos;
		}
		else
		{
			namesPlayers.transform.localPosition = defaultNameLabelPos + Vector3.right * 34f;
		}
		namesPlayers.color = color;
		scorePlayers.text = score;
		scorePlayers.color = color;
		countKillsPlayers.text = countKills;
		countKillsPlayers.color = color;
		rankSprite.spriteName = "Rank_" + rank;
		clanTexture.mainTexture = clanLogo;
	}

	private void OnClick()
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !LoadingInAfterGame.isShowLoading && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka && !isMine && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled))
		{
			if (ButtonClickSound.Instance != null)
			{
				ButtonClickSound.Instance.PlayClick();
			}
			networkStartTableNGUIController.ShowActionPanel(pixelbookID, nickPlayer);
		}
	}
}
