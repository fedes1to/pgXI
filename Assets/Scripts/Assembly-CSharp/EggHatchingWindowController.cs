using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Rilisoft;
using UnityEngine;

public class EggHatchingWindowController : GeneralBannerWindow
{
	internal enum WindowMode
	{
		Hatching,
		Rename
	}

	public Camera petCamera;

	public CharacterViewRotator petUpgradeRotationCollider;

	public CharacterViewRotator petAddedRotationCollider;

	public Transform _3dModelsParent;

	public Transform egg;

	public GameObject getPetButton;

	public GameObject confirmButton;

	public GameObject initialState;

	public GameObject newPetState;

	public GameObject upgradePetState;

	public GameObject hatchButton;

	public List<UILabel> currentGrade;

	public List<UILabel> nextGrade;

	public List<UILabel> points;

	public List<UILabel> rarity;

	public List<UILabel> petNameUpgrade;

	public List<UILabel> maximumLevel;

	public UISprite oldUpgradeIndicator;

	public UISprite newUpgradeIndicator;

	public UILabel specialPrizeCoinCount;

	public GameObject noUpgrade;

	public GameObject upgradeAvailable;

	public GameObject specialPrize;

	public UIInput petsNameInput;

	public UISprite darkenSprite;

	private bool isClosing;

	public AnimationCoroutineRunner petProfileAnimationRunner;

	[Header("panel upgrade settings")]
	public GameObject btnClose;

	public GameObject btnUpgrade;

	public float upgradeBtnCloseOffset = 175f;

	public TextGroup _upgradePriceText;

	public UISprite _upgradePriceGoldSprite;

	public UISprite _upgradePriceGemSprite;

	public GameObject upgradeParticles;

	public UITweener tweenHatcWindow;

	public UITweener tweenUpgradeWindow;

	private int touchesToHatch = 3;

	private bool m_shouldEquipPet;

	private readonly Vector3 EGG_HIDE_OFFSET = new Vector3(-10000f, 0f, 0f);

	private WindowMode m_windowMode;

	private int m_moveToSecondState;

	private Egg m_eggForHatching;

	private string m_petId = string.Empty;

	internal WindowMode CurrentWindowMode
	{
		get
		{
			return m_windowMode;
		}
	}

	public Egg EggForHatching
	{
		get
		{
			return m_eggForHatching;
		}
		set
		{
			m_eggForHatching = value;
			if (_3dModelsParent != null && m_eggForHatching != null)
			{
				try
				{
					Player_move_c.SetTextureRecursivelyFrom(_3dModelsParent.gameObject, Resources.Load<Texture>(m_eggForHatching.GetRelativeMeshTexturePath()), new GameObject[0]);
					egg.localPosition += EGG_HIDE_OFFSET;
					CoroutineRunner.Instance.StartCoroutine(ReturnEggToIsPosition());
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Exception in setting egg's texture: {0}", ex);
				}
			}
		}
	}

	private string HatchedPetName { get; set; }

	protected bool IsPetOfMaxUpgrade
	{
		get
		{
			//Discarded unreachable code: IL_0043
			if (m_petId.IsNullOrEmpty())
			{
				return false;
			}
			try
			{
				return Singleton<PetsManager>.Instance.GetAllUpgrades(m_petId).Length == PetsInfo.info[m_petId].Up + 1;
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in IsPetOfMaxUpgrade: {0}", ex);
			}
			return false;
		}
	}

	private bool ShouldEquipPet
	{
		get
		{
			return m_shouldEquipPet;
		}
		set
		{
			m_shouldEquipPet = value;
		}
	}

	public void SetRenameMode()
	{
		m_windowMode = WindowMode.Rename;
		Destroy3dModels();
		initialState.SetActiveSafeSelf(false);
		upgradePetState.SetActiveSafeSelf(false);
		petUpgradeRotationCollider.gameObject.SetActiveSafeSelf(false);
		newPetState.SetActiveSafeSelf(true);
		petAddedRotationCollider.gameObject.SetActiveSafeSelf(true);
		newPetState.GetComponent<UIPanel>().alpha = 1f;
		HatchingEndedCallback.HatchingEnded -= HatchingEndedCallback_HatchingEnded;
		getPetButton.SetActiveSafeSelf(false);
		confirmButton.SetActiveSafeSelf(true);
	}

