using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public sealed class PersConfigurator : MonoBehaviour
{
	public static PersConfigurator currentConfigurator;

	private CharacterInterface characterInterface;

	public GameObject gun;

	private GameObject weapon;

	private NickLabelController _label;

	private GameObject shadow;

	private AnimationClip profile;

	private void Awake()
	{
		currentConfigurator = this;
		GameObject original = Resources.Load("Character_model") as GameObject;
		characterInterface = Object.Instantiate(original).GetComponent<CharacterInterface>();
		characterInterface.transform.SetParent(base.transform, false);
		characterInterface.SetCharacterType(false, false, false);
		ShopNGUIController.DisableLightProbesRecursively(characterInterface.gameObject);
	}

	private IEnumerator Start()
	{
		ResetWeapon();
		SetCurrentSkin();
		ShopNGUIController.sharedShop.onEquipSkinAction = delegate
		{
			SetCurrentSkin();
		};
		yield return new WaitForEndOfFrame();
		UpdateWear();
		ShopNGUIController.sharedShop.wearEquipAction = delegate
		{
			UpdateWear();
		};
		ShopNGUIController.sharedShop.wearUnequipAction = delegate
		{
			UpdateWear();
		};
		ShopNGUIController.ShowArmorChanged += UpdateWear;
		ShopNGUIController.ShowWearChanged += UpdateWear;
		while (NickLabelStack.sharedStack == null)
		{
			yield return null;
		}
		NickLabelController.currentCamera = Camera.main;
	}

	public void ResetWeapon()
	{
		if (this.weapon != null)
		{
			Object.Destroy(this.weapon);
		}
		this.weapon = null;
		WeaponManager sharedManager = WeaponManager.sharedManager;
		int num = 0;
		GameObject gameObject = null;
		List<Weapon> list = new List<Weapon>();
		foreach (Weapon allAvailablePlayerWeapon in sharedManager.allAvailablePlayerWeapons)
		{
			if (WeaponManager.tagToStoreIDMapping.ContainsKey(ItemDb.GetByPrefabName(allAvailablePlayerWeapon.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag))
			{
				list.Add(allAvailablePlayerWeapon);
			}
		}
		if (list.Count == 0)
		{
			foreach (Weapon allAvailablePlayerWeapon2 in sharedManager.allAvailablePlayerWeapons)
			{
				if (allAvailablePlayerWeapon2.weaponPrefab.name.Replace("(Clone)", string.Empty) == WeaponManager.PistolWN)
				{
					gameObject = allAvailablePlayerWeapon2.weaponPrefab;
					break;
				}
			}
		}
		else
		{
			gameObject = list[Random.Range(0, list.Count)].weaponPrefab;
		}
		if (gameObject == null)
		{
			Debug.LogWarning("pref == null");
		}
		else
		{
			Debug.Log("ProfileAnims/" + gameObject.name + "_Profile");
			profile = Resources.Load<AnimationClip>("ProfileAnimClips/" + gameObject.name + "_Profile");
			GameObject gameObject2 = Object.Instantiate(gameObject);
			gameObject2.transform.parent = characterInterface.gunPoint.transform;
			weapon = gameObject2;
			weapon.transform.localPosition = Vector3.zero;
			weapon.transform.localRotation = Quaternion.identity;
			if (profile != null)
			{
				weapon.GetComponent<WeaponSounds>().animationObject.GetComponent<Animation>().AddClip(profile, "Profile");
				weapon.GetComponent<WeaponSounds>().animationObject.GetComponent<Animation>().Play("Profile");
			}
			gun = gameObject2.GetComponent<WeaponSounds>().bonusPrefab;
			WeaponSkin skinForWeapon = WeaponSkinsManager.GetSkinForWeapon(gameObject2.nameNoClone());
			if (skinForWeapon != null)
			{
				skinForWeapon.SetTo(gameObject2);
			}
			SetCurrentSkin();
		}
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		GameObject[] array2 = array;
		foreach (GameObject gameObject3 in array2)
		{
			if (gameObject3.name.Equals("GunFlash"))
			{
				gameObject3.SetActive(false);
			}
		}
		ResetArmor();
		ShopNGUIController.DisableLightProbesRecursively(characterInterface.gameObject);
	}

	private void SetCurrentSkin()
	{
		characterInterface.SetSkin(SkinsController.currentSkinForPers, (!(weapon != null)) ? null : weapon.GetComponent<WeaponSounds>());
	}

	public void UpdateWear()
	{
		string @string = Storager.getString(Defs.CapeEquppedSN, false);
		characterInterface.UpdateCape(@string, null, !ShopNGUIController.ShowWear);
		string string2 = Storager.getString("MaskEquippedSN", false);
		characterInterface.UpdateMask(string2, !ShopNGUIController.ShowWear);
		string text = Storager.getString(Defs.HatEquppedSN, false);
		string string3 = Storager.getString(Defs.VisualHatArmor, false);
		if (!string.IsNullOrEmpty(string3) && Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(text) >= 0 && Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(text) < Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(string3))
		{
			text = string3;
		}
		characterInterface.UpdateHat(text, !ShopNGUIController.ShowWear);
		string string4 = Storager.getString(Defs.BootsEquppedSN, false);
		characterInterface.UpdateBoots(string4, !ShopNGUIController.ShowWear);
		ShopNGUIController.SetPersHatVisible(characterInterface.hatPoint);
		ResetArmor();
	}

	private void ResetArmor()
	{
		string text = Storager.getString(Defs.ArmorNewEquppedSN, false);
		string @string = Storager.getString(Defs.VisualArmor, false);
		if (!string.IsNullOrEmpty(@string) && Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(text) >= 0 && Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(text) < Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(@string))
		{
			text = @string;
		}
		characterInterface.UpdateArmor(text, weapon);
		ShopNGUIController.SetPersArmorVisible(characterInterface.armorPoint);
	}

	private void Update()
	{
		if (!(Camera.main != null))
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; i++)
		{
			RaycastHit hitInfo;
			if (Input.GetTouch(i).phase == TouchPhase.Began && Physics.Raycast(ray, out hitInfo, 1000f, -5) && hitInfo.collider.gameObject.name.Equals("MainMenu_Pers"))
			{
				PlayerPrefs.SetInt(Defs.ProfileEnteredFromMenu, 1);
				ConnectSceneNGUIController.GoToProfile();
				break;
			}
		}
	}

	private void OnDestroy()
	{
		if (ShopNGUIController.sharedShop != null)
		{
			ShopNGUIController.sharedShop.onEquipSkinAction = null;
			ShopNGUIController.sharedShop.wearEquipAction = null;
			ShopNGUIController.sharedShop.wearUnequipAction = null;
		}
		if (profile != null)
		{
			Resources.UnloadAsset(profile);
		}
		ShopNGUIController.ShowArmorChanged -= UpdateWear;
		ShopNGUIController.ShowWearChanged -= UpdateWear;
		currentConfigurator = null;
	}
}
