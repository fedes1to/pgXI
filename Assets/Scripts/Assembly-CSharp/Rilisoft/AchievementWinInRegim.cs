using UnityEngine;

namespace Rilisoft
{
	public class AchievementWinInRegim : Achievement
	{
		public AchievementWinInRegim(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.Win += delegate(object sender, WinEventArgs e)
			{
				if (!base._data.RegimGame.HasValue)
				{
					Debug.LogErrorFormat("achievement '{0}' without value", base._data.Id);
				}
				else if (e.Mode == base._data.RegimGame.Value)
				{
					Gain(1);
				}
			};
		}
	}
}
