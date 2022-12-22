using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class EggPetInfo
	{
		[SerializeField]
		[Tooltip("Тип пета")]
		public ItemDb.ItemRarity Rarity;

		[SerializeField]
		[Tooltip("шанс получения")]
		public float Chance;
	}
}
