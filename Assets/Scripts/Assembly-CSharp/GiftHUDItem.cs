using System.Collections;
using I2.Loc;
using Rilisoft;
using UnityEngine;

public class GiftHUDItem : MonoBehaviour
{
	public bool isInfo;

	public UISprite sprIcon;

	public UITexture textureIcon;

	public UILabel nameGift;

	public GameObject parentForSkin;

	public BoxCollider colliderForDrag;

	public UILabel lbInfoGift;

	private Transform skinModelTransform;

	[SerializeField]
	[ReadOnly]
	private string nameAndCountGift = string.Empty;

	private Vector3 offsetSkin = new Vector3(0f, -44.12f, 0f);

	private Vector3 scaleSkin = new Vector3(45f, 45f, 45f);

	private bool endAnim;

	[SerializeField]
	private SlotInfo _data;

	private void OnEnable()
	{
		if (colliderForDrag == null)
		{
			colliderForDrag = GetComponent<BoxCollider>();
		}
		if (!isInfo)
		{
			StartCoroutine(ActiveSkinAfterWait());
		}
	}

	public void SetInfoButton(SlotInfo curInfo)
	{
		_data = curInfo;
		if (_data == null)
		{
			Debug.LogError("SetInfoButton");
			return;
		}
		if ((bool)sprIcon)
		{
			sprIcon.gameObject.SetActive(false);
		}
		if ((bool)textureIcon)
		{
			textureIcon.gameObject.SetActive(false);
		}
		if (skinModelTransform != null)
		{
			Object.Destroy(skinModelTransform.gameObject);
			skinModelTransform = null;
		}
		string text = ((_data.CountGift <= 1) ? string.Empty : (_data.CountGift + " "));
		switch (_data.category.Type)
		{
		case GiftCategoryType.Skins:
			nameAndCountGift = SkinsController.skinsNamesForPers[_data.gift.Id];
			break;
		case GiftCategoryType.Coins:
			nameAndCountGift = text + LocalizationStore.Get("Key_0275");
			break;
		case GiftCategoryType.Gems:
			nameAndCountGift = text + LocalizationStore.Get("Key_0951");
			break;
		case GiftCategoryType.Grenades:
		case GiftCategoryType.Gear:
		case GiftCategoryType.ArmorAndHat:
		case GiftCategoryType.Wear:
		case GiftCategoryType.Masks:
		case GiftCategoryType.Capes:
		case GiftCategoryType.Boots:
		case GiftCategoryType.Hats_random:
		case GiftCategoryType.Gun1:
		case GiftCategoryType.Gun2:
		case GiftCategoryType.Gun3:
		case GiftCategoryType.Gun4:
		case GiftCategoryType.Guns_gray:
		{
			string id = _data.gift.Id;
			ShopNGUIController.CategoryNames? typeShopCat = _data.gift.TypeShopCat;
			nameAndCountGift = text + RespawnWindowItemToBuy.GetItemName(id, (!typeShopCat.HasValue) ? ShopNGUIController.CategoryNames.ArmorCategory : typeShopCat.Value);
			break;
		}
		case GiftCategoryType.Editor:
			if (_data.gift.Id == "editor_Cape")
			{
				nameAndCountGift = LocalizationStore.Get("Key_0746");
			}
			else if (_data.gift.Id == "editor_Skin")
			{
				nameAndCountGift = LocalizationStore.Get("Key_0086");
			}
			else
			{
				Debug.LogError(string.Format("[GIFT] unknown gift id: '{0}'", _data.gift.Id));
			}
			break;
		case GiftCategoryType.Stickers:
			if (_data.gift.Id == "classic")
			{
				nameAndCountGift = LocalizationStore.Get("Key_1756");
			}
			else if (_data.gift.Id == "christmas")
			{
				nameAndCountGift = LocalizationStore.Get("Key_1758");
			}
			break;
		case GiftCategoryType.Freespins:
			nameAndCountGift = string.Format(LocalizationStore.Get("Key_2196"), _data.CountGift);
			break;
		case GiftCategoryType.WeaponSkin:
		{
			string lkey = WeaponSkinsManager.GetSkin(_data.gift.Id).Lkey;
			nameAndCountGift = text + LocalizationStore.Get(lkey);
			break;
		}
		case GiftCategoryType.Gadgets:
			if (GadgetsInfo.info.ContainsKey(_data.gift.Id))
			{
				GadgetInfo gadgetInfo = GadgetsInfo.info[_data.gift.Id];
				nameAndCountGift = text + ItemDb.GetItemName(gadgetInfo.Id, (ShopNGUIController.CategoryNames)gadgetInfo.Category);
			}
			else
			{
				Debug.LogErrorFormat("not found gadget: '{0}'", _data.gift.Id);
			}
			break;
		default:
			nameAndCountGift = text + LocalizationStore.Get(_data.gift.KeyTranslateInfo);
			break;
		case GiftCategoryType.Event_content:
			break;
		}
		if ((bool)nameGift)
		{
			nameGift.text = nameAndCountGift;
		}
		if (lbInfoGift != null)
		{
			if (_data.category.Type == GiftCategoryType.Gadgets)
			{
				if (GadgetsInfo.info.ContainsKey(_data.gift.Id))
				{
					GadgetInfo gadgetInfo2 = GadgetsInfo.info[_data.gift.Id];
					lbInfoGift.text = LocalizationStore.Get(gadgetInfo2.DescriptionLkey);
					lbInfoGift.gameObject.SetActive(true);
				}
				else
				{
					Debug.LogErrorFormat("not found gadget: '{0}'", _data.gift.Id);
				}
			}
			else if (!string.IsNullOrEmpty(_data.gift.KeyTranslateInfo))
			{
				lbInfoGift.text = ScriptLocalization.Get(_data.gift.KeyTranslateInfo);
				lbInfoGift.gameObject.SetActive(true);
			}
			else if (!string.IsNullOrEmpty(_data.category.KeyTranslateInfoCommon))
			{
				lbInfoGift.text = ScriptLocalization.Get(_data.category.KeyTranslateInfoCommon);
				lbInfoGift.gameObject.SetActive(true);
			}
			else
			{
				lbInfoGift.gameObject.SetActive(false);
			}
		}
		switch (_data.category.Type)
		{
		case GiftCategoryType.Skins:
			if ((bool)parentForSkin)
			{
				if (!isInfo)
				{
					parentForSkin.layer = LayerMask.NameToLayer("FriendsWindowGUI");
				}
				skinModelTransform = SkinsController.SkinModel(_data.gift.Id, 1, parentForSkin.transform, offsetSkin, scaleSkin);
			}
			break;
		case GiftCategoryType.Event_content:
			break;
		default:
			SetImage();
			break;
		}
	}

