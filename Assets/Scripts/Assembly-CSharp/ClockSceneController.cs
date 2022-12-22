using System;
using UnityEngine;

public class ClockSceneController : MonoBehaviour
{
	public enum TypeHand
	{
		minutes,
		hour
	}

	public TypeHand type;

	private Transform thisTransform;

	public DisableObjectFromTimer bats;

	private int oldValue = -1000;

	private void Start()
	{
		thisTransform = base.transform;
		UpdateAngle();
	}

	private void Update()
	{
		UpdateAngle();
	}

	private void UpdateAngle()
	{
		DateTime now = DateTime.Now;
		int num = ((type != 0) ? (now.Hour * 60 + now.Minute) : now.Minute);
		if (num != oldValue)
		{
			if (bats != null && num < oldValue && num == 0)
			{
				bats.timer = 10f;
				bats.gameObject.SetActive(true);
			}
			oldValue = num;
			if (type == TypeHand.hour && num >= 720)
			{
				num -= 720;
			}
			float y = 360f * (float)num / ((type != 0) ? 720f : 60f);
			thisTransform.localRotation = Quaternion.Euler(new Vector3(0f, y, 0f));
		}
	}
}
