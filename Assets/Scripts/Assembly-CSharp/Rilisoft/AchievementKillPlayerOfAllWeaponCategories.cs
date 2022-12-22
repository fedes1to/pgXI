using System.Linq;

namespace Rilisoft
{
	public class AchievementKillPlayerOfAllWeaponCategories : Achievement
	{
		private readonly string[] _allGameModes = new string[6] { "PrimaryCategory", "BackupCategory", "MeleeCategory", "SpecilCategory", "SniperCategory", "PremiumCategory" };

		public AchievementKillPlayerOfAllWeaponCategories(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (!base.IsCompleted)
			{
				QuestMediator.Events.KillOtherPlayer += QuestMediator_Events_KillOtherPlayer;
			}
		}

		private void QuestMediator_Events_KillOtherPlayer(object sender, KillOtherPlayerEventArgs e)
		{
			if (base.IsActive && !base.Progress.CustomDataExists(e.WeaponSlot.ToString()))
			{
				base.Progress.CustomDataSet(e.WeaponSlot.ToString());
				if (_allGameModes.All((string gm) => base.Progress.CustomDataExists(gm)))
				{
					Gain(1);
				}
			}
		}

		public override void Dispose()
		{
			QuestMediator.Events.KillOtherPlayer -= QuestMediator_Events_KillOtherPlayer;
		}
	}
}
