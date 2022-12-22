using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDummy : MonoBehaviour
{
	public enum HealthType
	{
		AverageOnTier,
		LowestOnTier,
		HighestOnTier
	}

	public enum ArmorType
	{
		None,
		LowArmorOnTier,
		TopArmorOnTier
	}

	public float health;

	private float startHealth;

	public float awakeTime = 2f;

	public float hideTime = 2f;

	private bool isDown = true;

	private bool isHiding;

	public bool flyTarget;

	public bool hidingTarget;

	private Animation _animation;

	private AudioSource audioSource;

	private float nextHideTime;

	private float nextUpTime;

	public Collider[] colliders;

	public Transform armorArmLeft;

	public Transform armorArmRight;

	public Transform armorPoint;

	public HealthType healthType = HealthType.LowestOnTier;

	public ArmorType armorType;

	public SkinnedMeshRenderer skinnedMesh;

	public Texture hitTexture;

	private Texture myTexture;

	private Material bodyMaterial;

	public AudioClip hitSound;

	public AudioClip upSound;

	public AudioClip downSound;

	public Transform jetpackPoint;

	public GameObject jetpackSounds;

	public Transform explosionEffect;

	public bool isDead
	{
		get
		{
			return isDown;
		}
	}

	private void OnPolygonEnter()
	{
		if (jetpackSounds != null)
		{
			jetpackSounds.SetActive(Defs.isSoundFX);
		}
		if (jetpackPoint != null)
		{
			jetpackPoint.gameObject.SetActive(true);
		}
	}

	private void OnPolygonExit()
	{
		if (jetpackSounds != null)
		{
			jetpackSounds.SetActive(false);
		}
		if (jetpackPoint != null)
		{
			jetpackPoint.gameObject.SetActive(false);
		}
	}

	public void GetDamage(float dm, Player_move_c.TypeKills damageType)
	{
		if (!isDown)
		{
			health -= dm;
			DamageEffects((int)damageType);
			if (Defs.isSoundFX)
			{
				audioSource.PlayOneShot(hitSound);
			}
			if (health <= 0f)
			{
				Die();
			}
		}
	}

	private void Die()
	{
		if (!flyTarget)
		{
			LayDown();
			return;
		}
		GameObject gameObject = Object.Instantiate(explosionEffect.gameObject, explosionEffect.position, Quaternion.identity) as GameObject;
		gameObject.SetActive(true);
		gameObject.GetComponent<AudioSource>().enabled = Defs.isSoundFX;
		Object.Destroy(gameObject, 2f);
		GetComponent<MovingDummy>().ResetPath();
		health = startHealth;
	}

	private void DamageEffects(int _typeKills)
	{
	}

	private IEnumerator HitMaterial(bool poison)
	{
		SetTextureForBody((!poison) ? SkinsController.damageHitTexture : SkinsController.poisonHitTexture);
		yield return new WaitForSeconds(0.125f);
		SetTextureForBody(myTexture);
	}

	public void SetTextureForBody(Texture texture)
	{
		if (bodyMaterial != null)
		{
			bodyMaterial.mainTexture = texture;
		}
	}

	private string GetArmorForTier(bool topArmor)
	{
		List<string> list = Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0];
		List<string> list2 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			if (Wear.TierForWear(list[i]) <= ExpController.OurTierForAnyPlace())
			{
				list2.Add(list[i]);
			}
		}
		return list2[(!topArmor) ? (list2.Count - 3) : (list2.Count - 1)];
	}

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		_animation = GetComponent<Animation>();
		_animation.Play("Dummy_Idle", PlayMode.StopAll);
		int num = 1;
		switch (healthType)
		{
		case HealthType.LowestOnTier:
			num = ExpController.LevelsForTiers[ExpController.OurTierForAnyPlace()];
			break;
		case HealthType.HighestOnTier:
			num = ((ExpController.LevelsForTiers.Length > ExpController.OurTierForAnyPlace() + 1) ? ExpController.LevelsForTiers[ExpController.OurTierForAnyPlace() + 1] : 31);
			break;
		case HealthType.AverageOnTier:
		{
			int num2 = ExpController.LevelsForTiers[ExpController.OurTierForAnyPlace()];
			int num3 = ((ExpController.LevelsForTiers.Length > ExpController.OurTierForAnyPlace() + 1) ? ExpController.LevelsForTiers[ExpController.OurTierForAnyPlace() + 1] : 31);
			num = Mathf.RoundToInt((float)(num3 + num2) / 2f);
			break;
		}
		}
		health = ExperienceController.HealthByLevel[num];
		if (armorType != 0)
		{
			string armorForTier = GetArmorForTier(armorType == ArmorType.TopArmorOnTier);
			SetArmor(armorForTier);
			health += Wear.armorNum[armorForTier];
		}
		startHealth = health;
		bodyMaterial = skinnedMesh.material;
		skinnedMesh.sharedMaterial = bodyMaterial;
		myTexture = bodyMaterial.mainTexture;
	}

	private void Start()
	{
	}

	private void LayDown()
	{
		if (Defs.isSoundFX)
		{
			audioSource.clip = downSound;
			audioSource.Play();
		}
		isDown = true;
		Collider[] array = colliders;
		foreach (Collider collider in array)
		{
			collider.enabled = false;
		}
		_animation.Play("Dead", PlayMode.StopAll);
		nextUpTime = Time.time + awakeTime;
		if (hidingTarget)
		{
			nextHideTime = Time.time + hideTime + _animation.GetClip("Dummy_Up").length + _animation.GetClip("Dead").length;
		}
	}

	private IEnumerator GetUp()
	{
		if (Defs.isSoundFX)
		{
			audioSource.clip = upSound;
			audioSource.Play();
		}
		isDown = false;
		isHiding = false;
		if (!flyTarget)
		{
			_animation.Play("Dummy_Up", PlayMode.StopAll);
			yield return new WaitForSeconds(_animation.GetClip("Dummy_Up").length * 0.55f);
		}
		yield return null;
		Collider[] array = colliders;
		foreach (Collider collider in array)
		{
			collider.enabled = true;
		}
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		if (hidingTarget && !isDown && nextHideTime < Time.time)
		{
			isHiding = true;
			LayDown();
		}
		if (isDown && nextUpTime < Time.time)
		{
			if (!isHiding)
			{
				health = startHealth;
			}
			StartCoroutine(GetUp());
		}
	}

	public void SetArmor(string armor)
	{
		GameObject gameObject = Resources.Load("Armor/" + armor) as GameObject;
		if (gameObject == null)
		{
			Debug.LogWarning("armorPrefab == null");
			return;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject);
		ArmorRefs component = gameObject2.transform.GetChild(0).GetComponent<ArmorRefs>();
		if (component != null)
		{
			if (component.leftBone != null)
			{
				component.leftBone.parent = armorArmLeft;
				component.leftBone.localPosition = Vector3.zero;
				component.leftBone.localRotation = Quaternion.identity;
				component.leftBone.localScale = new Vector3(1f, 1f, 1f);
			}
			if (component.rightBone != null)
			{
				component.rightBone.parent = armorArmRight;
				component.rightBone.localPosition = Vector3.zero;
				component.rightBone.localRotation = Quaternion.identity;
				component.rightBone.localScale = new Vector3(1f, 1f, 1f);
			}
			gameObject2.transform.parent = armorPoint;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			Player_move_c.SetLayerRecursively(gameObject2, base.gameObject.layer);
		}
	}
}
