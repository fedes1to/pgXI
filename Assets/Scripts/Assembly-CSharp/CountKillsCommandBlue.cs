using UnityEngine;

public class CountKillsCommandBlue : MonoBehaviour
{
	public static float localScaleForLabels = 1.25f;

	private UILabel _label;

	public bool isEnemyCommandLabel;

	private WeaponManager _weaponManager;

	public GameObject myBackground;

	private Color goldColor = new Color(1f, 1f, 0f);

	private void Start()
	{
		_weaponManager = WeaponManager.sharedManager;
		InGameGUI sharedInGameGUI = InGameGUI.sharedInGameGUI;
		_label = GetComponent<UILabel>();
	}

	private void Update()
	{
		if (!_weaponManager || !_weaponManager.myPlayer)
		{
			return;
		}
		string text = "0";
		bool flag = false;
		if (Defs.isFlag)
		{
			if (WeaponManager.sharedManager.myTable != null)
			{
				if (isEnemyCommandLabel == (WeaponManager.sharedManager.myNetworkStartTable.myCommand == 2))
				{
					text = WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag1.ToString();
					flag = WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag1 > WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag2;
				}
				else
				{
					text = WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag2.ToString();
					flag = WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag2 > WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag1;
				}
			}
		}
		else if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints)
		{
			if (isEnemyCommandLabel == (WeaponManager.sharedManager.myNetworkStartTable.myCommand == 2))
			{
				text = Mathf.RoundToInt(CapturePointController.sharedController.scoreBlue).ToString();
				flag = CapturePointController.sharedController.scoreBlue > CapturePointController.sharedController.scoreRed;
			}
			else
			{
				text = Mathf.RoundToInt(CapturePointController.sharedController.scoreRed).ToString();
				flag = CapturePointController.sharedController.scoreRed > CapturePointController.sharedController.scoreBlue;
			}
		}
		else if (isEnemyCommandLabel == (WeaponManager.sharedManager.myNetworkStartTable.myCommand == 2))
		{
			text = _weaponManager.myPlayerMoveC.countKillsCommandBlue.ToString();
			flag = WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandBlue > WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandRed;
		}
		else
		{
			text = _weaponManager.myPlayerMoveC.countKillsCommandRed.ToString();
			flag = WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandRed > WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandBlue;
		}
		_label.text = text;
		_label.color = ((!flag) ? Color.white : goldColor);
	}
}
