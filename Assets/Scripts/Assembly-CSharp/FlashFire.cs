using System.Collections;
using UnityEngine;

public class FlashFire : MonoBehaviour
{
	public GameObject gunFlashObj;

	public float timeFireAction = 0.2f;

	private float activeTime;

	private WeaponSounds ws;

	private void Awake()
	{
		ws = GetComponent<WeaponSounds>();
	}

	private void Start()
	{
		if (gunFlashObj == null)
		{
			foreach (Transform item in base.transform)
			{
				bool flag = false;
				if (item.gameObject.name.Equals("BulletSpawnPoint"))
				{
					foreach (Transform item2 in item)
					{
						if (item2.gameObject.name.Equals("GunFlash"))
						{
							flag = true;
							gunFlashObj = item2.gameObject;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		WeaponManager.SetGunFlashActive(gunFlashObj, false);
	}

	private void Update()
	{
		if (activeTime > 0f)
		{
			activeTime -= Time.deltaTime;
			if (activeTime <= 0f)
			{
				WeaponManager.SetGunFlashActive(gunFlashObj, false);
			}
		}
	}

	public void fire(Player_move_c moveC)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(fireCourotine(moveC));
		}
	}

	public IEnumerator fireCourotine(Player_move_c moveC)
	{
		WeaponManager.SetGunFlashActive(gunFlashObj, true);
		activeTime = timeFireAction;
		if (!(ws != null) || !ws.railgun || !(gunFlashObj != null))
		{
			yield break;
		}
		Ray ray = new Ray(gunFlashObj.transform.parent.position, gunFlashObj.transform.parent.parent.forward);
		bool isReflection = false;
		int _countReflection = 0;
		if (ws.countReflectionRay == 1)
		{
			WeaponManager.AddRay(ray.origin, ray.direction, ws.railName);
			yield break;
		}
		do
		{
			Player_move_c.RayHitsInfo rayHitsInfo = moveC.GetHitsFromRay(ray, false);
			bool isOneRayOrFirstNoReflection = _countReflection == 0 && !rayHitsInfo.obstacleFound;
			WeaponManager.AddRay(forw: ray.direction, len: (!isOneRayOrFirstNoReflection) ? rayHitsInfo.lenRay : 150f, pos: ray.origin, nm: ws.railName);
			if (rayHitsInfo.obstacleFound)
			{
				ray = rayHitsInfo.rayReflect;
				isReflection = true;
			}
			yield return new WaitForSeconds((float)_countReflection * 0.05f);
			_countReflection++;
		}
		while (isReflection && _countReflection < ws.countReflectionRay);
	}
}
