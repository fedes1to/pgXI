using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public class ChooseGadgetPanel : MonoBehaviour
{
	private enum HintState
	{
		ShouldNotShow,
		RunningFirstTimer,
		SHowingFirstTip,
		ShowingSecondTip,
		Finished
	}

	private const float DELAY_FIRST_TIP = 3f;

	private const float TIME_SHOWING_FIRST_TIP = 15f;

	private const string GADGET_PANEL_HINTS_SHOWN_KEY = "ChooseGadgetPanel.GADGET_PANEL_HINTS_SHOWN_KEY";

	public GameObject swipeToOpenHint;

	public GameObject chooseGadgetHint;

	public List<Transform> objectsNoFilpXScale;

	public GameObject hoverBackground1;

	public GameObject hoverBackground2;

	public GameObject hoverBackground3;

	private bool isGadgetReady = true;

	private int entrieCategory = -1;

	private GadgetButton _cachedGadgetButton;

	private List<GadgetPanelEntry> _entries = new List<GadgetPanelEntry>();

	private HintState m_hintState;

	private float m_delayShowFirstHintStartTime;

	private bool m_hintsShown;

	public GadgetButton gadgetButtonScript
	{
		get
		{
			if (_cachedGadgetButton == null)
			{
				_cachedGadgetButton = GetComponent<GadgetButton>();
			}
			return _cachedGadgetButton;
		}
	}

	public static event Action OnDisablePanel;

	private void Awake()
	{
		LoadDefaultGadget();
		InGameGadgetSet.Renew();
		UpdatePanel();
		ShopNGUIController.EquippedGadget += ShopNGUIController_EquippedGadget;
		try
		{
			swipeToOpenHint.SetActiveSafeSelf(false);
			chooseGadgetHint.SetActiveSafeSelf(false);
			m_hintsShown = Storager.getInt("ChooseGadgetPanel.GADGET_PANEL_HINTS_SHOWN_KEY", false) == 1;
			if (!m_hintsShown && InGameGadgetSet.CurrentSet.Count > 1)
			{
				m_hintState = HintState.RunningFirstTimer;
				m_delayShowFirstHintStartTime = Time.time;
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in initializing gadget hints: {0}", ex);
		}
	}

	private void Update()
	{
		if (m_hintState == HintState.RunningFirstTimer)
		{
			if (Time.time - 3f >= m_delayShowFirstHintStartTime)
			{
				m_hintState = HintState.SHowingFirstTip;
				m_hintsShown = true;
				swipeToOpenHint.SetActiveSafeSelf(true);
				chooseGadgetHint.SetActiveSafeSelf(false);
			}
		}
		else if (m_hintState == HintState.SHowingFirstTip && Time.time - 15f - 3f >= m_delayShowFirstHintStartTime)
		{
			m_hintState = HintState.Finished;
			swipeToOpenHint.SetActiveSafeSelf(false);
			chooseGadgetHint.SetActiveSafeSelf(false);
		}
		try
		{
			if (_entries != null)
			{
				if (_entries.Count > 0)
				{
					gadgetButtonScript.duration.fillAmount = InGameGadgetSet.CurrentSet[_entries[0].Info.Category].ExpirationProgress;
					gadgetButtonScript.cooldown.fillAmount = InGameGadgetSet.CurrentSet[_entries[0].Info.Category].CooldownProgress;
					bool canUse = InGameGadgetSet.CurrentSet[_entries[0].Info.Category].CanUse;
					if (entrieCategory != -1 && entrieCategory != (int)_entries[0].Info.Category)
					{
						isGadgetReady = canUse;
					}
					if (canUse && !isGadgetReady)
					{
						isGadgetReady = true;
						gadgetButtonScript.cooldownEnds.GetComponent<UITweener>().ResetToBeginning();
						gadgetButtonScript.cooldownEnds.GetComponent<UITweener>().PlayForward();
						if (Defs.isSoundFX)
						{
							gadgetButtonScript.cooldownEnds.GetComponent<AudioSource>().Play();
						}
					}
					isGadgetReady = canUse;
					entrieCategory = (int)_entries[0].Info.Category;
				}
				if (_entries.Count > 1)
				{
					gadgetButtonScript.duration1.fillAmount = InGameGadgetSet.CurrentSet[_entries[1].Info.Category].ExpirationProgress;
					gadgetButtonScript.cooldown1.fillAmount = InGameGadgetSet.CurrentSet[_entries[1].Info.Category].CooldownProgress;
				}
				if (_entries.Count > 2)
				{
					gadgetButtonScript.duration2.fillAmount = InGameGadgetSet.CurrentSet[_entries[2].Info.Category].ExpirationProgress;
					gadgetButtonScript.cooldown2.fillAmount = InGameGadgetSet.CurrentSet[_entries[2].Info.Category].CooldownProgress;
				}
			}
			gadgetButtonScript.yazichok.SetActiveSafeSelf(CanExtend() && WeaponManager.sharedManager.myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC.canUseGadgets);
		}
		catch (Exception ex)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogErrorFormat("Exception in ChooseGadgetPanel.Update: {0}", ex);
			}
		}
	}

	private void ShopNGUIController_EquippedGadget(string arg1, string arg2, GadgetInfo.GadgetCategory arg3)
	{
		UpdatePanel();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			SaveDefaultGadget();
			SaveHintsShown();
		}
	}

	private void OnDisable()
	{
		Action onDisablePanel = ChooseGadgetPanel.OnDisablePanel;
		if (onDisablePanel != null)
		{
			onDisablePanel();
		}
	}

	private void OnDestroy()
	{
		SaveDefaultGadget();
		ShopNGUIController.EquippedGadget -= ShopNGUIController_EquippedGadget;
		SaveHintsShown();
	}

	private void SaveHintsShown()
	{
		if (m_hintsShown && Storager.getInt("ChooseGadgetPanel.GADGET_PANEL_HINTS_SHOWN_KEY", false) == 0)
		{
			Storager.setInt("ChooseGadgetPanel.GADGET_PANEL_HINTS_SHOWN_KEY", 1, false);
		}
	}

	public void Show()
	{
		if (m_hintState == HintState.RunningFirstTimer || m_hintState == HintState.SHowingFirstTip)
		{
			m_hintState = HintState.ShowingSecondTip;
			m_hintsShown = true;
			swipeToOpenHint.SetActiveSafeSelf(false);
			chooseGadgetHint.SetActiveSafeSelf(true);
		}
		gadgetButtonScript.OpenGadgetPanel(true);
	}

	public void Hide()
	{
		if (m_hintState == HintState.ShowingSecondTip)
		{
			m_hintState = HintState.Finished;
			m_hintsShown = true;
			swipeToOpenHint.SetActiveSafeSelf(false);
			chooseGadgetHint.SetActiveSafeSelf(false);
		}
		gadgetButtonScript.OpenGadgetPanel(false);
	}

	public void ChooseDefault()
	{
	}

	public void ChooseFirst()
	{
		if (_entries.Count > 1)
		{
			GadgetsInfo.DefaultGadget = _entries[1].Info.Category;
			UpdatePanel();
		}
	}

	public void ChooseSecond()
	{
		if (_entries.Count > 2)
		{
			GadgetsInfo.DefaultGadget = _entries[2].Info.Category;
			UpdatePanel();
		}
	}

	public bool CanExtend()
	{
		return _entries.Count > 1;
	}

	private void UpdatePanel()
	{
		GadgetPanelEntry[] array = new GadgetPanelEntry[_entries.Count];
		_entries.CopyTo(array);
		_entries.Clear();
		foreach (GadgetInfo.GadgetCategory item in Enum.GetValues(typeof(GadgetInfo.GadgetCategory)).OfType<GadgetInfo.GadgetCategory>())
		{
			Gadget value = null;
			InGameGadgetSet.CurrentSet.TryGetValue(item, out value);
			if (value != null)
			{
				_entries.Add(new GadgetPanelEntry
				{
					Texture = ItemDb.GetItemIcon(value.Info.Id, (ShopNGUIController.CategoryNames)item),
					Info = value.Info
				});
			}
		}
		_entries.Sort(delegate(GadgetPanelEntry entry1, GadgetPanelEntry entry2)
		{
			if (entry1.Info.Category == GadgetsInfo.DefaultGadget && entry2.Info.Category == GadgetsInfo.DefaultGadget)
			{
				return 0;
			}
			if (entry1.Info.Category == GadgetsInfo.DefaultGadget)
			{
				return -1;
			}
			return (entry2.Info.Category == GadgetsInfo.DefaultGadget) ? 1 : entry1.Info.Category.CompareTo(entry2.Info.Category);
		});
		if (_entries.Count <= 0)
		{
			return;
		}
		gadgetButtonScript.gadgetIcon.mainTexture = _entries[0].Texture;
		if (_entries.Count > 1)
		{
			gadgetButtonScript.gadgetIcon1.mainTexture = _entries[1].Texture;
			if (_entries.Count > 2)
			{
				gadgetButtonScript.gadgetIcon2.mainTexture = _entries[2].Texture;
			}
			gadgetButtonScript.thirdAvailableGadgetCell.SetActiveSafeSelf(_entries.Count > 2);
			gadgetButtonScript.thirdAvailableGadgetFrame.SetActiveSafeSelf(_entries.Count > 2);
		}
	}

	private static void SaveDefaultGadget()
	{
		if (Storager.getInt("GadgetsInfo.DefaultGadgetKey", false) != (int)GadgetsInfo.DefaultGadget)
		{
			Storager.setInt("GadgetsInfo.DefaultGadgetKey", (int)GadgetsInfo.DefaultGadget, false);
		}
	}

	private static void LoadDefaultGadget()
	{
		int num = Storager.getInt("GadgetsInfo.DefaultGadgetKey", false);
		if (num == 0)
		{
			num = 12500;
		}
		GadgetsInfo.DefaultGadget = (GadgetInfo.GadgetCategory)num;
	}
}
