using UnityEngine;

public class HitParticle : MonoBehaviour
{
	public const float DefaultHeightFlyOutEffect = 1.75f;

	private float liveTime = -1f;

	public float maxliveTime = 0.3f;

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
		base.gameObject.SetActive(true);
		isUseMine = _isUseMine;
		liveTime = maxliveTime;
		myTransform.position = pos;
		myTransform.rotation = rot;
		myParticleSystem.enableEmission = true;
	}

	public void StartShowParticle(Vector3 pos, Quaternion rot, bool _isUseMine, Vector3 flyOutPos)
	{
		StartShowParticle(pos, rot, _isUseMine);
		if (myTransform.childCount > 0)
		{
			myParticleSystem.transform.position = flyOutPos;
		}
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
