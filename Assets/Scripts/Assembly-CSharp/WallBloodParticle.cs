using UnityEngine;

public class WallBloodParticle : MonoBehaviour
{
	private float liveTime = -1f;

	private float maxliveTime = 0.1f;

	public bool isUseMine;

	private Transform myTransform;

	public ParticleSystem myParticleSystem;

	private void Start()
	{
		myTransform = base.transform;
		myTransform.position = new Vector3(-10000f, -10000f, -10000f);
		myParticleSystem.enableEmission = false;
		base.gameObject.SetActive(false);
	}

	public void StartShowParticle(Vector3 pos, Quaternion rot, bool _isUseMine)
	{
		isUseMine = _isUseMine;
		liveTime = maxliveTime;
		myTransform.position = pos;
		myTransform.rotation = rot;
		myParticleSystem.enableEmission = true;
		base.gameObject.SetActive(true);
	}

	private void Update()
	{
		if (!(liveTime < 0f))
		{
			liveTime -= Time.deltaTime;
			if (liveTime < 0f)
			{
				myTransform.position = new Vector3(-10000f, -10000f, -10000f);
				myParticleSystem.enableEmission = false;
				isUseMine = false;
				base.gameObject.SetActive(false);
			}
		}
	}
}
