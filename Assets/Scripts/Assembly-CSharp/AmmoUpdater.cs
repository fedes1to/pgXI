using System.Collections.Generic;
using System.Globalization;
using Rilisoft;
using UnityEngine;

internal sealed class AmmoUpdater : MonoBehaviour
{
	private UILabel _label;

	public GameObject ammoSprite;

	private KeyValuePair<int, string> _formatMeleeAmmoMemo = new KeyValuePair<int, string>(0, "0");

	private KeyValuePair<Ammo, string> _formatShootingAmmoMemo = new KeyValuePair<Ammo, string>(new Ammo(0, 0), "0/0");

	private void Start()
	{
		_label = GetComponent<UILabel>();
	}

	private void Update()
	{
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null)
		{
			Weapon weapon = (Weapon)WeaponManager.sharedManager.playerWeapons[WeaponManager.sharedManager.CurrentWeaponIndex];
			WeaponSounds component = weapon.weaponPrefab.GetComponent<WeaponSounds>();
			if ((!component.isMelee || component.isShotMelee) && _label != null)
			{
				_label.text = ((!component.isShotMelee) ? FormatShootingAmmoLabel(weapon.currentAmmoInClip, weapon.currentAmmoInBackpack) : FormatMeleeAmmoLabel(weapon.currentAmmoInClip, weapon.currentAmmoInBackpack));
				if (ammoSprite != null && !ammoSprite.activeSelf)
				{
					ammoSprite.SetActive(true);
				}
				return;
			}
		}
		_label.text = string.Empty;
		if (ammoSprite != null && ammoSprite.activeSelf)
		{
			ammoSprite.SetActive(false);
		}
	}

	private string FormatMeleeAmmoLabel(int currentAmmoInClip, int currentAmmoInBackpack)
	{
		int num = currentAmmoInClip + currentAmmoInBackpack;
		if (num != _formatMeleeAmmoMemo.Key)
		{
			string value = num.ToString(CultureInfo.InvariantCulture);
			_formatMeleeAmmoMemo = new KeyValuePair<int, string>(num, value);
		}
		return _formatMeleeAmmoMemo.Value;
	}

	private string FormatShootingAmmoLabel(int currentAmmoInClip, int currentAmmoInBackpack)
	{
		Ammo key = new Ammo(currentAmmoInClip, currentAmmoInBackpack);
		if (!key.Equals(_formatShootingAmmoMemo.Key))
		{
			string value = currentAmmoInClip + "/" + currentAmmoInBackpack;
			_formatShootingAmmoMemo = new KeyValuePair<Ammo, string>(key, value);
		}
		return _formatShootingAmmoMemo.Value ?? "0/0";
	}
}
