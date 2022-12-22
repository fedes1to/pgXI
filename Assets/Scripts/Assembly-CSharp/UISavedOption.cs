using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	public string keyName;

	private UIPopupList mList;

	private UIToggle mCheck;

	private UIProgressBar mSlider;

	private string key
	{
		get
		{
			return (!string.IsNullOrEmpty(keyName)) ? keyName : ("NGUI State: " + base.name);
		}
	}

	private void Awake()
	{
		mList = GetComponent<UIPopupList>();
		mCheck = GetComponent<UIToggle>();
		mSlider = GetComponent<UIProgressBar>();
	}

	private void OnEnable()
	{
		if (mList != null)
		{
			EventDelegate.Add(mList.onChange, SaveSelection);
			string @string = PlayerPrefs.GetString(key);
			if (!string.IsNullOrEmpty(@string))
			{
				mList.value = @string;
			}
			return;
		}
		if (mCheck != null)
		{
			EventDelegate.Add(mCheck.onChange, SaveState);
			mCheck.value = PlayerPrefs.GetInt(key, mCheck.startsActive ? 1 : 0) != 0;
			return;
		}
		if (mSlider != null)
		{
			EventDelegate.Add(mSlider.onChange, SaveProgress);
			mSlider.value = PlayerPrefs.GetFloat(key, mSlider.value);
			return;
		}
		string string2 = PlayerPrefs.GetString(key);
		UIToggle[] componentsInChildren = GetComponentsInChildren<UIToggle>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UIToggle uIToggle = componentsInChildren[i];
			uIToggle.value = uIToggle.name == string2;
		}
	}

	private void OnDisable()
	{
		if (mCheck != null)
		{
			EventDelegate.Remove(mCheck.onChange, SaveState);
			return;
		}
		if (mList != null)
		{
			EventDelegate.Remove(mList.onChange, SaveSelection);
			return;
		}
		if (mSlider != null)
		{
			EventDelegate.Remove(mSlider.onChange, SaveProgress);
			return;
		}
		UIToggle[] componentsInChildren = GetComponentsInChildren<UIToggle>(true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UIToggle uIToggle = componentsInChildren[i];
			if (uIToggle.value)
			{
				PlayerPrefs.SetString(key, uIToggle.name);
				break;
			}
		}
	}

	public void SaveSelection()
	{
		PlayerPrefs.SetString(key, UIPopupList.current.value);
	}

	public void SaveState()
	{
		PlayerPrefs.SetInt(key, UIToggle.current.value ? 1 : 0);
	}

	public void SaveProgress()
	{
		PlayerPrefs.SetFloat(key, UIProgressBar.current.value);
	}
}