	public void TouchEgg()
	{
		if (hatchButton.activeSelf)
		{
			touchesToHatch--;
			if (touchesToHatch == 0)
			{
				tweenHatcWindow.ResetToBeginning();
				tweenHatcWindow.PlayForward();
				HandleHatchButton();
			}
			else if (touchesToHatch > 0)
			{
				egg.GetComponent<Animator>().SetTrigger("Touch");
				tweenHatcWindow.ResetToBeginning();
				tweenHatcWindow.PlayForward();
			}
		}
	}

	public void SetPetId(string petId)
	{
		m_petId = petId;
	}

	public Transform ReplaceEggWithPet(string petId)
	{
		Transform transform = _3dModelsParent;
		PetEngine petEngine = Resources.Load<PetEngine>(Singleton<PetsManager>.Instance.GetRelativePrefabPath(petId));
		Transform component = UnityEngine.Object.Instantiate(petEngine.Model).GetComponent<Transform>();
		component.parent = transform;
		component.localPosition = Singleton<PetsManager>.Instance.GetInfo(petId).PositionInBanners;
		component.localRotation = Quaternion.Euler(Singleton<PetsManager>.Instance.GetInfo(petId).RotationInBanners);
		component.localScale = Vector3.one;
		Tools.SetLayerRecursively(component.gameObject, transform.gameObject.layer);
		PlayPetAnimation(component.gameObject);
		petAddedRotationCollider.characterView = component;
		petAddedRotationCollider.SetDefaultRotationFromCharacterView();
		petUpgradeRotationCollider.characterView = component;
		petUpgradeRotationCollider.SetDefaultRotationFromCharacterView();
		ShopNGUIController.DisableLightProbesRecursively(component.gameObject);
		return component;
	}

	public void SetPetsNameToInput(string petsName)
	{
		petsNameInput.value = petsName;
	}

