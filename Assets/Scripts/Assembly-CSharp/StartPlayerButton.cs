using UnityEngine;

public class StartPlayerButton : MonoBehaviour
{
	public enum TypeButton
	{
		Start,
		Team1,
		Team2,
		RandomBtn,
		TeamBattle
	}

	public TypeButton command;

	public BlueRedButtonController buttonController;

	private float timeEnable;

	private void Awake()
	{
		bool flag = ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints;
		bool flag2 = NetworkStartTable.LocalOrPasswordRoom();
		if ((!flag && command != 0) || (flag && (command == TypeButton.Start || ((command == TypeButton.RandomBtn || command == TypeButton.Team2 || command == TypeButton.Team1) && !flag2) || (command == TypeButton.TeamBattle && flag2))) || Defs.isHunger)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		if ((command == TypeButton.Start || command == TypeButton.TeamBattle) && Defs.isRegimVidosDebug)
		{
			base.gameObject.SetActive(false);
			GetComponent<UIButton>().enabled = false;
		}
	}

	private void OnEnable()
	{
		timeEnable = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		bool flag = Initializer.players.Count == 0 || Defs.isDaterRegim || Defs.isHunger || TimeGameController.sharedController == null || TimeGameController.sharedController.timerToEndMatch <= 0.0 || TimeGameController.sharedController.timerToEndMatch > 16.0;
		GetComponent<UIButton>().isEnabled = flag && (!Defs.isFlag || Initializer.flag1 != null);
	}

	private void OnClick()
	{
		if (Time.time - NotificationController.timeStartApp < 3f || (Defs.isCapturePoints && Time.realtimeSinceStartup - timeEnable < 1.5f) || (BankController.Instance != null && BankController.Instance.InterfaceEnabled) || LoadingInAfterGame.isShowLoading || (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown) || ShopNGUIController.GuiActive || ExperienceController.sharedController.isShowNextPlashka)
		{
			return;
		}
		ButtonClickSound.Instance.PlayClick();
		if (!(WeaponManager.sharedManager.myTable != null))
		{
			return;
		}
		int num = WeaponManager.sharedManager.myNetworkStartTable.myCommand;
		if (num <= 0)
		{
			num = (int)command;
			if ((command == TypeButton.RandomBtn || command == TypeButton.TeamBattle) && buttonController != null)
			{
				num = ((buttonController.countRed < buttonController.countBlue) ? 2 : ((buttonController.countRed > buttonController.countBlue) ? 1 : Random.Range(1, 3)));
			}
		}
		WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().StartPlayerButtonClick(num);
	}
}
