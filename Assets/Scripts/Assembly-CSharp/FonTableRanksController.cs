using UnityEngine;

public class FonTableRanksController : MonoBehaviour
{
	public bool isTeamTable;

	public GameObject scoreHead;

	public GameObject countKillsHead;

	public GameObject likeHead;

	public int command;

	public string nameCommand;

	public UISprite fon;

	public UISprite fonHead;

	public UISprite fonUndrhead;

	public UILabel headLabel;

	public UILabel[] undrheadLabels;

	private void Start()
	{
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
		{
			float x = countKillsHead.transform.position.x;
			countKillsHead.transform.position = new Vector3(scoreHead.transform.position.x, countKillsHead.transform.position.y, countKillsHead.transform.position.z);
			scoreHead.transform.position = new Vector3(x, scoreHead.transform.position.y, scoreHead.transform.position.z);
			countKillsHead.GetComponent<UILabel>().text = LocalizationStore.Get("Key_1006");
			if (likeHead != null)
			{
				likeHead.SetActive(false);
			}
		}
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle)
		{
			scoreHead.transform.position = new Vector3(countKillsHead.transform.position.x, scoreHead.transform.position.y, scoreHead.transform.position.z);
			countKillsHead.SetActive(false);
			if (likeHead != null)
			{
				likeHead.SetActive(false);
			}
		}
		if (Defs.isDaterRegim)
		{
			scoreHead.gameObject.SetActive(false);
			countKillsHead.gameObject.SetActive(false);
			if (likeHead != null)
			{
				likeHead.SetActive(true);
			}
		}
		else if (likeHead != null)
		{
			likeHead.SetActive(false);
		}
	}

	public void SetCommand(int _command)
	{
		if (!isTeamTable)
		{
			return;
		}
		if (_command == 0)
		{
			fon.spriteName = "GreyTableHead";
			fonHead.spriteName = "GreyTableHead";
			fonUndrhead.spriteName = "GreyTable";
			UILabel[] array = undrheadLabels;
			foreach (UILabel uILabel in array)
			{
				uILabel.color = new Color(0.6f, 0.6f, 0.6f, 1f);
			}
			headLabel.text = LocalizationStore.Get(nameCommand);
		}
		if (_command == 1)
		{
			fon.spriteName = "BlueTeamTableHead";
			fonHead.spriteName = "BlueTeamTableHead";
			fonUndrhead.spriteName = "BlueTeamTable";
			UILabel[] array2 = undrheadLabels;
			foreach (UILabel uILabel2 in array2)
			{
				uILabel2.color = new Color(0.6f, 0.8f, 1f, 1f);
			}
			headLabel.text = LocalizationStore.Get("Key_1771");
		}
		if (_command == 2)
		{
			fon.spriteName = "RedTeamTableHead";
			fonHead.spriteName = "RedTeamTableHead";
			fonUndrhead.spriteName = "RedTeamTable";
			headLabel.text = LocalizationStore.Get("Key_1772");
			UILabel[] array3 = undrheadLabels;
			foreach (UILabel uILabel3 in array3)
			{
				uILabel3.color = new Color(1f, 0.7f, 0.7f, 1f);
			}
		}
	}

	private void Update()
	{
		int num = ((WeaponManager.sharedManager.myNetworkStartTable.myCommand > 0) ? WeaponManager.sharedManager.myNetworkStartTable.myCommand : WeaponManager.sharedManager.myNetworkStartTable.myCommandOld);
		if (NetworkStartTable.LocalOrPasswordRoom() && NetworkStartTableNGUIController.IsStartInterfaceShown())
		{
			num = 0;
		}
		SetCommand((num > 0) ? command : 0);
	}
}
