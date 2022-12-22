using System.Collections;
using Rilisoft;
using UnityEngine;

public class TargetScanner : MonoBehaviour
{
	public float DetectRadius = 30f;

	[Range(0f, 2f)]
	public float UpdateFrequency = 0.3f;

	[ReadOnly]
	public GameObject Target;

	public Transform LoopPoint;

	private void OnEnable()
	{
		StartCoroutine(CheckTargets());
	}

	private IEnumerator CheckTargets()
	{
		if (Defs.isDaterRegim)
		{
			yield break;
		}
		while (true)
		{
			GameObject closestTargetObj = null;
			float closestTarget = float.MaxValue;
			Initializer.TargetsList targets = new Initializer.TargetsList(WeaponManager.sharedManager.myPlayerMoveC, false, false);
			foreach (Transform enemy in targets)
			{
				if (enemy == null || enemy == base.gameObject || enemy == WeaponManager.sharedManager.myPlayer)
				{
					continue;
				}
				Vector3 direction = enemy.position - base.transform.position;
				Vector3 lookPoint = ((!(LoopPoint != null)) ? base.transform.position : LoopPoint.position);
				float targetDistance = direction.sqrMagnitude;
				if ((targetDistance < closestTarget && targetDistance < Mathf.Pow(DetectRadius, 2f)) || Defs.isDaterRegim)
				{
					Vector3 popravochka = Vector3.zero;
					BoxCollider _collider = enemy.GetComponent<BoxCollider>();
					if (_collider != null)
					{
						popravochka = _collider.center;
					}
					RaycastHit hit;
					if (Physics.Raycast(new Ray(lookPoint, enemy.position + popravochka - lookPoint), layerMask: Tools.AllWithoutDamageCollidersMaskAndWithoutRocket & ~(1 << LayerMask.NameToLayer("Pets")), hitInfo: out hit, maxDistance: DetectRadius) && (hit.collider.gameObject == enemy.gameObject || (hit.collider.gameObject.transform.parent != null && (hit.collider.gameObject.transform.parent.Equals(enemy) || hit.collider.gameObject.transform.parent.Equals(enemy.parent)))))
					{
						closestTarget = targetDistance;
						closestTargetObj = enemy.gameObject;
					}
				}
				yield return null;
			}
			Target = closestTargetObj;
			yield return new WaitForSeconds(UpdateFrequency);
		}
	}
}