	public virtual void HandleHatchButton()
	{
		hatchButton.SetActiveSafeSelf(false);
		Animator componentInChildren = _3dModelsParent.GetComponentInChildren<Animator>(true);
		componentInChildren.SetTrigger("Open");
		float num = 1.22f;
		float p_delay = 2f;
		try
		{
			num = componentInChildren.runtimeAnimatorController.animationClips.FirstOrDefault((AnimationClip clip) => clip.name == "Open").length / 4f;
			p_delay = componentInChildren.runtimeAnimatorController.animationClips.FirstOrDefault((AnimationClip clip) => clip.name == "Open").length - num * 2f;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in getting eggOpenDuraition: {0}", ex);
		}
		try
		{
			Renderer eggRenderer = componentInChildren.GetComponent<HatchingEggRefsHolder>().eggRenderer;
			eggRenderer.material.color = new Color(1f, 1f, 1f, 1f);
			TweenParms p_parms = new TweenParms().Prop("color", new PlugSetColor(new Color(1f, 1f, 1f, 0f)).Property("_Color")).Ease(EaseType.Linear).UpdateType(UpdateType.TimeScaleIndependentUpdate)
				.Delay(p_delay);
			HOTween.To(eggRenderer.material, num, p_parms);
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("Exception in starting fading out egg: {0}", ex2);
		}
		string petId = Singleton<EggsManager>.Instance.Use(EggForHatching);
		AnalyticsStuff.LogHatching(petId, EggForHatching);
		PetUpdateInfo petUpdateInfo = Singleton<PetsManager>.Instance.AddOrUpdatePet(petId);
		string infoId = petUpdateInfo.PetNew.InfoId;
		try
		{
			ShouldEquipPet = Singleton<PetsManager>.Instance.PlayerPets.Count == 1 && petUpdateInfo.PetAdded;
		}
		catch (Exception ex3)
		{
			Debug.LogErrorFormat("Exception in calculating shouldEquipPet: {0}", ex3);
		}
		SetPetId(infoId);
		PetInfo info = petUpdateInfo.PetNew.Info;
		Transform transform = ReplaceEggWithPet(info.Id);
		transform.localScale = Vector3.zero;
		TweenParms p_parms2 = new TweenParms().Prop("localScale", Vector3.one).Ease(EaseType.Linear).UpdateType(UpdateType.TimeScaleIndependentUpdate)
			.Delay(p_delay)
			.OnComplete((TweenDelegate.TweenCallback)delegate
			{
			});
		HOTween.To(transform, num / 2f, p_parms2);
		try
		{
			if (petUpdateInfo.PetAdded)
			{
				petAddedRotationCollider.gameObject.SetActiveSafeSelf(true);
				petUpgradeRotationCollider.gameObject.SetActiveSafeSelf(false);
				string petsNameToInput = LocalizationStore.Get(info.Lkey);
				SetPetsNameToInput(petsNameToInput);
			}
			else
			{
				petAddedRotationCollider.gameObject.SetActiveSafeSelf(false);
				petUpgradeRotationCollider.gameObject.SetActiveSafeSelf(true);
				bool isPetOfMaxUpgrade = IsPetOfMaxUpgrade;
				foreach (UILabel item in petNameUpgrade)
				{
					item.text = string.Format(LocalizationStore.Get("Key_2596"), LocalizationStore.Get(info.Lkey));
				}
				foreach (UILabel item2 in rarity)
				{
					item2.text = ItemDb.GetItemRarityLocalizeName(info.Rarity);
				}
				if (!isPetOfMaxUpgrade)
				{
					foreach (UILabel item3 in currentGrade)
					{
						item3.text = string.Format(LocalizationStore.Get("Key_2496"), info.Up + 1);
					}
					foreach (UILabel item4 in nextGrade)
					{
						item4.text = string.Format(LocalizationStore.Get("Key_2496"), info.Up + 2);
					}
				}
				else
				{
					currentGrade.ForEach(delegate(UILabel lab)
					{
						lab.gameObject.SetActiveSafeSelf(false);
					});
					nextGrade.ForEach(delegate(UILabel lab)
					{
						lab.gameObject.SetActiveSafeSelf(false);
					});
				}
				int num2 = petUpdateInfo.PetNew.Points;
				int num3 = PointsToNextUpOfPet();
				if (!isPetOfMaxUpgrade)
				{
					foreach (UILabel point in points)
					{
						point.text = string.Format("{0} {1} / {2}", LocalizationStore.Get("Key_2793"), num2, num3);
					}
				}
				else
				{
					points.ForEach(delegate(UILabel lab)
					{
						lab.gameObject.SetActiveSafeSelf(false);
					});
				}
				if (!isPetOfMaxUpgrade)
				{
					float nextUpPointsSafe = GetNextUpPointsSafe(num3);
					oldUpgradeIndicator.fillAmount = (float)Mathf.Min(petUpdateInfo.PetOld.Points, num3) / nextUpPointsSafe;
					newUpgradeIndicator.fillAmount = (float)Mathf.Min(petUpdateInfo.PetOld.Points, num3) / nextUpPointsSafe;
				}
				else
				{
					oldUpgradeIndicator.fillAmount = 1f;
					newUpgradeIndicator.fillAmount = 0f;
				}
				noUpgrade.SetActiveSafeSelf(num2 < num3 && !isPetOfMaxUpgrade);
				upgradeAvailable.SetActiveSafeSelf(num2 >= num3 && !isPetOfMaxUpgrade);
				specialPrize.SetActiveSafeSelf(isPetOfMaxUpgrade);
				if (isPetOfMaxUpgrade)
				{
					int count = CoinsForPetMaxUpggrade(m_petId);
					BankController.AddCoins(count);
					specialPrizeCoinCount.text = count.ToString();
				}
				maximumLevel.ForEach(delegate(UILabel lab)
				{
					lab.gameObject.SetActiveSafeSelf(isPetOfMaxUpgrade);
				});
				AnimateUpgradeIndicator();
			}
		}
		catch (Exception ex4)
		{
			Debug.LogErrorFormat("Exception in HandleHatchButton in hatching banner: {0}", ex4);
		}
		TweenParms p_parms3 = new TweenParms().Prop("alpha", 1f).Ease(EaseType.Linear).UpdateType(UpdateType.TimeScaleIndependentUpdate)
			.Delay(p_delay)
			.OnComplete((TweenDelegate.TweenCallback)delegate
			{
			});
		UIPanel component = ((!petUpdateInfo.PetAdded) ? upgradePetState : newPetState).GetComponent<UIPanel>();
		HOTween.To(component, 0.4f, p_parms3);
	}

