using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	public class AchievementOpenLeague : Achievement
	{
		public AchievementOpenLeague(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (!base._data.League.HasValue)
			{
				Debug.LogErrorFormat("achievement '{0}' without value", base._data.Id);
			}
			else
			{
				AchievementsManager.Awaiter.Register(WaitRatingSystem());
			}
		}

		private IEnumerator WaitRatingSystem()
		{
			while (RatingSystem.instance == null)
			{
				yield return null;
			}
			OnRatingUpdated();
			RatingSystem.OnRatingUpdate += OnRatingUpdated;
		}

		private void OnRatingUpdated()
		{
			if (base._data.League.Value == RatingSystem.instance.currentLeague && base.Progress.Points < base.PointsLeft)
			{
				Gain(1);
			}
		}

		public override void Dispose()
		{
			RatingSystem.OnRatingUpdate -= OnRatingUpdated;
			AchievementsManager.Awaiter.Remove(WaitRatingSystem());
		}
	}
}
