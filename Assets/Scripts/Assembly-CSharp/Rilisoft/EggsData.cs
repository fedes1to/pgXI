using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[CreateAssetMenu(fileName = "eggs_data", menuName = "Rilisoft/SO/EggsData")]
	public class EggsData : ScriptableObject
	{
		public const string EGGS_DATA_PATH = "Eggs/eggs_data";

		[SerializeField]
		public List<EggData> Eggs = new List<EggData>();

		public static EggsData Load()
		{
			EggsData eggsData = Resources.Load<EggsData>("Eggs/eggs_data");
			if (eggsData == null)
			{
				Debug.LogError("[EGGS] data not found");
				return null;
			}
			return eggsData;
		}
	}
}
