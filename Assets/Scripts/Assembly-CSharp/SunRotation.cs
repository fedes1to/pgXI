using UnityEngine;

public class SunRotation : MonoBehaviour
{
	public AnimationCurve xRotation;

	public AnimationCurve yRotation;

	public Transform sun;

	public Transform yAxis;

	private float matchTime;

	private float matchTimeDelta;

	private void LateUpdate()
	{
		if (!(TimeGameController.sharedController != null) || PhotonNetwork.room == null || string.IsNullOrEmpty(ConnectSceneNGUIController.maxKillProperty) || !PhotonNetwork.room.customProperties.ContainsKey(ConnectSceneNGUIController.maxKillProperty))
		{
			return;
		}
		int result = -1;
		int.TryParse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty].ToString(), out result);
		if (result < 0)
		{
			return;
		}
		matchTime = (float)result * 60f;
		if ((float)TimeGameController.sharedController.timerToEndMatch < matchTime)
		{
			matchTimeDelta = matchTime - (float)TimeGameController.sharedController.timerToEndMatch;
			if ((bool)Camera.main)
			{
				sun.LookAt(Camera.main.transform);
			}
			Quaternion localRotation = default(Quaternion);
			localRotation.eulerAngles = new Vector3(xRotation.Evaluate(matchTimeDelta / matchTime), 0f, 0f);
			base.transform.localRotation = localRotation;
			localRotation.eulerAngles = new Vector3(0f, yRotation.Evaluate(matchTimeDelta / matchTime), 0f);
			yAxis.localRotation = localRotation;
		}
	}
}
