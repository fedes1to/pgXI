using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	public class AchievementJetPackFlying : Achievement
	{
		private const string DATA_KEY = "ftime";

		public AchievementJetPackFlying(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			AchievementsManager.Awaiter.Register(Check());
		}

		private IEnumerator Check()
		{
			float _timeElapsed = 0f;
			while (true)
			{
				if (Defs.isJetpackEnabled && Defs.isJump)
				{
					_timeElapsed += Time.deltaTime;
				}
				else if (_timeElapsed > 0f)
				{
					float currentProgress2 = 0f;
					object rawProgress = base.Progress.CustomDataGet("ftime");
					if (rawProgress != null)
					{
						float.TryParse((string)rawProgress, out currentProgress2);
					}
					currentProgress2 += _timeElapsed;
					base.Progress.CustomDataSet("ftime", currentProgress2.ToString());
					int flyingMinutes = (int)(currentProgress2 / 60f);
					if (flyingMinutes != base.Progress.Points)
					{
						SetProgress(flyingMinutes);
					}
					_timeElapsed = 0f;
				}
				yield return null;
			}
		}

		public override void Dispose()
		{
			AchievementsManager.Awaiter.Remove(Check());
		}
	}
}
