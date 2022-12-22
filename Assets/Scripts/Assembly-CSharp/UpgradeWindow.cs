using System.Collections;
using UnityEngine;

public class UpgradeWindow : MonoBehaviour
{
	public TweenColor[] upgrades;

	public void SetUpgrade(int num, int minBoughtIndex)
	{
		for (int i = 0; i < upgrades.Length; i++)
		{
			upgrades[i].gameObject.SetActive(i <= num);
			if (i <= minBoughtIndex)
			{
				upgrades[i].enabled = false;
				upgrades[i].GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				upgrades[i].enabled = true;
				StartCoroutine(ResetToBeginning(upgrades[i]));
				upgrades[i].ResetToBeginning();
			}
		}
	}

	private IEnumerator ResetToBeginning(TweenColor tw)
	{
		yield return null;
		tw.ResetToBeginning();
	}
}
