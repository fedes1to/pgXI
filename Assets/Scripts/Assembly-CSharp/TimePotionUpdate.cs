using System;
using UnityEngine;

public class TimePotionUpdate : MonoBehaviour
{
	public UILabel myLabel;

	public GameObject mySpriteObj;

	public string myPotionName;

	[NonSerialized]
	public float timerUpdate = -1f;

	private void Start()
	{
	}

	private void Update()
	{
		if (myLabel.enabled)
		{
			timerUpdate -= Time.deltaTime;
			if (timerUpdate < 0f)
			{
				timerUpdate = 0.25f;
				SetTimeForLabel();
			}
		}
	}

	private void SetTimeForLabel()
	{
		if (!PotionsController.sharedController.PotionIsActive(myPotionName))
		{
			if (mySpriteObj != null && mySpriteObj.activeSelf)
			{
				mySpriteObj.SetActive(false);
				myLabel.text = string.Empty;
			}
			return;
		}
		if (mySpriteObj != null && !mySpriteObj.activeSelf)
		{
			mySpriteObj.SetActive(true);
		}
		float num = PotionsController.sharedController.RemainDuratioForPotion(myPotionName);
		TimeSpan timeSpan = TimeSpan.FromSeconds(num);
		myLabel.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
		if (num <= 5f)
		{
			myLabel.color = new Color(1f, 0f, 0f);
		}
		else
		{
			myLabel.color = new Color(1f, 1f, 1f);
		}
	}

	public void UpdateTime()
	{
		float num = PotionsController.sharedController.RemainDuratioForPotion(myPotionName);
		TimeSpan timeSpan = TimeSpan.FromSeconds(num);
		myLabel.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
		if (num <= 5f)
		{
			myLabel.color = new Color(1f, 0f, 0f);
		}
		else
		{
			myLabel.color = new Color(1f, 1f, 1f);
		}
	}
}
