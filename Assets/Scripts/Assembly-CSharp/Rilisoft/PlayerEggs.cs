using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class PlayerEggs
	{
		[SerializeField]
		public List<PlayerEgg> Eggs = new List<PlayerEgg>();

		public override string ToString()
		{
			return JsonUtility.ToJson(this);
		}

		public static PlayerEggs Create(string raw)
		{
			return JsonUtility.FromJson<PlayerEggs>(raw);
		}
	}
}
