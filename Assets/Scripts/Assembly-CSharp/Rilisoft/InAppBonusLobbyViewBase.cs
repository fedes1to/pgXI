using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	public class InAppBonusLobbyViewBase : MonoBehaviour
	{
		[ReadOnly]
		[Header("[ debug ]")]
		[SerializeField]
		protected BonusData _data = new BonusData();

		protected BonusData _prevData = new BonusData();

		public BonusData Data
		{
			get
			{
				return _data;
			}
		}

		private void Awake()
		{
			LocalizationStore.AddEventCallAfterLocalize(OnLocalizationChanged);
		}

		private void OnDestroy()
		{
			LocalizationStore.DelEventCallAfterLocalize(OnLocalizationChanged);
		}

		private void OnLocalizationChanged()
		{
			UpdateView(true);
		}

		public void SetData(Dictionary<string, object> bonusData)
		{
			RiliExtensions.Swap(ref _data, ref _prevData);
			_data.Set(bonusData);
			if (base.gameObject.name != _data.Action)
			{
				base.gameObject.name = _data.Action;
			}
			UpdateView(false);
		}

		public virtual void UpdateView(bool force = false)
		{
		}

		public void Interact()
		{
			if (_data == null)
			{
				return;
			}
			EventHandler handleBackFromBank = null;
			handleBackFromBank = delegate
			{
				if (BankController.Instance.InterfaceEnabledCoroutineLocked)
				{
					Debug.LogWarning("InterfaceEnabledCoroutineLocked");
				}
				else
				{
					BankController.Instance.BackRequested -= handleBackFromBank;
					BankController.Instance.InterfaceEnabled = false;
				}
			};
			BankController.Instance.BackRequested += handleBackFromBank;
			BankController.Instance.SetInterfaceEnabledWithDesiredCurrency(true, (!_data.IsGems) ? "Coins" : "GemsCurrency");
		}
	}
}
