using System.Collections.Generic;
using UnityEngine;

public class RanksTable : MonoBehaviour
{
	private const int maxCountInCommandPlusOther = 6;

	private const int maxCountInTeam = 5;

	public GameObject panelRanks;

	public GameObject panelRanksTeam;

	public GameObject tekPanel;

	public GameObject modePC1;

	public GameObject modeFC1;

	public GameObject modeTDM1;

	public ActionInTableButton[] playersButtonsDeathmatch;

	public ActionInTableButton[] playersButtonsTeamFight;

	private List<NetworkStartTable> tabs = new List<NetworkStartTable>();

	private List<NetworkStartTable> tabsBlue = new List<NetworkStartTable>();

	private List<NetworkStartTable> tabsRed = new List<NetworkStartTable>();

	private List<NetworkStartTable> tabsWhite = new List<NetworkStartTable>();

	public bool isShowRanks;

	public bool isShowTableStart;

	public bool isShowTableWin;

	private bool isTeamMode;

	private string othersStr = "Others";

	public int totalBlue;

	public int totalRed;

	public int sumBlue;

	public int sumRed;

	private void Awake()
	{
		othersStr = LocalizationStore.Get("Key_1224");
		isTeamMode = ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints;
	}

	private void Start()
	{
		if (isTeamMode)
		{
			panelRanksTeam.SetActive(true);
			panelRanks.SetActive(false);
			modePC1.SetActive(ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints);
			modeFC1.SetActive(ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture);
			modeTDM1.SetActive(ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight);
		}
		else
		{
			panelRanksTeam.SetActive(false);
			panelRanks.SetActive(true);
		}
	}

	private void Update()
	{
		if (isShowRanks || isShowTableStart)
		{
			ReloadTabsFromReal();
			UpdateRanksFromTabs();
		}
	}

	private void ReloadTabsFromReal()
	{
		tabsBlue.Clear();
		tabsRed.Clear();
		tabsWhite.Clear();
		tabs.Clear();
		tabs.AddRange(Initializer.networkTables);
		for (int i = 1; i < tabs.Count; i++)
		{
			NetworkStartTable networkStartTable = tabs[i];
			for (int j = 0; j < i; j++)
			{
				NetworkStartTable networkStartTable2 = tabs[j];
				if ((!Defs.isDuel && !Defs.isFlag && !Defs.isCapturePoints && (networkStartTable.score > networkStartTable2.score || (networkStartTable.score == networkStartTable2.score && networkStartTable.CountKills > networkStartTable2.CountKills))) || ((Defs.isDuel || Defs.isFlag || Defs.isCapturePoints) && (networkStartTable.CountKills > networkStartTable2.CountKills || (networkStartTable.CountKills == networkStartTable2.CountKills && networkStartTable.score > networkStartTable2.score))))
				{
					NetworkStartTable value = tabs[i];
					for (int num = i - 1; num >= j; num--)
					{
						tabs[num + 1] = tabs[num];
					}
					tabs[j] = value;
					break;
				}
			}
		}
		if (!isTeamMode)
		{
			return;
		}
		for (int k = 0; k < tabs.Count; k++)
		{
			if (tabs[k].myCommand == 1)
			{
				tabsBlue.Add(tabs[k]);
			}
			else if (tabs[k].myCommand == 2)
			{
				tabsRed.Add(tabs[k]);
			}
			else
			{
				tabsWhite.Add(tabs[k]);
			}
		}
	}

