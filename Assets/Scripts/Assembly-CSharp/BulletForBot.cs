using System;
using System.Collections;
using UnityEngine;

internal sealed class BulletForBot : MonoBehaviour
{
	public delegate void OnBulletDamageDelegate(GameObject targetDamage, Vector3 positionDamage);

	[NonSerialized]
	public float lifeTime;

	private float _bulletSpeed;

	private Vector3 _startPos;

	private Vector3 _endPos;

	private bool _isFrienlyFire;

	private float _startBulletTime;

	private bool doDamage = true;

	private bool _isMoveByPhysics;

	public bool needDestroyByStop { get; set; }

	public bool IsUse { get; private set; }

	public event OnBulletDamageDelegate OnBulletDamage;

	public void StartBullet(Vector3 startPos, Vector3 endPos, float bulletSpeed, bool isFriendlyFire, bool doDamage)
	{
		_isMoveByPhysics = false;
		_startPos = startPos;
		_endPos = endPos;
		_isFrienlyFire = isFriendlyFire;
		_bulletSpeed = bulletSpeed;
		base.transform.position = _startPos;
		IsUse = true;
		base.transform.gameObject.SetActive(true);
		_startBulletTime = Time.realtimeSinceStartup;
		base.transform.rotation = Quaternion.LookRotation(endPos - startPos);
		this.doDamage = doDamage;
	}

	private void StopBullet()
	{
		if (needDestroyByStop)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (_isMoveByPhysics)
		{
			SetVisible(false);
		}
		else
		{
			base.transform.gameObject.SetActive(false);
		}
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		IsUse = false;
		if (_isMoveByPhysics)
		{
			EnablePhysicsGravityControll(false);
		}
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
		if (!IsUse)
		{
			return;
		}
		Transform root = collisionObj.transform.root;
		if (!(base.transform.root == root.transform.root) && (_isFrienlyFire || !root.tag.Equals("Enemy")))
		{
			if (root.tag.Equals("Player") || root.tag.Equals("Turret") || root.tag.Equals("Pet"))
			{
				CheckRunDamageEvent(root.gameObject);
			}
			else if (_isFrienlyFire && root.tag.Equals("Enemy"))
			{
				CheckRunDamageEvent(root.gameObject);
			}
			else
			{
				CheckRunDamageEvent(null);
			}
		}
	}

	private void CheckRunDamageEvent(GameObject target)
	{
		if (this.OnBulletDamage != null)
		{
			if (doDamage)
			{
				this.OnBulletDamage(target, base.transform.position);
			}
			StopBullet();
		}
	}

	private void Update()
	{
		if (IsUse)
		{
			if (!_isMoveByPhysics)
			{
				Vector3 vector = _endPos - _startPos;
				base.transform.position += vector.normalized * _bulletSpeed * Time.deltaTime;
			}
			if (Time.realtimeSinceStartup - _startBulletTime >= lifeTime)
			{
				StopBullet();
			}
		}
	}

	private void EnablePhysicsGravityControll(bool enable)
	{
		GetComponent<Rigidbody>().useGravity = enable;
		GetComponent<Rigidbody>().isKinematic = !enable;
	}

	public void ApplyForceFroBullet(Vector3 startPos, Vector3 endPos, bool isFriendlyFire, float forceValue, Vector3 forceVector, bool doDamage)
	{
		_isMoveByPhysics = true;
		_isFrienlyFire = isFriendlyFire;
		_startBulletTime = Time.realtimeSinceStartup;
		base.transform.position = startPos;
		base.transform.rotation = Quaternion.LookRotation(endPos - startPos);
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		SetVisible(true);
		EnablePhysicsGravityControll(true);
		IsUse = true;
		this.doDamage = doDamage;
		StartCoroutine(ApplyForce(forceVector * forceValue));
	}

	private IEnumerator ApplyForce(Vector3 force)
	{
		yield return new WaitForEndOfFrame();
		GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
	}

	private void SetVisible(bool enable)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(enable);
		}
		if (GetComponent<Renderer>() != null)
		{
			GetComponent<Renderer>().enabled = enable;
		}
		if (GetComponent<ParticleSystem>() != null)
		{
			GetComponent<ParticleSystem>().enableEmission = enable;
		}
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = enable;
		}
	}
}
