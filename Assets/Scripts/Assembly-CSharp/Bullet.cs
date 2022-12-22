using System.Reflection;
using UnityEngine;

internal sealed class Bullet : MonoBehaviour
{
	private float LifeTime = 0.5f;

	private float RespawnTime;

	public float bulletSpeed = 200f;

	public float lifeS = 100f;

	public Vector3 startPos;

	public Vector3 endPos;

	public bool isUse;

	public TrailRenderer myRender;

	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	public void StartBullet()
	{
		base.gameObject.SetActive(true);
		CancelInvoke("RemoveSelf");
		Invoke("RemoveSelf", LifeTime);
		base.transform.position = startPos;
		isUse = true;
		myRender.enabled = true;
	}

	[Obfuscation(Exclude = true)]
	private void RemoveSelf()
	{
		base.transform.position = new Vector3(-10000f, -10000f, -10000f);
		myRender.enabled = false;
		isUse = false;
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (isUse)
		{
			base.transform.position += (endPos - startPos).normalized * bulletSpeed * Time.deltaTime;
			if (Vector3.SqrMagnitude(startPos - base.transform.position) >= lifeS * lifeS)
			{
				RemoveSelf();
			}
		}
	}
}
