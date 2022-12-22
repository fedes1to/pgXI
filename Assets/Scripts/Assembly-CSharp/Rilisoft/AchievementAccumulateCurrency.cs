using UnityEngine;

namespace Rilisoft
{
	public class AchievementAccumulateCurrency : Achievement
	{
		public AchievementAccumulateCurrency(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (base._data.Currency != "Coins" && base._data.Currency != "GemsCurrency")
			{
				Debug.LogErrorFormat("achievement '{0}' without value", base._data.Id);
			}
			else
			{
				OnCurrencyChanged();
				Storager.SubscribeToChanged(base._data.Currency, OnCurrencyChanged);
			}
		}

		private void OnCurrencyChanged()
		{
			int @int = Storager.getInt(base._data.Currency, false);
			if (@int > base.Progress.Points)
			{
				Gain(@int - base.Progress.Points);
			}
		}

		public override void Dispose()
		{
			Storager.UnSubscribeToChanged(base._data.Currency, OnCurrencyChanged);
		}
	}
}
