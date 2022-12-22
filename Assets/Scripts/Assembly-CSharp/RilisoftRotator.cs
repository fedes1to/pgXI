using System;
using Rilisoft;
using UnityEngine;

public class RilisoftRotator : MonoBehaviour
{
	public float rate = 10f;

	private Transform _transform;

	public static float RotationRateForCharacterInMenues
	{
		get
		{
			float num = -120f;
			return num * ((BuildSettings.BuildTargetPlatform != RuntimePlatform.Android) ? 0.5f : 2f);
		}
	}

	public static void RotateCharacter(Transform character, float rotationRate, Rect touchZone, ref float idleTimerStartedTime, ref float lastTimeRotated, Func<bool> canProcess = null)
	{
		if (canProcess == null || canProcess())
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Moved && touchZone.Contains(touch.position))
				{
					idleTimerStartedTime = Time.realtimeSinceStartup;
					character.Rotate(Vector3.up, touch.deltaPosition.x * rotationRate * 0.5f * (Time.realtimeSinceStartup - lastTimeRotated));
				}
			}
			if (Application.isEditor)
			{
				float num = Input.GetAxis("Mouse ScrollWheel") * 10f * rotationRate * (Time.realtimeSinceStartup - lastTimeRotated);
				character.Rotate(Vector3.up, num);
				if (num != 0f)
				{
					idleTimerStartedTime = Time.realtimeSinceStartup;
				}
			}
		}
		lastTimeRotated = Time.realtimeSinceStartup;
	}

	private void Start()
	{
		_transform = base.transform;
	}

	private void Update()
	{
		_transform.Rotate(Vector3.forward, rate * Time.deltaTime, Space.Self);
	}
}
