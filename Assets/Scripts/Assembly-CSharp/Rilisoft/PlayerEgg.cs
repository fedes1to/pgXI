using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class PlayerEgg
	{
		[SerializeField]
		public string DataId = string.Empty;

		[SerializeField]
		public int Id = -1;

		[SerializeField]
		public long IncubationStart = -1L;

		[SerializeField]
		public bool IsReady;

		[SerializeField]
		public int Wins;

		[SerializeField]
		public int Rating;

		public PlayerEgg()
		{
		}

		public PlayerEgg(string dataEggId, int thisId)
		{
			DataId = dataEggId;
			Id = thisId;
		}

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
