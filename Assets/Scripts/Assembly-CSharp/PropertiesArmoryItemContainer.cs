using System.Collections.Generic;
using UnityEngine;

public class PropertiesArmoryItemContainer : MonoBehaviour
{
	public GameObject needTierPet;

	public UILabel needTierPetLabel;

	public GameObject needMoreTrophiesPanel;

	public GameObject renamePetButton;

	public UITable specialTable;

	public UITable gadgetPropertyTable;

	public GameObject weaponProperties;

	public GameObject meleeProperties;

	public GameObject specialParams;

	public GameObject nonArmorWearProperties;

	public GameObject armorWearProperties;

	public GameObject skinProperties;

	public GameObject gadgetProperties;

	public UILabel fireRate;

	public UILabel fireRateMElee;

	public UILabel mobility;

	public UILabel mobilityMelee;

	public UILabel capacity;

	public UILabel damage;

	public UILabel damageMelee;

	public UILabel weaponsRarityLabel;

	public UILabel descriptionGadget;

	public UIButton upgradeButton;

	public UIButton trainButton;

	public UIButton buyButton;

	public UIButton equipButton;

	public UIButton unequipButton;

	public UIButton infoButton;

	public UIButton editButton;

	public UIButton deleteButton;

	public UIButton enableButton;

	public UIButton createButton;

	public UIButton tryGun;

	public GameObject equipped;

	public GameObject needTier;

	public UILabel needTierLabel;

	public GameObject needBuyPrevious;

	public UILabel needBuyPreviousLabel;

	public List<UISprite> effectsSprites;

	public List<UILabel> effectsLabels;

	public List<GadgetPropertyItem> gadgetsPropertiesList;

	public UILabel nonArmorWearDEscription;

	public UILabel armorWearDescription;

	public UILabel armorCountLabel;

	public UILabel gadgetNameLabel;

	public GameObject tryGunPanel;

	public UILabel tryGunMatchesCount;

	public UILabel tryGunDiscountTime;

	public PriceContainer price;

	public GameObject discountPanel;

	public UILabel discountLabel;
}
