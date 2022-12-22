public class AdminSettingsController
{
	public struct Avard
	{
		public int coin;

		public int expierense;
	}

	public static int minScoreDeathMath = 50;

	public static int[][] coinAvardDeathMath = new int[3][]
	{
		new int[10] { 2, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
		new int[10] { 4, 2, 1, 0, 0, 0, 0, 0, 0, 0 },
		new int[10] { 6, 4, 2, 0, 0, 0, 0, 0, 0, 0 }
	};

	public static int[][] expAvardDeathMath = new int[3][]
	{
		new int[10] { 10, 8, 5, 3, 2, 1, 0, 0, 0, 0 },
		new int[10] { 20, 10, 6, 4, 3, 2, 0, 0, 0, 0 },
		new int[10] { 30, 15, 10, 6, 4, 2, 0, 0, 0, 0 }
	};

	public static int minScoreTeamFight = 50;

	public static int[][] coinAvardTeamFight = new int[3][]
	{
		new int[10] { 2, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
		new int[10] { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 },
		new int[10] { 6, 4, 3, 0, 0, 0, 0, 0, 0, 0 }
	};

	public static int[][] expAvardTeamFight = new int[3][]
	{
		new int[10] { 10, 8, 5, 3, 0, 0, 0, 0, 0, 0 },
		new int[10] { 20, 10, 6, 4, 0, 0, 0, 0, 0, 0 },
		new int[10] { 30, 15, 10, 6, 0, 0, 0, 0, 0, 0 }
	};

	public static int minScoreFlagCapture = 50;

	public static int[][] coinAvardFlagCapture = new int[3][]
	{
		new int[10] { 2, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
		new int[10] { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 },
		new int[10] { 6, 4, 3, 0, 0, 0, 0, 0, 0, 0 }
	};

	public static int[][] expAvardFlagCapture = new int[3][]
	{
		new int[10] { 10, 8, 5, 3, 0, 0, 0, 0, 0, 0 },
		new int[10] { 20, 10, 6, 4, 0, 0, 0, 0, 0, 0 },
		new int[10] { 30, 15, 10, 6, 0, 0, 0, 0, 0, 0 }
	};

	public static int minScoreTimeBattle = 2000;

	public static int[] coinAvardTimeBattle = new int[10] { 3, 2, 1, 1, 1, 1, 1, 1, 1, 1 };

	public static int[] expAvardTimeBattle = new int[10] { 20, 15, 10, 5, 5, 5, 5, 5, 5, 5 };

	public static int[] coinAvardDeadlyGames = new int[8] { 0, 2, 3, 4, 5, 6, 8, 10 };

	public static int[] expAvardDeadlyGames = new int[8] { 0, 10, 10, 11, 12, 13, 14, 15 };

	public static int minScoreCapturePoint = 50;

	public static int[][] coinAvardCapturePoint = new int[3][]
	{
		new int[10] { 2, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
		new int[10] { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 },
		new int[10] { 6, 4, 3, 0, 0, 0, 0, 0, 0, 0 }
	};

	public static int[][] expAvardCapturePoint = new int[3][]
	{
		new int[10] { 10, 8, 5, 3, 0, 0, 0, 0, 0, 0 },
		new int[10] { 20, 10, 6, 4, 0, 0, 0, 0, 0, 0 },
		new int[10] { 30, 15, 10, 6, 0, 0, 0, 0, 0, 0 }
	};

	public static int minScoreDuel = 50;

	public static int[] coinAvardDuel = new int[2] { 2, 0 };

	public static int[] expAvardDuel = new int[2] { 10, 0 };

	public static void ResetAvardSettingsOnDefault()
	{
		minScoreDeathMath = 50;
		coinAvardDeathMath = new int[3][]
		{
			new int[10] { 2, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
			new int[10] { 4, 2, 1, 0, 0, 0, 0, 0, 0, 0 },
			new int[10] { 6, 4, 2, 0, 0, 0, 0, 0, 0, 0 }
		};
		expAvardDeathMath = new int[3][]
		{
			new int[10] { 10, 8, 5, 3, 2, 1, 0, 0, 0, 0 },
			new int[10] { 20, 10, 6, 4, 3, 2, 0, 0, 0, 0 },
			new int[10] { 30, 15, 10, 6, 4, 2, 0, 0, 0, 0 }
		};
		minScoreTeamFight = 50;
		coinAvardTeamFight = new int[3][]
		{
			new int[10] { 2, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
			new int[10] { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 },
			new int[10] { 6, 4, 3, 0, 0, 0, 0, 0, 0, 0 }
		};
		expAvardTeamFight = new int[3][]
		{
			new int[10] { 10, 8, 5, 3, 0, 0, 0, 0, 0, 0 },
			new int[10] { 20, 10, 6, 4, 0, 0, 0, 0, 0, 0 },
			new int[10] { 30, 15, 10, 6, 0, 0, 0, 0, 0, 0 }
		};
		minScoreFlagCapture = 50;
		coinAvardFlagCapture = new int[3][]
		{
			new int[10] { 2, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
			new int[10] { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 },
			new int[10] { 6, 4, 3, 0, 0, 0, 0, 0, 0, 0 }
		};
		expAvardFlagCapture = new int[3][]
		{
			new int[10] { 10, 8, 5, 3, 0, 0, 0, 0, 0, 0 },
			new int[10] { 20, 10, 6, 4, 0, 0, 0, 0, 0, 0 },
			new int[10] { 30, 15, 10, 6, 0, 0, 0, 0, 0, 0 }
		};
		minScoreTimeBattle = 2000;
		coinAvardTimeBattle = new int[10] { 3, 2, 1, 1, 1, 1, 1, 1, 1, 1 };
		expAvardTimeBattle = new int[10] { 20, 15, 10, 5, 5, 5, 5, 5, 5, 5 };
		coinAvardDeadlyGames = new int[8] { 0, 2, 3, 4, 5, 6, 8, 10 };
		expAvardDeadlyGames = new int[8] { 0, 10, 10, 11, 12, 13, 14, 15 };
		minScoreCapturePoint = 50;
		coinAvardCapturePoint = new int[3][]
		{
			new int[10] { 2, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
			new int[10] { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 },
			new int[10] { 6, 4, 3, 0, 0, 0, 0, 0, 0, 0 }
		};
		expAvardCapturePoint = new int[3][]
		{
			new int[10] { 10, 8, 5, 3, 0, 0, 0, 0, 0, 0 },
			new int[10] { 20, 10, 6, 4, 0, 0, 0, 0, 0, 0 },
			new int[10] { 30, 15, 10, 6, 0, 0, 0, 0, 0, 0 }
		};
	}

	public static Avard GetAvardAfterMatch(ConnectSceneNGUIController.RegimGame regim, int timeGame, int place, int score, int countKills, bool isWin)
	{
		Avard result = default(Avard);
		result.coin = 0;
		result.expierense = 0;
		bool flag;
		switch (regim)
		{
		case ConnectSceneNGUIController.RegimGame.Deathmatch:
			if (score < minScoreDeathMath)
			{
				return result;
			}
			switch (timeGame)
			{
			case 4:
				result.coin = coinAvardDeathMath[0][place];
				result.expierense = expAvardDeathMath[0][place];
				break;
			case 5:
				result.coin = coinAvardDeathMath[1][place];
				result.expierense = expAvardDeathMath[1][place];
				break;
			case 7:
				result.coin = coinAvardDeathMath[2][place];
				result.expierense = expAvardDeathMath[2][place];
				break;
			default:
				result.coin = coinAvardDeathMath[0][place];
				result.expierense = expAvardDeathMath[0][place];
				break;
			}
			result.coin *= GetMultiplyerRewardWithBoostEvent(true);
			result.expierense *= GetMultiplyerRewardWithBoostEvent(false);
			return result;
		case ConnectSceneNGUIController.RegimGame.TeamFight:
			if (Defs.abTestBalansCohort == Defs.ABTestCohortsType.B && !isWin)
			{
				place += 5;
			}
			flag = ExperienceController.sharedController.currentLevel < 2;
			if (ABTestController.useBuffSystem)
			{
				bool num;
				if (flag)
				{
					if (!BuffSystem.instance.haveFirstInteractons)
					{
						goto IL_016c;
					}
					num = score < 5;
				}
				else
				{
					num = score < minScoreTeamFight;
				}
				if (num)
				{
					goto IL_016c;
				}
			}
			else if (score < ((!flag) ? minScoreTeamFight : 5))
			{
				return result;
			}
			switch (timeGame)
			{
			case 4:
				result.coin = coinAvardTeamFight[0][place];
				result.expierense = expAvardTeamFight[0][(!flag) ? place : 0];
				break;
			case 5:
				result.coin = coinAvardTeamFight[1][place];
				result.expierense = expAvardTeamFight[1][(!flag) ? place : 0];
				break;
			case 7:
				result.coin = coinAvardTeamFight[2][place];
				result.expierense = expAvardTeamFight[2][(!flag) ? place : 0];
				break;
			default:
				result.coin = coinAvardTeamFight[0][place];
				result.expierense = expAvardTeamFight[0][(!flag) ? place : 0];
				break;
			}
			result.coin *= GetMultiplyerRewardWithBoostEvent(true);
			result.expierense *= GetMultiplyerRewardWithBoostEvent(false);
			return result;
		case ConnectSceneNGUIController.RegimGame.FlagCapture:
			if (Defs.abTestBalansCohort == Defs.ABTestCohortsType.B && !isWin)
			{
				place += 5;
			}
			if (score < minScoreFlagCapture)
			{
				return result;
			}
			switch (timeGame)
			{
			case 4:
				result.coin = coinAvardFlagCapture[0][place];
				result.expierense = expAvardFlagCapture[0][place];
				break;
			case 5:
				result.coin = coinAvardFlagCapture[1][place];
				result.expierense = expAvardFlagCapture[1][place];
				break;
			case 7:
				result.coin = coinAvardFlagCapture[2][place];
				result.expierense = expAvardFlagCapture[2][place];
				break;
			default:
				result.coin = coinAvardFlagCapture[0][place];
				result.expierense = expAvardFlagCapture[0][place];
				break;
			}
			result.coin *= GetMultiplyerRewardWithBoostEvent(true);
			result.expierense *= GetMultiplyerRewardWithBoostEvent(false);
			return result;
		case ConnectSceneNGUIController.RegimGame.TimeBattle:
			if (score < minScoreTimeBattle)
			{
				return result;
			}
			result.coin = coinAvardTimeBattle[place] * PremiumAccountController.Instance.RewardCoeff;
			result.expierense = expAvardTimeBattle[place] * PremiumAccountController.Instance.RewardCoeff;
			return result;
		case ConnectSceneNGUIController.RegimGame.DeadlyGames:
			if (!isWin || countKills < 0)
			{
				return result;
			}
			result.coin = coinAvardDeadlyGames[countKills] * PremiumAccountController.Instance.RewardCoeff;
			result.expierense = expAvardDeadlyGames[countKills] * PremiumAccountController.Instance.RewardCoeff;
			return result;
		case ConnectSceneNGUIController.RegimGame.CapturePoints:
			if (Defs.abTestBalansCohort == Defs.ABTestCohortsType.B && !isWin)
			{
				place += 5;
			}
			if (score < minScoreCapturePoint)
			{
				return result;
			}
			switch (timeGame)
			{
			case 4:
				result.coin = coinAvardCapturePoint[0][place];
				result.expierense = expAvardCapturePoint[0][place];
				break;
			case 5:
				result.coin = coinAvardCapturePoint[1][place];
				result.expierense = expAvardCapturePoint[1][place];
				break;
			case 7:
				result.coin = coinAvardCapturePoint[2][place];
				result.expierense = expAvardCapturePoint[2][place];
				break;
			default:
				result.coin = coinAvardCapturePoint[0][place];
				result.expierense = expAvardCapturePoint[0][place];
				break;
			}
			result.coin *= GetMultiplyerRewardWithBoostEvent(true);
			result.expierense *= GetMultiplyerRewardWithBoostEvent(false);
			return result;
		case ConnectSceneNGUIController.RegimGame.Duel:
			if (score < minScoreDeathMath)
			{
				return result;
			}
			result.coin = coinAvardDuel[place];
			result.expierense = expAvardDuel[place];
			result.coin *= GetMultiplyerRewardWithBoostEvent(true);
			result.expierense *= GetMultiplyerRewardWithBoostEvent(false);
			return result;
		default:
			{
				return result;
			}
			IL_016c:
			if (flag && score >= 5)
			{
				result.expierense = 3;
			}
			return result;
		}
	}

	public static int GetMultiplyerRewardWithBoostEvent(bool isMoney)
	{
		int num = 1;
		PromoActionsManager sharedManager = PromoActionsManager.sharedManager;
		PremiumAccountController instance = PremiumAccountController.Instance;
		int num2 = ((!isMoney) ? sharedManager.DayOfValorMultiplyerForExp : sharedManager.DayOfValorMultiplyerForMoney);
		if (sharedManager.IsDayOfValorEventActive && instance.IsActiveOrWasActiveBeforeStartMatch())
		{
			num = num2 + instance.GetRewardCoeffByActiveOrActiveBeforeMatch();
		}
		else if (sharedManager.IsDayOfValorEventActive)
		{
			num *= num2;
		}
		else if (instance.IsActiveOrWasActiveBeforeStartMatch())
		{
			num *= instance.GetRewardCoeffByActiveOrActiveBeforeMatch();
		}
		return num;
	}
}
