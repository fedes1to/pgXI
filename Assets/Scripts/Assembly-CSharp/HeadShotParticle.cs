using UnityEngine;

public class HeadShotParticle : MonoBehaviour
{
	private float liveTime = -1f;

	public float maxliveTime = 1.5f;

	public bool isUseMine;

	private Transform myTransform;

	public ParticleSystem myParticleSystem;

	private void Start()
	{
		myTransform = base.transform;
		myTransform.position = new Vector3(-10000f, -10000f, -10000f);
	}

	public void StartShowParticle(Vector3 pos, Quaternion rot, bool _isUseMine)
	{
		isUseMine = _isUseMine;
		liveTime = maxliveTime;
		myTransform.position = pos;
		myTransform.rotation = rot;
		myParticleSystem.Emit(myTransform.position, myTransform.position, 1f, liveTime, new Color32());
	}

	private void Update()
	{
		if (!(liveTime < 0f))
		{
			liveTime -= Time.deltaTime;
			if (liveTime < 0f)
			{
				myTransform.position = new Vector3(-10000f, -10000f, -10000f);
				isUseMine = false;
			}
		}
	}
}