	private void FillButtonFromOldState(ActionInTableButton button, int tableIndex, bool isBlueTable = true, int team = 0)
	{
		NetworkStartTable myNetworkStartTable = WeaponManager.sharedManager.myNetworkStartTable;
		bool flag = false;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		string empty4 = string.Empty;
		int num = 0;
		Texture texture = null;
		if (!isTeamMode)
		{
			flag = myNetworkStartTable.oldIndexMy == tableIndex;
			empty = myNetworkStartTable.oldCountLilsSpisok[tableIndex];
			empty2 = myNetworkStartTable.oldScoreSpisok[tableIndex];
			empty3 = myNetworkStartTable.oldSpisokPixelBookID[tableIndex].ToString();
			empty4 = myNetworkStartTable.oldSpisokName[tableIndex];
			num = myNetworkStartTable.oldSpisokRanks[tableIndex];
			texture = myNetworkStartTable.oldSpisokMyClanLogo[tableIndex];
		}
		else
		{
			flag = ((!isBlueTable) ? (myNetworkStartTable.oldIndexMy == tableIndex && myNetworkStartTable.myCommandOld == 2) : (myNetworkStartTable.oldIndexMy == tableIndex && myNetworkStartTable.myCommandOld == 1));
			empty = ((!isBlueTable) ? myNetworkStartTable.oldCountLilsSpisokRed[tableIndex] : myNetworkStartTable.oldCountLilsSpisokBlue[tableIndex]);
			empty2 = ((!isBlueTable) ? myNetworkStartTable.oldScoreSpisokRed[tableIndex] : myNetworkStartTable.oldScoreSpisokBlue[tableIndex]);
			empty3 = ((!isBlueTable) ? myNetworkStartTable.oldSpisokPixelBookIDRed[tableIndex] : myNetworkStartTable.oldSpisokPixelBookIDBlue[tableIndex].ToString());
			empty4 = ((!isBlueTable) ? myNetworkStartTable.oldSpisokNameRed[tableIndex] : myNetworkStartTable.oldSpisokNameBlue[tableIndex]);
			num = ((!isBlueTable) ? myNetworkStartTable.oldSpisokRanksRed[tableIndex] : myNetworkStartTable.oldSpisokRanksBlue[tableIndex]);
			texture = ((!isBlueTable) ? myNetworkStartTable.oldSpisokMyClanLogoRed[tableIndex] : myNetworkStartTable.oldSpisokMyClanLogoBlue[tableIndex]);
		}
		if (empty == "-1")
		{
			empty = "0";
		}
		if (empty2 == "-1")
		{
			empty2 = "0";
		}
		button.UpdateState(true, tableIndex, flag, team, empty4, empty2, empty, num, texture, empty3);
	}

	private void FillButtonFromTable(ActionInTableButton button, NetworkStartTable table, int tableIndex, int team = 0)
	{
		NetworkStartTable myNetworkStartTable = WeaponManager.sharedManager.myNetworkStartTable;
		bool flag = false;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		string empty4 = string.Empty;
		int num = 0;
		Texture texture = null;
		flag = table.Equals(myNetworkStartTable);
		empty = table.CountKills.ToString();
		empty2 = table.score.ToString();
		empty3 = table.pixelBookID.ToString();
		empty4 = table.NamePlayer;
		num = table.myRanks;
		texture = table.myClanTexture;
		if (empty == "-1")
		{
			empty = "0";
		}
		if (empty2 == "-1")
		{
			empty2 = "0";
		}
		button.UpdateState(true, tableIndex, flag, team, empty4, empty2, empty, num, texture, empty3);
	}

	private void FillDeathmatchButtons(bool oldState = false)
	{
		NetworkStartTable myNetworkStartTable = WeaponManager.sharedManager.myNetworkStartTable;
		for (int i = 0; i < playersButtonsDeathmatch.Length; i++)
		{
			if (!oldState && i < tabs.Count)
			{
				FillButtonFromTable(playersButtonsDeathmatch[i], tabs[i], i);
			}
			else if (oldState && i < myNetworkStartTable.oldSpisokName.Length)
			{
				FillButtonFromOldState(playersButtonsDeathmatch[i], i);
			}
			else
			{
				playersButtonsDeathmatch[i].UpdateState(false, 0, false, 0, string.Empty, string.Empty, string.Empty, 1, null, string.Empty);
			}
		}
	}

