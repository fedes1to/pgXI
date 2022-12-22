using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	public class WeaponSkinsTestComponent : MonoBehaviour
	{
		private GameObject _prevGo;

		[SerializeField]
		private GameObject _go;

		[SerializeField]
		private bool _doNotCreteBaseSkin;

		[SerializeField]
		[ReadOnly]
		private List<WeaponSkin> _skins = new List<WeaponSkin>();

		[ReadOnly]
		[SerializeField]
		private WeaponSkin _currentSkin;

		private WeaponSkin _baseSkin;

		private void OnGUI()
		{
		}
	}
}
