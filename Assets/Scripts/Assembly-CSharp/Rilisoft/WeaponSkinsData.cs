using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[CreateAssetMenu(fileName = "weapon_skins_data", menuName = "Rilisoft/SO/WeaponSkins", order = 1)]
	public class WeaponSkinsData : ScriptableObject
	{
		public List<WeaponSkin> Data = new List<WeaponSkin>();
	}
}