	private void FillTeamButtons(bool oldState = false)
	{
		NetworkStartTable myNetworkStartTable = WeaponManager.sharedManager.myNetworkStartTable;
		int num = Mathf.Max(0, (!oldState) ? myNetworkStartTable.myCommand : myNetworkStartTable.myCommandOld);
		sumRed = 0;
		sumBlue = 0;
		for (int i = 0; i < playersButtonsTeamFight.Length / 2; i++)
		{
			if (!oldState && i < Mathf.Min(tabsBlue.Count, 5))
			{
				sumBlue += ((tabsBlue[i].CountKills != -1) ? tabsBlue[i].CountKills : 0);
				FillButtonFromTable(playersButtonsTeamFight[i + ((num == 2) ? 6 : 0)], tabsBlue[i], i, num);
			}
			else if (oldState && i < Mathf.Min(myNetworkStartTable.oldSpisokNameBlue.Length, 5))
			{
				sumBlue += int.Parse((!(myNetworkStartTable.oldCountLilsSpisokBlue[i] != "-1")) ? "0" : myNetworkStartTable.oldCountLilsSpisokBlue[i]);
				FillButtonFromOldState(playersButtonsTeamFight[i + ((num == 2) ? 6 : 0)], i, true, num);
			}
			else if (totalBlue - sumBlue > 0 && i == 5 && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.CapturePoints)
			{
				playersButtonsTeamFight[i + ((num == 2) ? 6 : 0)].UpdateState(true, i, false, num, othersStr, string.Empty, (totalBlue - sumBlue).ToString(), -1, null, string.Empty);
			}
			else
			{
				playersButtonsTeamFight[i + ((num == 2) ? 6 : 0)].UpdateState(false, 0, false, 0, string.Empty, string.Empty, string.Empty, 1, null, string.Empty);
			}
			if (!oldState && i < Mathf.Min(tabsRed.Count, 5))
			{
				sumRed += ((tabsRed[i].CountKills != -1) ? tabsRed[i].CountKills : 0);
				ActionInTableButton button = playersButtonsTeamFight[i + ((num != 2) ? 6 : 0)];
				NetworkStartTable table = tabsRed[i];
				int tableIndex = i;
				int team;
				switch (num)
				{
				case 0:
					team = 0;
					break;
				case 2:
					team = 1;
					break;
				default:
					team = 2;
					break;
				}
				FillButtonFromTable(button, table, tableIndex, team);
			}
			else if (oldState && i < Mathf.Min(myNetworkStartTable.oldSpisokNameRed.Length, 5))
			{
				sumRed += int.Parse((!(myNetworkStartTable.oldCountLilsSpisokRed[i] != "-1")) ? "0" : myNetworkStartTable.oldCountLilsSpisokRed[i]);
				ActionInTableButton button2 = playersButtonsTeamFight[i + ((num != 2) ? 6 : 0)];
				int tableIndex2 = i;
				int team2;
				switch (num)
				{
				case 0:
					team2 = 0;
					break;
				case 2:
					team2 = 1;
					break;
				default:
					team2 = 2;
					break;
				}
				FillButtonFromOldState(button2, tableIndex2, false, team2);
			}
			else if (totalRed - sumRed > 0 && i == 5 && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.CapturePoints)
			{
				ActionInTableButton obj = playersButtonsTeamFight[i + ((num != 2) ? 6 : 0)];
				int placeIndex = i;
				int command;
				switch (num)
				{
				case 0:
					command = 0;
					break;
				case 2:
					command = 1;
					break;
				default:
					command = 2;
					break;
				}
				obj.UpdateState(true, placeIndex, false, command, othersStr, string.Empty, (totalRed - sumRed).ToString(), -1, null, string.Empty);
			}
			else
			{
				playersButtonsTeamFight[i + ((num != 2) ? 6 : 0)].UpdateState(false, 0, false, 0, string.Empty, string.Empty, string.Empty, 1, null, string.Empty);
			}
		}
		if (oldState && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.CapturePoints)
		{
			if (totalBlue < sumBlue)
			{
				totalBlue = sumBlue;
			}
			if (totalRed < sumRed)
			{
				totalRed = sumRed;
			}
		}
		for (int j = 0; j < NetworkStartTableNGUIController.sharedController.totalBlue.Length; j++)
		{
			NetworkStartTableNGUIController.sharedController.totalBlue[j].text = ((num == 2) ? totalRed.ToString() : totalBlue.ToString());
		}
		for (int k = 0; k < NetworkStartTableNGUIController.sharedController.totalRed.Length; k++)
		{
			NetworkStartTableNGUIController.sharedController.totalRed[k].text = ((num == 2) ? totalBlue.ToString() : totalRed.ToString());
		}
	}

	private void UpdateRanksFromTabs()
	{
		if (Defs.isCompany)
		{
			if (WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				totalBlue = WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandBlue;
				totalRed = WeaponManager.sharedManager.myPlayerMoveC.countKillsCommandRed;
			}
			else
			{
				totalBlue = GlobalGameController.countKillsBlue;
				totalRed = GlobalGameController.countKillsRed;
			}
		}
		if (Defs.isFlag && WeaponManager.sharedManager.myNetworkStartTable != null)
		{
			totalBlue = WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag1;
			totalRed = WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag2;
		}
		if (Defs.isCapturePoints)
		{
			totalBlue = Mathf.RoundToInt(CapturePointController.sharedController.scoreBlue);
			totalRed = Mathf.RoundToInt(CapturePointController.sharedController.scoreRed);
		}
		if (isTeamMode)
		{
			FillTeamButtons(false);
		}
		else
		{
			FillDeathmatchButtons(false);
		}
	}

	public void UpdateRanksFromOldSpisok()
	{
		if (isTeamMode)
		{
			FillTeamButtons(true);
		}
		else
		{
			FillDeathmatchButtons(true);
		}
	}
}
