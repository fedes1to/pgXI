using UnityEngine;

public class TargetDetectExplosion : MonoBehaviour
{
	[Header("Detect settings")]
	public float durationBeforeExplosion;

	public DamagedExplosionObject explosionScript;

	private bool _isEnter;

	private void Awake()
	{
		if (explosionScript == null)
		{
			explosionScript = base.transform.parent.GetComponent<DamagedExplosionObject>();
		}
	}

	private bool IsTargetAvailable(Transform targetTransform)
	{
		if (targetTransform.Equals(base.transform))
		{
			return false;
		}
		return targetTransform.CompareTag("Player") || targetTransform.CompareTag("Enemy") || targetTransform.CompareTag("Turret");
	}

	private void OnTriggerEnter(Collider collisionObj)
	{
		if (IsTargetAvailable(collisionObj.transform.root) && !_isEnter)
		{
			_isEnter = true;
			if (durationBeforeExplosion != 0f)
			{
				Invoke("OnTargetEnter", durationBeforeExplosion);
			}
			else
			{
				OnTargetEnter();
			}
		}
	}

	private void OnTargetEnter()
	{
		explosionScript.GetDamage(explosionScript.healthPoints);
	}
}
