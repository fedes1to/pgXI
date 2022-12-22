using UnityEngine;

namespace Rilisoft
{
	public class InAppBonusLobbyButtonView : InAppBonusLobbyViewBase
	{
		[SerializeField]
		[Header("[ base ]")]
		private UILabel _label;

		[SerializeField]
		[Header("[ images objects ]")]
		private GameObject _currencyObj;

		[SerializeField]
		private GameObject _leprechauntObj;

		[SerializeField]
		private GameObject _weaponObj;

		[SerializeField]
		private GameObject _petObj;

		[SerializeField]
		private GameObject _gadgetObj;

		[Header("[ textures ]")]
		[SerializeField]
		private UITexture _weaponTexture;

		[SerializeField]
		private UITexture _petTexture;

		[SerializeField]
		private UITexture _gadgetTexture;

		public override void UpdateView(bool force = false)
		{
			base.UpdateView(force);
			if (base.Data == null)
			{
				return;
			}
			if (_data.IsTypePack)
			{
				if (force || _prevData.Pack != _data.Pack)
				{
					_prevData.Pack = _data.Pack;
					_label.text = string.Format(LocalizationStore.Get("Key_2915"), _data.Pack);
				}
			}
			else if (force || _prevData.End != _data.End)
			{
				_prevData.End = _data.End;
				_label.text = RiliExtensions.GetTimeStringLocalizedShort(_data.End);
			}
			if (_data.Type == BonusData.BonusType.Weapons && (force || _prevData.WeaponId != _data.WeaponId))
			{
				_weaponTexture.mainTexture = ItemDb.GetItemIcon(_data.WeaponId, (ShopNGUIController.CategoryNames)ItemDb.GetItemCategory(_data.WeaponId));
			}
			else if (_data.Type == BonusData.BonusType.Pets && (force || _prevData.PetId != _data.PetId))
			{
				PetInfo info = Singleton<PetsManager>.Instance.GetInfo(_data.PetId);
				if (info != null)
				{
					_petTexture.mainTexture = Resources.Load<Texture>("OfferIcons/MiniOfferIcons/offer_icon_" + info.IdWithoutUp);
				}
			}
			if (_prevData.Type != _data.Type || force)
			{
				_currencyObj.SetActiveSafe(false);
				_leprechauntObj.SetActiveSafe(false);
				_weaponObj.SetActiveSafe(false);
				_petObj.SetActiveSafe(false);
				_gadgetObj.SetActiveSafe(false);
				if (_data.Type == BonusData.BonusType.Currency)
				{
					_currencyObj.SetActiveSafe(true);
				}
				else if (_data.Type == BonusData.BonusType.Weapons)
				{
					_weaponObj.SetActiveSafe(true);
				}
				else if (_data.Type == BonusData.BonusType.Pets)
				{
					_petObj.SetActiveSafe(true);
				}
				else if (_data.Type == BonusData.BonusType.Leprechaunt)
				{
					_leprechauntObj.SetActiveSafe(true);
				}
				else if (_data.Type == BonusData.BonusType.Gadgets)
				{
					_gadgetObj.SetActiveSafe(true);
				}
			}
		}

		private void OnClick()
		{
			Interact();
		}
	}
}
