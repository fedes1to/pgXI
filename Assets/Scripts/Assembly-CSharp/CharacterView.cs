using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
	public enum CharacterType
	{
		Player,
		Mech,
		Turret
	}

	public Transform mech;

	public SkinnedMeshRenderer mechBodyRenderer;

	public SkinnedMeshRenderer mechHandRenderer;

	public SkinnedMeshRenderer mechGunRenderer;

	public Material[] mechGunMaterials;

	public Material[] mechBodyMaterials;

	public Transform turret;

	private CharacterInterface _characterInterface;

	private AnimationCoroutineRunner _animationRunner;

	private AnimationClip _profile;

	private GameObject _weapon;

	public CharacterInterface characterInterface
	{
		get
		{
			if (_characterInterface == null)
			{
				CreateCharacterModel();
			}
			return _characterInterface;
		}
	}

	public Transform armorPoint
	{
		get
		{
			return characterInterface.armorPoint;
		}
	}

	public Transform hatPoint
	{
		get
		{
			return characterInterface.hatPoint;
		}
	}

	public Transform body
	{
		get
		{
			return characterInterface.gunPoint;
		}
	}

	public Transform character
	{
		get
		{
			return characterInterface.transform;
		}
	}

	private AnimationCoroutineRunner AnimationRunner
	{
		get
		{
			if (_animationRunner == null)
			{
				_animationRunner = GetComponent<AnimationCoroutineRunner>();
			}
			return _animationRunner;
		}
	}

	private void CreateCharacterModel()
	{
		GameObject original = Resources.Load("Character_model") as GameObject;
		_characterInterface = UnityEngine.Object.Instantiate(original).GetComponent<CharacterInterface>();
		_characterInterface.transform.SetParent(base.transform, false);
		_characterInterface.SetCharacterType(false, false, false);
		Player_move_c.SetLayerRecursively(_characterInterface.gameObject, base.gameObject.layer);
		ShopNGUIController.DisableLightProbesRecursively(_characterInterface.gameObject);
	}

	public void ShowCharacterType(CharacterType characterType)
	{
		character.gameObject.SetActive(false);
		if (mech != null)
		{
			mech.gameObject.SetActive(false);
		}
		if (turret != null)
		{
			turret.gameObject.SetActive(false);
		}
		switch (characterType)
		{
		case CharacterType.Player:
			character.gameObject.SetActive(true);
			break;
		case CharacterType.Mech:
			mech.gameObject.SetActive(true);
			break;
		case CharacterType.Turret:
			turret.gameObject.SetActive(true);
			break;
		}
	}

	public void UpdateMech(int mechUpgrade)
	{
		mechBodyRenderer.material = mechBodyMaterials[mechUpgrade];
		mechHandRenderer.material = mechBodyMaterials[mechUpgrade];
		mechGunRenderer.material = mechGunMaterials[mechUpgrade];
		mechBodyRenderer.material.SetColor("_ColorRili", Color.white);
		mechHandRenderer.material.SetColor("_ColorRili", Color.white);
	}

	public void UpdateTurret(int turretUpgrade)
	{
	}

	public void SetWeaponAndSkin(string tg, Texture skinForPers, bool replaceRemovedWeapons)
	{
		AnimationRunner.StopAllCoroutines();
		if (WeaponManager.sharedManager == null)
		{
			return;
		}
		if (armorPoint.childCount > 0)
		{
			ArmorRefs component = armorPoint.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
			if (component != null)
			{
				if (component.leftBone != null)
				{
					component.leftBone.parent = armorPoint.GetChild(0).GetChild(0);
				}
				if (component.rightBone != null)
				{
					component.rightBone.parent = armorPoint.GetChild(0).GetChild(0);
				}
			}
		}
		List<Transform> list = new List<Transform>();
		foreach (Transform item in body)
		{
			list.Add(item);
		}
		foreach (Transform item2 in list)
		{
			item2.parent = null;
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		if (tg == null)
		{
			return;
		}
		if (_profile != null)
		{
			Resources.UnloadAsset(_profile);
			_profile = null;
		}
		GameObject gameObject = null;
		if (tg == "WeaponGrenade")
		{
			gameObject = Resources.Load<GameObject>("WeaponGrenade");
		}
		else
		{
			try
			{
				string weaponName = ItemDb.GetByTag(tg).PrefabName;
				gameObject = WeaponManager.sharedManager.weaponsInGame.OfType<GameObject>().FirstOrDefault((GameObject wp) => wp.name == weaponName);
			}
			catch (Exception ex)
			{
				if (Application.isEditor)
				{
					Debug.LogError("Exception in var weaponName = ItemDb.GetByTag(tg).PrefabName: " + ex);
				}
			}
			if (replaceRemovedWeapons && gameObject != null)
			{
				WeaponSounds weaponSounds = gameObject.GetComponent<WeaponSounds>();
				if (weaponSounds != null && (WeaponManager.Removed150615_PrefabNames.Contains(gameObject.name) || weaponSounds.tier > 100))
				{
					gameObject = WeaponManager.sharedManager.weaponsInGame.OfType<GameObject>().FirstOrDefault((GameObject wp) => wp.name.Equals(weaponSounds.alternativeName));
				}
			}
		}
		if (gameObject == null)
		{
			Debug.Log("pref == null");
			return;
		}
		_profile = Resources.Load<AnimationClip>("ProfileAnimClips/" + gameObject.name + "_Profile");
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		Player_move_c.PerformActionRecurs(gameObject2, delegate(Transform t)
		{
			MeshRenderer component4 = t.GetComponent<MeshRenderer>();
			SkinnedMeshRenderer component5 = t.GetComponent<SkinnedMeshRenderer>();
			if (component4 != null)
			{
				component4.useLightProbes = false;
			}
			if (component5 != null)
			{
				component5.useLightProbes = false;
			}
		});
		Player_move_c.SetLayerRecursively(gameObject2, base.gameObject.layer);
		gameObject2.transform.parent = body;
		_weapon = gameObject2;
		_weapon.transform.localScale = new Vector3(1f, 1f, 1f);
		_weapon.transform.position = body.transform.position;
		_weapon.transform.localPosition = Vector3.zero;
		_weapon.transform.localRotation = Quaternion.identity;
		WeaponSounds component2 = _weapon.GetComponent<WeaponSounds>();
		if (armorPoint.childCount > 0 && component2 != null)
		{
			ArmorRefs component3 = armorPoint.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
			if (component3 != null)
			{
				if (component3.leftBone != null && component2.LeftArmorHand != null)
				{
					component3.leftBone.parent = component2.LeftArmorHand;
					component3.leftBone.localPosition = Vector3.zero;
					component3.leftBone.localRotation = Quaternion.identity;
					component3.leftBone.localScale = new Vector3(1f, 1f, 1f);
				}
				if (component3.rightBone != null && component2.RightArmorHand != null)
				{
					component3.rightBone.parent = component2.RightArmorHand;
					component3.rightBone.localPosition = Vector3.zero;
					component3.rightBone.localRotation = Quaternion.identity;
					component3.rightBone.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}
		PlayWeaponAnimation();
		SetSkinTexture(skinForPers);
		if (tg == "WeaponGrenade")
		{
			SetupWeaponGrenade(gameObject2);
		}
		WeaponSkin skinForWeapon = WeaponSkinsManager.GetSkinForWeapon(gameObject2.nameNoClone());
		if (skinForWeapon != null)
		{
			skinForWeapon.SetTo(gameObject2);
		}
	}

	public void SetupWeaponGrenade(GameObject weaponGrenade)
	{
		GameObject original = Resources.Load<GameObject>("Rocket");
		Rocket component = (UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Rocket>();
		component.enabled = false;
		component.GetComponent<Rigidbody>().useGravity = false;
		component.GetComponent<Rigidbody>().isKinematic = true;
		component.rocketNum = 10;
		Player_move_c.SetLayerRecursively(component.gameObject, base.gameObject.layer);
		component.transform.parent = weaponGrenade.GetComponent<WeaponSounds>().grenatePoint;
		component.transform.localPosition = Vector3.zero;
		component.transform.localRotation = Quaternion.identity;
		component.transform.localScale = Vector3.one;
	}

	public void SetSkinTexture(Texture skin)
	{
		characterInterface.SetSkin(skin, (body.transform.childCount <= 0) ? null : body.transform.GetChild(0).GetComponent<WeaponSounds>());
	}

	public void UpdateHat(string hat)
	{
		string @string = Storager.getString(Defs.VisualHatArmor, false);
		if (!string.IsNullOrEmpty(@string) && Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(hat) >= 0 && Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(hat) < Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(@string))
		{
			hat = @string;
		}
		characterInterface.UpdateHat(hat, !ShopNGUIController.ShowWear);
	}

	public void RemoveHat()
	{
		characterInterface.RemoveHat();
	}

	public void UpdateCape(string cape, Texture capeTex = null)
	{
		characterInterface.UpdateCape(cape, capeTex, !ShopNGUIController.ShowWear);
	}

	public void RemoveCape()
	{
		characterInterface.RemoveCape();
	}

	public void UpdateMask(string mask)
	{
		characterInterface.UpdateMask(mask, !ShopNGUIController.ShowWear);
	}

	public void RemoveMask()
	{
		characterInterface.RemoveMask();
	}

	public void UpdateBoots(string bs)
	{
		characterInterface.UpdateBoots(bs, !ShopNGUIController.ShowWear);
	}

	public void RemoveBoots()
	{
		characterInterface.RemoveBoots();
	}

	public void UpdatePet(string bs)
	{
		characterInterface.UpdatePet(bs);
	}

	public void RemovePet()
	{
		characterInterface.UpdatePet(string.Empty);
	}

	public void UpdateArmor(string armor)
	{
		string @string = Storager.getString(Defs.VisualArmor, false);
		if (!string.IsNullOrEmpty(@string) && Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(armor) >= 0 && Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(armor) < Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(@string))
		{
			armor = @string;
		}
		characterInterface.UpdateArmor(armor, _weapon);
	}

	public void RemoveArmor()
	{
		characterInterface.RemoveArmor();
	}

	private void PlayWeaponAnimation()
	{
		if (_profile != null)
		{
			Animation component = _weapon.GetComponent<WeaponSounds>().animationObject.GetComponent<Animation>();
			if (Time.timeScale != 0f)
			{
				if (component.GetClip("Profile") == null)
				{
					component.AddClip(_profile, "Profile");
				}
				else
				{
					Debug.LogWarning("Animation clip is null.");
				}
				component.Play("Profile");
				return;
			}
			AnimationRunner.StopAllCoroutines();
			if (component.GetClip("Profile") == null)
			{
				component.AddClip(_profile, "Profile");
			}
			else
			{
				Debug.LogWarning("Animation clip is null.");
			}
			AnimationRunner.StartPlay(component, "Profile", false, null);
		}
		else
		{
			Debug.LogWarning("_profile == null");
		}
	}

	public static Texture2D GetClanLogo(string logoBase64)
	{
		if (string.IsNullOrEmpty(logoBase64))
		{
			return null;
		}
		byte[] data = Convert.FromBase64String(logoBase64);
		Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
		texture2D.LoadImage(data);
		texture2D.filterMode = FilterMode.Point;
		texture2D.Apply();
		return texture2D;
	}
}