	private void SetImage()
	{
		Texture texture = null;
		string spriteName = null;
		switch (_data.category.Type)
		{
		case GiftCategoryType.Coins:
			texture = Resources.Load<Texture>("OfferIcons/Marathon/bonus_coins");
			break;
		case GiftCategoryType.Gems:
			texture = Resources.Load<Texture>("OfferIcons/Marathon/bonus_gems");
			break;
		case GiftCategoryType.Skins:
			return;
		case GiftCategoryType.Gear:
		{
			string text2 = string.Empty;
			if (_data.gift.Id.Equals("MusicBox"))
			{
				text2 = "Dater_bonus_turret";
			}
			if (_data.gift.Id.Equals("Wings"))
			{
				text2 = "Dater_bonus_jetpack";
			}
			if (_data.gift.Id.Equals("Bear"))
			{
				text2 = "Dater_bonus_mech";
			}
			if (_data.gift.Id.Equals("BigHeadPotion"))
			{
				text2 = "Dater_bonus_potion";
			}
			texture = Resources.Load<Texture>("OfferIcons/Marathon/" + text2);
			break;
		}
		case GiftCategoryType.Grenades:
		{
			string text = string.Empty;
			switch (_data.gift.Id)
			{
			case "GrenadeID":
				text = "Marathon/bonus_grenade";
				break;
			case "LikeID":
				text = "LikeID";
				break;
			}
			texture = Resources.Load<Texture>("OfferIcons/" + text);
			break;
		}
		case GiftCategoryType.ArmorAndHat:
		case GiftCategoryType.Wear:
		case GiftCategoryType.Masks:
		case GiftCategoryType.Capes:
		case GiftCategoryType.Boots:
		case GiftCategoryType.Hats_random:
		case GiftCategoryType.Gun1:
		case GiftCategoryType.Gun2:
		case GiftCategoryType.Gun3:
		case GiftCategoryType.Gun4:
		case GiftCategoryType.Guns_gray:
		{
			ShopNGUIController.CategoryNames category = ((!_data.gift.TypeShopCat.HasValue) ? ShopNGUIController.CategoryNames.ArmorCategory : _data.gift.TypeShopCat.Value);
			texture = ItemDb.GetItemIcon(_data.gift.Id, category);
			break;
		}
		case GiftCategoryType.Editor:
			if (_data.gift.Id == "editor_Cape")
			{
				texture = Resources.Load<Texture>("OfferIcons/editor_win_cape");
			}
			else if (_data.gift.Id == "editor_Skin")
			{
				texture = Resources.Load<Texture>("OfferIcons/editor_win_skin2");
			}
			else
			{
				Debug.LogError(string.Format("[GIFT] unknown gift id: '{0}'", _data.gift.Id));
			}
			break;
		case GiftCategoryType.Stickers:
			switch (_data.gift.Id.ToEnum<TypePackSticker>().Value)
			{
			case TypePackSticker.classic:
				texture = Resources.Load<Texture>("OfferIcons/free_smile");
				break;
			case TypePackSticker.christmas:
				texture = Resources.Load<Texture>("OfferIcons/free_christmas_smile");
				break;
			}
			break;
		case GiftCategoryType.Freespins:
			texture = Resources.Load<Texture>(string.Format("OfferIcons/free_spin_{0}", _data.gift.Count.Value));
			break;
		case GiftCategoryType.WeaponSkin:
			texture = ItemDb.GetItemIcon(_data.gift.Id, ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory);
			break;
		case GiftCategoryType.Gadgets:
			if (GadgetsInfo.info.ContainsKey(_data.gift.Id))
			{
				GadgetInfo gadgetInfo = GadgetsInfo.info[_data.gift.Id];
				texture = ItemDb.GetItemIcon(gadgetInfo.Id, (ShopNGUIController.CategoryNames)gadgetInfo.Category);
			}
			else
			{
				Debug.LogErrorFormat("not found gadget: '{0}'", _data.gift.Id);
			}
			break;
		}
		if (texture != null)
		{
			textureIcon.mainTexture = texture;
			textureIcon.gameObject.SetActive(true);
			sprIcon.gameObject.SetActive(false);
		}
		else
		{
			sprIcon.spriteName = spriteName;
			sprIcon.gameObject.SetActive(true);
			textureIcon.gameObject.SetActive(false);
		}
	}

