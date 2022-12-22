using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

public class AskNameManager : MonoBehaviour
{
	public const string keyNameAlreadySet = "keyNameAlreadySet";

	public static AskNameManager instance;

	public GameObject objWindow;

	public GameObject objPanelSetName;

	public GameObject objPanelEnterName;

	public UILabel lbPlayerName;

	public UIInput inputPlayerName;

	public UIButton btnSetName;

	public GameObject objLbWarning;

	private int _NameAlreadySet = -1;

	private string curChooseName = string.Empty;

	private bool isAutoName;

	public static bool isComplete;

	public static bool isShow;

	private IDisposable _backSubcripter;

	private bool CanShowWindow
	{
		get
		{
			if (NameAlreadySet)
			{
				return false;
			}
			if (TrainingController.CompletedTrainingStage != TrainingController.NewTrainingCompletedStage.ShopCompleted)
			{
				return false;
			}
			if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
			{
				return true;
			}
			if (MainMenuController.sharedController.SyncFuture.IsCompleted)
			{
				return true;
			}
			return false;
		}
	}

	private bool NameAlreadySet
	{
		get
		{
			if (_NameAlreadySet == -1)
			{
				_NameAlreadySet = Load.LoadInt("keyNameAlreadySet");
			}
			return _NameAlreadySet == 1;
		}
		set
		{
			_NameAlreadySet = (value ? 1 : 0);
			Save.SaveInt("keyNameAlreadySet", _NameAlreadySet);
		}
	}

	private bool CanSetName
	{
		get
		{
			string value = curChooseName.Trim();
			if (!string.IsNullOrEmpty(value))
			{
				return true;
			}
			return false;
		}
	}

	public static event Action onComplete;

	private void Awake()
	{
		instance = this;
		isComplete = false;
		isShow = false;
		objWindow.SetActive(false);
		objPanelSetName.SetActive(false);
		objPanelEnterName.SetActive(false);
		objLbWarning.SetActive(false);
		AskIsCompleted();
		MainMenuController.onEnableMenuForAskname += ShowWindow;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		MainMenuController.onEnableMenuForAskname -= ShowWindow;
		instance = null;
	}

	public void ShowWindow()
	{
		StopCoroutine("WaitAndShowWindow");
		StartCoroutine("WaitAndShowWindow");
	}

	private IEnumerator WaitAndShowWindow()
	{
		if (AskIsCompleted())
		{
			yield break;
		}
		while (!CanShowWindow)
		{
			if (AskIsCompleted())
			{
				yield break;
			}
			yield return null;
			yield return null;
		}
		OnShowWindowSetName();
	}

	private bool AskIsCompleted()
	{
		bool flag = NameAlreadySet || TrainingController.TrainingCompleted;
		if (flag)
		{
			isComplete = true;
			if (AskNameManager.onComplete != null)
			{
				AskNameManager.onComplete();
			}
		}
		return flag;
	}

	private void OnShowWindowSetName()
	{
		if (_backSubcripter != null)
		{
			_backSubcripter.Dispose();
		}
		_backSubcripter = BackSystem.Instance.Register(delegate
		{
		});
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("+ OnShowWindowSetName");
		}
		isShow = true;
		curChooseName = GetNameForAsk();
		lbPlayerName.text = curChooseName;
		inputPlayerName.value = curChooseName;
		CheckActiveBtnSetName();
		objPanelSetName.SetActive(true);
		objWindow.SetActive(true);
		isAutoName = true;
	}

	public void OnShowWindowEnterName()
	{
		objPanelEnterName.SetActive(true);
		objPanelSetName.SetActive(false);
		OnStartEnterName();
	}

	private string GetNameForAsk()
	{
		return ProfileController.GetPlayerNameOrDefault();
	}

	private void CheckActiveBtnSetName()
	{
		BoxCollider component = btnSetName.GetComponent<BoxCollider>();
		objLbWarning.SetActive(false);
		if (CanSetName)
		{
			component.enabled = true;
			btnSetName.SetState(UIButtonColor.State.Normal, true);
		}
		else
		{
			objLbWarning.SetActive(true);
			component.enabled = false;
			btnSetName.SetState(UIButtonColor.State.Disabled, true);
		}
	}

	public void OnStartEnterName()
	{
		if (isAutoName)
		{
			inputPlayerName.isSelected = true;
			curChooseName = string.Empty;
			inputPlayerName.value = curChooseName;
			CheckActiveBtnSetName();
			isAutoName = false;
		}
	}

	public void OnChangeName()
	{
		curChooseName = inputPlayerName.value;
		CheckActiveBtnSetName();
	}

	public void SaveChooseName()
	{
		if (ProfileController.Instance != null)
		{
			ProfileController.Instance.SaveNamePlayer(curChooseName);
		}
		if (PlayerPanel.instance != null)
		{
			PlayerPanel.instance.UpdateNickPlayer();
		}
		if (MainMenuController.sharedController != null)
		{
		}
		NameAlreadySet = true;
		OnCloseAllWindow();
	}

	private void OnCloseAllWindow()
	{
		if (_backSubcripter != null)
		{
			_backSubcripter.Dispose();
		}
		objWindow.SetActive(false);
		isComplete = true;
		if (AskNameManager.onComplete != null)
		{
			AskNameManager.onComplete();
		}
		isShow = false;
	}

	[ContextMenu("Show Window")]
	public void TestShow()
	{
		isComplete = false;
		OnShowWindowSetName();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && objWindow != null && objWindow.activeInHierarchy)
		{
			curChooseName = "Player";
			SaveChooseName();
		}
	}
}
