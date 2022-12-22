using UnityEngine;

public class CountKillsDuelLabel : MonoBehaviour
{
	public bool enemyKills;

	private UILabel label;

	private void Awake()
	{
		label = GetComponent<UILabel>();
	}

	private void Update()
	{
		int b = 0;
		if (enemyKills)
		{
			if (DuelController.instance != null && DuelController.instance.opponentNetworkTable != null)
			{
				b = ((DuelController.instance.opponentNetworkTable.CountKills < 0) ? DuelController.instance.opponentNetworkTable.oldCountKills : DuelController.instance.opponentNetworkTable.CountKills);
			}
		}
		else
		{
			b = GlobalGameController.CountKills;
		}
		label.text = Mathf.Max(0, b).ToString();
	}
}
