using System.Collections.Generic;
using System.Linq;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	public class AchievementsManager : Singleton<AchievementsManager>
	{
		private const string KEY_PROGRESSES = "achievementsProgress";

		private readonly List<Achievement> _achievementsAll = new List<Achievement>();

		private readonly ObservableList<Achievement> _achievements = new ObservableList<Achievement>();

		public static Awaiter Awaiter = new Awaiter();

		public bool IsReady { get; private set; }

		public ObservableList<Achievement> AvailableAchiements
		{
			get
			{
				return _achievements;
			}
		}

		public static List<AchievementProgressData> ReadLocalProgress()
		{
			return AchievementProgressData.ParseAll(Storager.getString("achievementsProgress", false));
		}

		protected override void Awake()
		{
			base.Awake();
			_achievementsAll.Clear();
			TextAsset textAsset = Resources.Load<TextAsset>("Achievements/achievements_data");
			if (textAsset == null)
			{
				Debug.LogError("Achievements cfg not found");
				return;
			}
			if (Debug.isDebugBuild)
			{
			}
			IEnumerable<AchievementData> enumerable = AchievementData.ParseAllAsEnumerable(textAsset.text);
			if (Debug.isDebugBuild)
			{
			}
			List<AchievementProgressData> source = ReadLocalProgress();
			foreach (AchievementData data in enumerable)
			{
				AchievementProgressData achievementProgressData = source.FirstOrDefault((AchievementProgressData p) => p.AchievementId == data.Id);
				if (achievementProgressData == null)
				{
					achievementProgressData = new AchievementProgressData();
					achievementProgressData.AchievementId = data.Id;
				}
				if (Debug.isDebugBuild)
				{
				}
				Achievement ach = CreateAchievement(data, achievementProgressData);
				if (Debug.isDebugBuild)
				{
				}
				if (ach == null)
				{
					continue;
				}
				if (_achievementsAll.Contains(ach))
				{
					Debug.LogErrorFormat("achievement instancing error, DUPLICATE Id: '{0}'", ach.Id);
					continue;
				}
				_achievementsAll.Add(ach);
				ach.OnProgressChanged += delegate(bool p, bool s)
				{
					OnAchievementProgressChanged(ach, p, s);
				};
			}
			_achievements.Clear();
			IEnumerable<Achievement> achievementsToShow = GetAchievementsToShow();
			_achievements.AddRange(achievementsToShow);
			IsReady = true;
			Singleton<SceneLoader>.Instance.OnSceneLoading -= OnSceneLoading;
			Singleton<SceneLoader>.Instance.OnSceneLoading += OnSceneLoading;
		}

		private void Update()
		{
			Awaiter.Tick();
		}

		private Achievement CreateAchievement(AchievementData data, AchievementProgressData progress)
		{
			Achievement result = null;
			switch (data.ClassType)
			{
			case AchievementClassType.Unknown:
				Debug.LogError("detect AchievementClassType.Unknown");
				break;
			case AchievementClassType.KillMobs:
				result = new AchievementKillMobs(data, progress);
				break;
			case AchievementClassType.KillPlayers:
				result = new AchievementKillPlayers(data, progress);
				break;
			case AchievementClassType.KillPlayerThroughWeaponCategory:
				result = new AchievementKillPlayerThroughWeaponCategory(data, progress);
				break;
			case AchievementClassType.InflictHeadshot:
				result = new AchievementInflictHeadshot(data, progress);
				break;
			case AchievementClassType.Win:
				result = new AchievementWin(data, progress);
				break;
			case AchievementClassType.WinInRegim:
				result = new AchievementWinInRegim(data, progress);
				break;
			case AchievementClassType.Gacha:
				result = new AchievementGacha(data, progress);
				break;
			case AchievementClassType.GetCurrency:
				result = new AchievementGetCurrency(data, progress);
				break;
			case AchievementClassType.AccumulateCurrency:
				result = new AchievementAccumulateCurrency(data, progress);
				break;
			case AchievementClassType.JoinToClan:
				result = new AchievementJoinToClan(data, progress);
				break;
			case AchievementClassType.OpenLeague:
				result = new AchievementOpenLeague(data, progress);
				break;
			case AchievementClassType.CollectItem:
				result = new AchievementCollectItem(data, progress);
				break;
			case AchievementClassType.CollectCompagnSecret:
				result = new AchievementCollectCompagnSecret(data, progress);
				break;
			case AchievementClassType.Jump:
				result = new AchievementJump(data, progress);
				break;
			case AchievementClassType.KillInvisiblePlayer:
				result = new AchievementKillInvisiblePlayer(data, progress);
				break;
			case AchievementClassType.KillPlayerOfAllWeaponCategories:
				result = new AchievementKillPlayerOfAllWeaponCategories(data, progress);
				break;
			case AchievementClassType.TurretKill:
				result = new AchievementTurretKill(data, progress);
				break;
			case AchievementClassType.KillAtFly:
				result = new AchievementKillAtFly(data, progress);
				break;
			case AchievementClassType.KillPlayerWhenHpEqualsOne:
				result = new AchievementKillPlayerWhenHpEqualsOne(data, progress);
				break;
			case AchievementClassType.RemainArenaWaves:
				result = new AchievementRemainArenaWaves(data, progress);
				break;
			case AchievementClassType.JetPackFlying:
				result = new AchievementJetPackFlying(data, progress);
				break;
			case AchievementClassType.MechKillPlayers:
				result = new AchievementMechKillPlayers(data, progress);
				break;
			case AchievementClassType.Pacifist:
				result = new AchievementPacifist(data, progress);
				break;
			case AchievementClassType.Shooting:
				result = new AchievementShooting(data, progress);
				break;
			case AchievementClassType.ReturnAfterDays:
				result = new AchievementReturnAfterDays(data, progress);
				break;
			case AchievementClassType.CollectPets:
				result = new AchievementCollectPets(data, progress);
				break;
			case AchievementClassType.QuestsComplited:
				result = new AchievementQuestsComplited(data, progress);
				break;
			case AchievementClassType.ExistsGadgetsInAllCategories:
				result = new AchievementExistsGadgetsInAllCategories(data, progress);
				break;
			case AchievementClassType.Resurection:
				result = new AchievementResurection(data, progress);
				break;
			case AchievementClassType.EggsHatched:
				result = new AchievementEggsHatched(data, progress);
				break;
			case AchievementClassType.DemonKillMech:
				result = new AchievementDemonKillMech(data, progress);
				break;
			case AchievementClassType.GetItem:
				result = new AchievementGetItem(data, progress);
				break;
			default:
				Debug.LogErrorFormat("unsupported AchievementClassType : '{0}'", data.ClassType);
				break;
			}
			return result;
		}

		private void OnAchievementProgressChanged(Achievement ach, bool pointsChanged, bool stageChanged)
		{
			if (!_achievementsAll.Contains(ach))
			{
				Debug.LogErrorFormat("[Achievement] Unknown achievement: '{0}'", ach.Id);
			}
			else if (stageChanged)
			{
				if (!_achievements.Contains(ach))
				{
					_achievements.Add(ach);
				}
				string text = LocalizationStore.Get(ach.Data.LKeyName);
				Texture bgTexture = AchievementView.BackgroundTextureFor(ach);
				InfoWindowController.ShowAchievementsBox(text, bgTexture, ach.Data.Icon);
				AnalyticsStuff.LogAchievementEarned(ach.Id, ach.Stage);
			}
			else if (ach.Points > 0 && !_achievements.Contains(ach) && ach.Type != AchievementType.Hidden)
			{
				List<Achievement> list = GetAchievementsToShow().ToList();
				int num = list.IndexOf(ach);
				if (num > -1)
				{
					_achievements.Insert(num, ach);
				}
			}
		}

		private IEnumerable<Achievement> GetAchievementsToShow()
		{
			IEnumerable<Achievement> source = _achievementsAll.Where((Achievement a) => a.Type == AchievementType.Common || (a.Points > 0 && a.Type == AchievementType.Openable) || (a.Stage > 0 && a.Type == AchievementType.Hidden));
			return from a in source
				orderby a.Type, a.Data.GroupId, a.Id
				select a;
		}

		public void SetProgress(AchievementProgressData pData)
		{
			Achievement achievement = _achievementsAll.FirstOrDefault((Achievement a) => a.Id == pData.AchievementId);
			if (achievement != null)
			{
				achievement.ProgressData = pData;
			}
		}

		private void OnApplicationPause(bool b)
		{
			if (b)
			{
				SaveProgresses();
			}
		}

		private void OnSceneLoading(SceneLoadInfo li)
		{
			SaveProgresses();
		}

		public void SaveProgresses()
		{
			List<Dictionary<string, object>> obj = _achievementsAll.Select((Achievement a) => a.ProgressData.ObjectForSave()).ToList();
			string val = Json.Serialize(obj);
			Storager.setString("achievementsProgress", val, false);
		}
	}
}
