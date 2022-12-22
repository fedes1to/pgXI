using UnityEngine;

namespace Rilisoft
{
	public class AchievementGetItem : Achievement
	{
		private int _prevVal = -1;

		public AchievementGetItem(AchievementData data, AchievementProgressData progressData)
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
			if (@int < 1)
			{
				_prevVal = @int;
				return;
			}
			if (_prevVal < 0)
			{
				if (base.Points < @int)
				{
					Gain(@int - base.Points);
				}
			}
			else if (@int > _prevVal)
			{
				Gain(@int - _prevVal);
			}
			_prevVal = @int;
		}

		public override void Dispose()
		{
			Storager.UnSubscribeToChanged(base._data.ItemId, OnStoragerKeyChanged);
		}
	}
}
