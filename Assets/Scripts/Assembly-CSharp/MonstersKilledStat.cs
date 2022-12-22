using UnityEngine;

public class MonstersKilledStat : MonoBehaviour
{
	private void Start()
	{
		GetComponent<UILabel>().text = PlayerPrefs.GetInt(Defs.KilledZombiesSett, 0).ToString();
	}
}