	public void SavePetsName()
	{
		try
		{
			if (m_petId.IsNullOrEmpty())
			{
				Debug.LogErrorFormat("SavePetsName m_petId.IsNullOrEmpty()");
				return;
			}
			bool flag = newPetState.activeInHierarchy && newPetState.GetComponent<UIPanel>().alpha > 0.05f;
			if (m_windowMode == WindowMode.Rename || flag)
			{
				Singleton<PetsManager>.Instance.SetPetName(m_petId, petsNameInput.value ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in setting pets name after hatching: {0}", ex);
		}
	}

	public override void HandleClose()
	{
		EquipPetIfNeeded();
		if (!isClosing)
		{
			isClosing = true;
			SavePetsName();
			EquipPetIfNeeded();
			TweenParms p_parms = new TweenParms().Prop("alpha", 0f).Ease(EaseType.EaseInCubic).UpdateType(UpdateType.TimeScaleIndependentUpdate)
				.OnComplete((TweenDelegate.TweenCallback)delegate
				{
				});
			HOTween.To(darkenSprite, 0.1f, p_parms);
			TweenParms p_parms2 = new TweenParms().Prop("localScale", Vector3.one * 0.02f).Ease(EaseType.EaseInCubic).UpdateType(UpdateType.TimeScaleIndependentUpdate)
				.Delay(0.1f)
				.OnComplete((TweenDelegate.TweenCallback)delegate
				{
					base.HandleClose();
				});
			HOTween.To(base.transform, 0.2f, p_parms2);
			if (CurrentWindowMode != WindowMode.Rename)
			{
				ShopNGUIController.sharedShop.UpdatePetsCategoryIfNeeded();
			}
		}
	}

	public void HandleDArkBackgroundPressed()
	{
		OnHardwareBackPressed();
	}

	protected override void OnHardwareBackPressed()
	{
		if (initialState.activeInHierarchy)
		{
			if (hatchButton.activeInHierarchy)
			{
				HandleHatchButton();
			}
		}
		else
		{
			HandleClose();
		}
	}

	private int PointsToNextUpOfPet()
	{
		if (m_petId.IsNullOrEmpty())
		{
			return 999999;
		}
		PetInfo info = Singleton<PetsManager>.Instance.GetInfo(m_petId);
		return (info == null) ? 999999 : info.ToUpPoints;
	}

	private static float GetNextUpPointsSafe(int nextUpPoints)
	{
		return (nextUpPoints == 0) ? 1 : nextUpPoints;
	}

	private void Start()
	{
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager(true);
		RegisterEscapeHandler();
		HatchingEndedCallback.HatchingEnded += HatchingEndedCallback_HatchingEnded;
		btnUpgrade.GetComponent<ButtonHandler>().Clicked += OnUpgradeButtonClicked;
	}

	private void HatchingEndedCallback_HatchingEnded()
	{
		m_moveToSecondState = 5;
	}

	private void OnUpgradeButtonClicked(object sender, EventArgs eventArgs)
	{
		if (ShopNGUIController.sharedShop == null || m_petId.IsNullOrEmpty())
		{
			return;
		}
		ItemPrice itemPrice = ShopNGUIController.GetItemPrice(m_petId, ShopNGUIController.CategoryNames.PetsCategory);
		if (itemPrice == null)
		{
			return;
		}
		ShopNGUIController.TryToBuy(ShopNGUIController.sharedShop.mainPanel, itemPrice, OnBuyPetSuccess, OnBuyPetFailure, null, delegate
		{
			ShopNGUIController.sharedShop.PlayPersAnimations();
		}, delegate
		{
			ShopNGUIController.SetBankCamerasEnabled();
		}, delegate
		{
			ShopNGUIController.sharedShop.ShowGridOrArmorCarousel();
			ShopNGUIController.sharedShop.SetOtherCamerasEnabled(false);
			if (this != null)
			{
				EnablePetCamera();
			}
		});
	}

	private void OnBuyPetSuccess()
	{
		if (m_petId.IsNullOrEmpty())
		{
			return;
		}
		ShopNGUIController.ProvideItem(ShopNGUIController.CategoryNames.PetsCategory, m_petId);
		ShopNGUIController.sharedShop.UpdatePetsCategoryIfNeeded();
		string itemNameNonLocalized = ItemDb.GetItemNameNonLocalized(m_petId, m_petId, ShopNGUIController.CategoryNames.PetsCategory);
		string categoryParameterName = AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.PetsCategory) ?? ShopNGUIController.CategoryNames.PetsCategory.ToString();
		AnalyticsStuff.LogSales(itemNameNonLocalized, categoryParameterName);
		ItemPrice itemPrice = ShopNGUIController.GetItemPrice(m_petId, ShopNGUIController.CategoryNames.PetsCategory);
		AnalyticsFacade.InAppPurchase(itemNameNonLocalized, AnalyticsStuff.AnalyticsReadableCategoryNameFromOldCategoryName(categoryParameterName), 1, itemPrice.Price, itemPrice.Currency);
		btnUpgrade.GetComponent<Collider>().enabled = false;
		btnClose.GetComponent<Collider>().enabled = false;
		upgradeParticles.SetActive(true);
		tweenUpgradeWindow.ResetToBeginning();
		tweenUpgradeWindow.PlayForward();
		CoroutineRunner.DeferredAction(1.4f, delegate
		{
			PlayerPet playerPet = Singleton<PetsManager>.Instance.GetPlayerPet(m_petId);
			if (playerPet != null)
			{
				ShopNGUIController.sharedShop.ChooseCategoryAndSuperCategory(ShopNGUIController.CategoryNames.PetsCategory, new ShopNGUIController.ShopItem(playerPet.InfoId, ShopNGUIController.CategoryNames.PetsCategory), false);
			}
			HandleClose();
		});
	}

	private void OnBuyPetFailure()
	{
		Debug.Log(">>>> OnBuyPetFailure");
	}

	private void Destroy3dModels()
	{
		_3dModelsParent.DestroyChildren();
	}

	private void AnimateUpgradeIndicator()
	{
		if (IsPetOfMaxUpgrade)
		{
			return;
		}
		btnClose.SetActiveSafe(false);
		btnUpgrade.SetActive(false);
		PlayerPet pet = Singleton<PetsManager>.Instance.GetPlayerPet(m_petId);
		int num = PointsToNextUpOfPet();
		float num2 = (float)Mathf.Min(pet.Points, num) / GetNextUpPointsSafe(num);
		if (newUpgradeIndicator.fillAmount < 1f)
		{
			TweenParms p_parms = new TweenParms().Prop("fillAmount", num2).Ease(EaseType.Linear).UpdateType(UpdateType.TimeScaleIndependentUpdate)
				.Delay(2f)
				.OnComplete((TweenDelegate.TweenCallback)delegate
				{
					SetUpgradeWindowButtonsState(pet);
				});
			HOTween.To(newUpgradeIndicator, 0.7f, p_parms);
		}
		else
		{
			SetUpgradeWindowButtonsState(pet);
		}
	}

	private void SetUpgradeWindowButtonsState(PlayerPet currentPet)
	{
		int num = PointsToNextUpOfPet();
		PetInfo nextUp = Singleton<PetsManager>.Instance.GetNextUp(currentPet.InfoId);
		if ((float)Mathf.Min(currentPet.Points, num) >= GetNextUpPointsSafe(num) && nextUp != null && ExpController.OurTierForAnyPlace() >= nextUp.Tier)
		{
			btnClose.SetActiveSafe(true);
			btnUpgrade.SetActive(true);
			btnClose.transform.localPosition = new Vector3(upgradeBtnCloseOffset, btnClose.transform.localPosition.y, btnClose.transform.localPosition.z);
			ItemPrice itemPrice = ShopNGUIController.GetItemPrice(nextUp.Id, ShopNGUIController.CategoryNames.PetsCategory);
			_upgradePriceText.Text = itemPrice.Price.ToString();
			bool flag = itemPrice.Currency == "Coins";
			_upgradePriceGoldSprite.gameObject.SetActiveSafe(flag);
			_upgradePriceGemSprite.gameObject.SetActiveSafe(!flag);
		}
		else
		{
			btnClose.SetActiveSafe(true);
			btnUpgrade.SetActive(false);
			btnClose.transform.localPosition = new Vector3(0f, btnClose.transform.localPosition.y, btnClose.transform.localPosition.z);
		}
	}

	private void MoveToSecondState()
	{
		initialState.SetActiveSafeSelf(false);
		egg.parent = null;
		UnityEngine.Object.Destroy(egg.gameObject);
	}

	private void OnDestroy()
	{
		HatchingEndedCallback.HatchingEnded -= HatchingEndedCallback_HatchingEnded;
		UnregisterEscapeHandler();
	}

	private void EnablePetCamera()
	{
		if (petCamera != null && !petCamera.enabled)
		{
			petCamera.enabled = true;
		}
	}

	private void Update()
	{
		if (m_moveToSecondState > 0)
		{
			if (m_moveToSecondState == 1)
			{
				MoveToSecondState();
			}
			m_moveToSecondState--;
		}
		EnablePetCamera();
	}

	private void PlayPetAnimation(GameObject objectWithAnimation)
	{
		if (objectWithAnimation == null)
		{
			return;
		}
		try
		{
			if (!ShopNGUIController.IsModeWithNormalTimeScaleInShop())
			{
				Animation component = objectWithAnimation.GetComponent<Animation>();
				petProfileAnimationRunner.StopAllCoroutines();
				if (component.GetClip("Profile") == null)
				{
					Debug.LogErrorFormat("Error: pet {0} has no Profile animation clip (EggHatchingWindowController)", objectWithAnimation.nameNoClone());
				}
				else if (petProfileAnimationRunner.gameObject.activeInHierarchy)
				{
					petProfileAnimationRunner.StartPlay(component, "Profile", false, null);
				}
				else
				{
					Debug.LogErrorFormat("Coroutine couldn't be started because the the game object 'AnimationCoroutineRunner' is inactive! (Pet)");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in PlayPetAnimation: {0}", ex);
		}
	}

	private IEnumerator ReturnEggToIsPosition()
	{
		yield return null;
		egg.localPosition -= EGG_HIDE_OFFSET;
	}

	private int CoinsForPetMaxUpggrade(string petId)
	{
		try
		{
			int value;
			if (BalanceController.cashbackPets.TryGetValue(petId, out value))
			{
				return value;
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in CoinsForPetMaxUpggrade: {0}", ex);
		}
		return 5;
	}

	private void EquipPetIfNeeded()
	{
		if (m_petId == null)
		{
			Debug.LogErrorFormat("EquipPetIfNeeded: m_petId == null");
			ShouldEquipPet = false;
			return;
		}
		if (ShouldEquipPet)
		{
			try
			{
				ShopNGUIController.sharedShop.EquipPetAndUpdate(m_petId);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in equipping pet in EggHatchingWindowController: {0}", ex);
			}
		}
		ShouldEquipPet = false;
	}
}
