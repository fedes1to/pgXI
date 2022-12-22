using UnityEngine;

namespace Rilisoft
{
	public class AchievementCollectItem : Achievement
	{
		public AchievementCollectItem(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (base._data.ItemId.IsNullOrEmpty())
			{
				Debug.LogErrorFormat("achievement '{0}' without value", base._data.Id);
			}
			else
			{
				OnStoragerKeyChanged();
				Storager.SubscribeToChanged(base._data.ItemId, OnStoragerKeyChanged);
			}
		}

		private void OnStoragerKeyChanged()
		{
			int @int = Storager.getInt(base.Data.ItemId, false);
			if (base.Stage > 0)
			{
				int num = base._data.Thresholds[base.Stage - 1];
				if (@int >= num)
				{
					SetProgress(@int);
				}
			}
			else if (base.Points != @int)
			{
				SetProgress(@int);
			}
		}

		public override void Dispose()
		{
			Storager.UnSubscribeToChanged(base._data.ItemId, OnStoragerKeyChanged);
		}
	}
}
