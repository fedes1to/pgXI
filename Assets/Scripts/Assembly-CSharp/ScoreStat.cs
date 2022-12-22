using UnityEngine;

public class ScoreStat : MonoBehaviour
{
	private void Start()
	{
		GetComponent<UILabel>().text = GlobalGameController.Score.ToString();
	}
}
