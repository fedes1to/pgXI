using UnityEngine;

namespace Rilisoft
{
	public class AchievementGetCurrency : Achievement
	{
		private int _prevValue = -1;

		public AchievementGetCurrency(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (base._data.Currency.IsNullOrEmpty() || (base._data.Currency != "Coins" && base._data.Currency != "GemsCurrency"))
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
			if (_prevValue == -1)
			{
				_prevValue = @int;
			}
			int num = @int - _prevValue;
			if (num > 0)
			{
				Gain(num);
			}
			_prevValue = @int;
		}

		public override void Dispose()
		{
			Storager.UnSubscribeToChanged(base._data.Currency, OnCurrencyChanged);
		}
	}
}
