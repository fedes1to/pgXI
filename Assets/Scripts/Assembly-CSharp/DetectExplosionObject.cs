using UnityEngine;

public class DetectExplosionObject : BaseExplosionObject
{
	[Header("Detect settings")]
	public float durationBeforeExplosion;

	private bool _isEnter;

	private void SetEnableDetectCollider(bool enable)
	{
		if (!(GetComponent<Collider>() == null))
		{
			GetComponent<Collider>().enabled = enable;
		}
	}

	private void Awake()
	{
		SetEnableDetectCollider(false);
	}

	private void OnTriggerEnter(Collider collisionObj)
	{
		CollisionEvent(collisionObj.gameObject);
	}

	private void OnCollisionEnter(Collision collisionObj)
	{
		CollisionEvent(collisionObj.gameObject);
	}

	private void CollisionEvent(GameObject collisionObj)
	{
		if (IsTargetAvailable(collisionObj.transform.root) && !_isEnter)
		{
			_isEnter = true;
			if (durationBeforeExplosion != 0f)
			{
				Invoke("RunExplosion", durationBeforeExplosion);
			}
			else
			{
				RunExplosion();
			}
		}
	}

	protected override void InitializeData()
	{
		base.InitializeData();
		SetEnableDetectCollider(true);
	}
}
