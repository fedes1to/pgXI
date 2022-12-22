using UnityEngine;

public sealed class CameraSceneController : MonoBehaviour
{
	public static CameraSceneController sharedController;

	private Vector3 posCam = new Vector3(17f, 11f, 17f);

	private Quaternion rotateCam = Quaternion.Euler(new Vector3(39f, 226f, 0f));

	private Transform myTransform;

	public RPG_Camera killCamController;

	public Transform objListener;

	public bool EnableSounds
	{
		get
		{
			return objListener.localPosition.Equals(Vector3.zero);
		}
		set
		{
			if (value)
			{
				objListener.localPosition = Vector3.zero;
			}
			else
			{
				objListener.localPosition = new Vector3(0f, 10000f, 0f);
			}
		}
	}

	private void Awake()
	{
		sharedController = this;
		myTransform = base.transform;
		EnableSounds = false;
	}

	private void Start()
	{
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(Application.loadedLevelName);
		if (infoScene != null)
		{
			posCam = infoScene.positionCam;
			rotateCam = Quaternion.Euler(infoScene.rotationCam);
		}
		myTransform.position = posCam;
		myTransform.rotation = rotateCam;
		killCamController.enabled = false;
	}

	public void SetTargetKillCam(Transform target = null)
	{
		if (target == null)
		{
			killCamController.enabled = false;
			killCamController.cameraPivot = null;
			myTransform.position = posCam;
			myTransform.rotation = rotateCam;
			EnableSounds = false;
		}
		else
		{
			killCamController.enabled = true;
			killCamController.cameraPivot = target;
			myTransform.position = target.position;
			myTransform.rotation = target.rotation;
			EnableSounds = true;
		}
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}