	private IEnumerator ActiveSkinAfterWait()
	{
		while (skinModelTransform == null)
		{
			yield return null;
		}
		skinModelTransform.gameObject.SetActive(false);
		while (!(GiftBannerWindow.instance != null) || !GiftBannerWindow.instance.bannerObj.activeSelf)
		{
			yield return null;
		}
		yield return null;
		skinModelTransform.gameObject.SetActive(true);
	}

	public void InCenter(bool anim = false, int countBut = 1)
	{
		UIScrollView componentInParent = GetComponentInParent<UIScrollView>();
		if (componentInParent == null)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3[] worldCorners = componentInParent.panel.worldCorners;
		Vector3 position = (worldCorners[2] + worldCorners[0]) * 0.5f;
		if (!(transform != null) || !(componentInParent != null) || !(componentInParent.panel != null))
		{
			return;
		}
		Transform cachedTransform = componentInParent.panel.cachedTransform;
		GameObject gameObject = transform.gameObject;
		Vector3 vector = cachedTransform.InverseTransformPoint(transform.position);
		Vector3 vector2 = cachedTransform.InverseTransformPoint(position);
		Vector3 vector3 = vector - vector2;
		if (!componentInParent.canMoveHorizontally)
		{
			vector3.x = 0f;
		}
		if (!componentInParent.canMoveVertically)
		{
			vector3.y = 0f;
		}
		vector3.z = 0f;
		if (anim)
		{
			Vector3 offset = cachedTransform.localPosition - vector3;
			StartCoroutine(Crt_Anim_InCenter(componentInParent.panel.cachedGameObject, offset, countBut * 130));
			return;
		}
		Vector3 vector4 = Vector3.zero;
		if (componentInParent.transform.localPosition.Equals(cachedTransform.localPosition - vector3))
		{
			vector4 = new Vector3(1f, 0f, 0f);
		}
		SpringPanel.Begin(componentInParent.gameObject, cachedTransform.localPosition - vector3 + vector4, 10f);
	}

	private void FastCenter(UIScrollView scroll, Vector3 needPos)
	{
		float deltaTime = RealTime.deltaTime;
		Vector3 localPosition = scroll.transform.localPosition;
		scroll.transform.localPosition = needPos;
		Vector3 vector = needPos - localPosition;
		Vector2 clipOffset = scroll.panel.clipOffset;
		clipOffset.x -= vector.x;
		clipOffset.y -= vector.y;
		scroll.panel.clipOffset = clipOffset;
	}

	private IEnumerator Crt_Anim_InCenter(GameObject obj, Vector3 offset, float width)
	{
		StartCoroutine(Crt_TimerAnim());
		float speedAnim = 0f;
		Vector3 animOffset = new Vector3(width * 5f, 0f, 0f) + offset;
		while (!endAnim)
		{
			if (speedAnim < 1f)
			{
				speedAnim += 0.05f;
			}
			SpringPanel.Begin(obj, animOffset, speedAnim);
			yield return new WaitForEndOfFrame();
		}
		SpringPanel.Begin(obj, animOffset, 1f);
	}

	private IEnumerator Crt_TimerAnim()
	{
		endAnim = false;
		yield return new WaitForSeconds(1.5f);
		endAnim = true;
	}
}
