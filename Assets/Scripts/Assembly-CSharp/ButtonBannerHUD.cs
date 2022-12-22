using System;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBannerHUD : MonoBehaviour
{
	private const string keyLastShowIndex = "keyLastShowIndex";

	private const string pathToBtnBanner = "ButtonBanners";

	private const float timeForNextScroll = 5f;

	public static ButtonBannerHUD instance;

	public UIScrollView scrollBanners;

	public UIWrapContent wrapBanners;

	public ButtonBannerBase curShowBanner;

	private ButtonBannerBase oldShowBanner;

	public Transform objAnchorNoActiveBanners;

	public List<ButtonBannerBase> listAllBanners = new List<ButtonBannerBase>();

	public List<ButtonBannerBase> listActiveBanners = new List<ButtonBannerBase>();

	[HideInInspector]
	public MyCenterOnChild centerScript;

	public int IndexShowBanner
	{
		get
		{
			return Load.LoadInt("keyLastShowIndex");
		}
		set
		{
			int num = value;
			if (num < 0)
			{
				num = 0;
			}
			if (num >= listActiveBanners.Count)
			{
				num = 0;
			}
			Save.SaveInt("keyLastShowIndex", num);
		}
	}

	private void Awake()
	{
		instance = this;
		LoadAllExistBanners();
	}

	private void Start()
	{
		centerScript = wrapBanners.GetComponent<MyCenterOnChild>();
		if (centerScript != null)
		{
			MyCenterOnChild myCenterOnChild = centerScript;
			myCenterOnChild.onFinished = (SpringPanel.OnFinished)Delegate.Combine(myCenterOnChild.onFinished, new SpringPanel.OnFinished(OnCenterBanner));
		}
		LocalizationStore.AddEventCallAfterLocalize(LocalizeBanner);
		UpdateListBanners();
		ResetTimerNextBanner();
	}

	private void OnDestroy()
	{
		if (centerScript != null)
		{
			MyCenterOnChild myCenterOnChild = centerScript;
			myCenterOnChild.onFinished = (SpringPanel.OnFinished)Delegate.Remove(myCenterOnChild.onFinished, new SpringPanel.OnFinished(OnCenterBanner));
		}
		LocalizationStore.DelEventCallAfterLocalize(LocalizeBanner);
		instance = null;
	}

	private void LoadAllExistBanners()
	{
		listAllBanners.Clear();
		UnityEngine.Object[] array = Resources.LoadAll("ButtonBanners");
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			if (@object != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
				gameObject.transform.parent = objAnchorNoActiveBanners;
				gameObject.transform.localScale = Vector3.one;
				ButtonBannerBase component = gameObject.GetComponent<ButtonBannerBase>();
				if (component != null)
				{
					listAllBanners.Add(component);
				}
			}
		}
	}

	public static void OnUpdateBanners()
	{
		if (instance != null)
		{
			instance.UpdateListBanners();
		}
	}

	private void UpdateListBanners()
	{
		RemoveAllNoActiveBanners();
		AddNewActiveBanners();
		SortByPriority();
	}

	private void RemoveAllNoActiveBanners()
	{
		if (curShowBanner != null && !curShowBanner.BannerIsActive())
		{
			curShowBanner = GetNextActiveBanner();
		}
		List<ButtonBannerBase> list = new List<ButtonBannerBase>();
		foreach (ButtonBannerBase listActiveBanner in listActiveBanners)
		{
			if (!listActiveBanner.BannerIsActive())
			{
				list.Add(listActiveBanner);
			}
		}
		foreach (ButtonBannerBase item in list)
		{
			item.transform.parent = objAnchorNoActiveBanners;
			listActiveBanners.Remove(item);
		}
	}

	private bool IsExistActiveBanner(ButtonBannerBase needBanners)
	{
		return listActiveBanners.Contains(needBanners);
	}

	private void AddNewActiveBanners()
	{
		foreach (ButtonBannerBase listAllBanner in listAllBanners)
		{
			if (!IsExistActiveBanner(listAllBanner) && listAllBanner.BannerIsActive())
			{
				listAllBanner.transform.parent = wrapBanners.transform;
				listActiveBanners.Add(listAllBanner);
			}
		}
	}

	private void SortByPriority()
	{
		listActiveBanners.Sort(delegate(ButtonBannerBase left, ButtonBannerBase right)
		{
			if (left == null && right == null)
			{
				return 0;
			}
			if (left == null)
			{
				return -1;
			}
			return (right == null) ? 1 : left.priorityShow.CompareTo(right.priorityShow);
		});
		string empty = string.Empty;
		for (int i = 0; i < listActiveBanners.Count; i++)
		{
			empty = i.ToString();
			if (i < 10)
			{
				empty = "0" + empty;
			}
			listActiveBanners[i].gameObject.name = empty;
			listActiveBanners[i].indexBut = i;
		}
		wrapBanners.SortAlphabetically();
		wrapBanners.WrapContent();
		ShowBanner(curShowBanner);
	}

	public void OnClickShowBanner()
	{
		if (curShowBanner != null)
		{
			curShowBanner.OnClickButton();
		}
	}

	private ButtonBannerBase GetNextActiveBanner()
	{
		int num = listActiveBanners.Count;
		ButtonBannerBase buttonBannerBase = null;
		if (num > 0)
		{
			while (true)
			{
				IndexShowBanner++;
				num--;
				buttonBannerBase = listActiveBanners[IndexShowBanner];
				if (buttonBannerBase.BannerIsActive())
				{
					break;
				}
				if (num <= 0)
				{
					Debug.LogWarning("No next banner for show");
					break;
				}
			}
		}
		return buttonBannerBase;
	}

	public void ShowNextBanner()
	{
		ButtonBannerBase nextActiveBanner = GetNextActiveBanner();
		ShowBanner(nextActiveBanner);
	}

	public void ShowBanner(ButtonBannerBase needBanner)
	{
		if (!(needBanner == null) && needBanner.BannerIsActive() && needBanner != null)
		{
			SetShowBanner(needBanner, true);
			centerScript.CenterOn(needBanner.transform);
		}
	}

	public void OnCenterBanner()
	{
		if (centerScript != null)
		{
			ButtonBannerBase component = centerScript.centeredObject.GetComponent<ButtonBannerBase>();
			if (oldShowBanner != null)
			{
				oldShowBanner.OnHide();
			}
			SetShowBanner(component);
			if ((bool)component)
			{
				ResetTimerNextBanner();
			}
		}
	}

	private void SetShowBanner(ButtonBannerBase needBanner, bool auto = false)
	{
		if (!(curShowBanner != needBanner))
		{
			return;
		}
		oldShowBanner = curShowBanner;
		curShowBanner = needBanner;
		if (curShowBanner != null)
		{
			IndexShowBanner = curShowBanner.indexBut;
			curShowBanner.OnShow();
			if (auto)
			{
				curShowBanner.OnUpdateParameter();
			}
		}
	}

	public void StopTimerNextBanner()
	{
		CancelInvoke("ShowNextBanner");
	}

	public void ResetTimerNextBanner()
	{
		StopTimerNextBanner();
		InvokeRepeating("ShowNextBanner", 5f, 5f);
	}

	public void LocalizeBanner()
	{
		if (curShowBanner != null)
		{
			curShowBanner.OnChangeLocalize();
		}
	}
}
