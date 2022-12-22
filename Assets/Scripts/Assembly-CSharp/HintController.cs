using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal sealed class HintController : MonoBehaviour
{
	public enum HideReason
	{
		None,
		ButtonClick
	}

	public enum ShowReason
	{
		New,
		PlayerDay,
		PlayerSession,
		level,
		TrainingStage,
		OpenByScript
	}

	[Serializable]
	public class HintItem
	{
		public string name;

		public GameObject target;

		public string hintText;

		public Vector3 relativeHintPosition;

		public Vector3 relativeLabelPosition;

		public ShowReason showReason;

		public HideReason hideReason;

		public UIButton[] buttonsToBlock;

		public GameObject[] objectsToHide;

		[HideInInspector]
		public bool[] buttonsState;

		[HideInInspector]
		public bool[] objActiveState;

		[HideInInspector]
		public UIButton targetButton;

		[HideInInspector]
		public UISprite targetSprite;

		[HideInInspector]
		public UISprite[] targetSprites;

		public float timeout;

		public bool indicateTarget;

		public bool manualRotateArrow;

		public bool scaleTween;

		public bool showLabelByCode;

		public Vector3 manualArrowRotation;

		public string indicatedSpriteName;

		[HideInInspector]
		public string defaultSpriteName;

		public bool enableColliders;

		public GameObject collidersObj;

		public TrainingController.NewTrainingCompletedStage trainingStage;
	}

	public HintObject hintObject;

	public HintItem[] hints;

	private readonly List<HintItem> hintToShow = new List<HintItem>();

	private EventDelegate showNextEvent;

	private static readonly List<HintController> _instances = new List<HintController>();

	private HintItem inShow;

	public static HintController instance
	{
		get
		{
			if (_instances.Any())
			{
				return _instances.Last();
			}
			return null;
		}
	}

	private void Awake()
	{
		_instances.Add(this);
	}

	private void OnDestroy()
	{
		_instances.Remove(this);
	}

	private void Start()
	{
		showNextEvent = new EventDelegate(this, "ShowNext");
		Invoke("StartShow", 0.5f);
	}

	private void OnEnable()
	{
		Invoke("StartShow", 0.5f);
	}

	private bool CheckShowReason(HintItem hint)
	{
		switch (hint.showReason)
		{
		case ShowReason.TrainingStage:
			return !TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == hint.trainingStage;
		case ShowReason.New:
			return true;
		default:
			return false;
		}
	}

	public void ShowHintByName(string name, float time = 0f)
	{
		if (hints == null)
		{
			return;
		}
		for (int i = 0; i < hints.Length; i++)
		{
			if (hints[i].name == name && !hintToShow.Contains(hints[i]))
			{
				hintToShow.Add(hints[i]);
				if (time == 0f)
				{
					ShowHint();
				}
				else
				{
					Invoke("ShowHint", time);
				}
			}
		}
	}

	public void HideCurrentHintObjectLabel()
	{
		if (hintToShow.Count > 0)
		{
			hintObject.gameObject.SetActive(false);
		}
	}

	public void ShowCurrentHintObjectLabel()
	{
		if (hintToShow.Count > 0)
		{
			hintObject.gameObject.SetActive(true);
		}
	}

	public void HideHintByName(string name)
	{
		if (inShow != null && inShow.name == name)
		{
			ShowNext();
			return;
		}
		for (int i = 0; i < hintToShow.Count; i++)
		{
			if (hintToShow[i].name == name)
			{
				hintToShow.RemoveAt(i);
				break;
			}
		}
	}

	public void StartShow()
	{
		if (hints != null && hintToShow.Count == 0)
		{
			for (int i = 0; i < hints.Length; i++)
			{
				if (CheckShowReason(hints[i]))
				{
					hintToShow.Add(hints[i]);
				}
			}
		}
		ShowHint();
	}

	public void ShowNext()
	{
		if (hintToShow.Count == 0)
		{
			return;
		}
		if (hintToShow[0].hideReason == HideReason.ButtonClick)
		{
			hintToShow[0].target.GetComponent<UIButton>().onClick.Remove(showNextEvent);
		}
		if (hintToShow[0].buttonsToBlock != null && hintToShow[0].buttonsToBlock.Length > 0)
		{
			for (int i = 0; i < hintToShow[0].buttonsToBlock.Length; i++)
			{
				hintToShow[0].buttonsToBlock[i].isEnabled = hintToShow[0].buttonsState[i];
			}
		}
		if (hintToShow[0].objectsToHide != null && hintToShow[0].objectsToHide.Length > 0)
		{
			for (int j = 0; j < hintToShow[0].objectsToHide.Length; j++)
			{
				hintToShow[0].objectsToHide[j].SetActive(hintToShow[0].objActiveState[j]);
			}
		}
		if (hintToShow[0].enableColliders)
		{
			hintToShow[0].collidersObj.SetActive(false);
		}
		hintToShow.RemoveAt(0);
		hintObject.Hide();
		inShow = null;
		ShowHint();
	}

	private void ShowHint()
	{
		if (hintToShow.Count <= 0)
		{
			return;
		}
		HintItem hintItem = hintToShow[0];
		if (inShow == hintItem)
		{
			return;
		}
		HideReason hideReason = hintItem.hideReason;
		if (hideReason == HideReason.ButtonClick)
		{
			hintItem.targetButton = hintItem.target.GetComponent<UIButton>();
			hintItem.targetButton.onClick.Add(showNextEvent);
		}
		if (hintItem.timeout > 0f)
		{
			Invoke("ShowNext", hintItem.timeout);
		}
		if (hintItem.buttonsToBlock != null && hintItem.buttonsToBlock.Length > 0)
		{
			hintItem.buttonsState = new bool[hintItem.buttonsToBlock.Length];
			for (int i = 0; i < hintItem.buttonsToBlock.Length; i++)
			{
				hintItem.buttonsState[i] = hintItem.buttonsToBlock[i].isEnabled;
				if (hintItem.buttonsToBlock[i] != hintItem.target.GetComponent<UIButton>())
				{
					hintItem.buttonsToBlock[i].isEnabled = false;
				}
			}
		}
		if (hintItem.objectsToHide != null && hintItem.objectsToHide.Length > 0)
		{
			hintItem.objActiveState = new bool[hintItem.objectsToHide.Length];
			for (int j = 0; j < hintItem.objectsToHide.Length; j++)
			{
				hintItem.objActiveState[j] = hintItem.objectsToHide[j].activeSelf;
				hintItem.objectsToHide[j].SetActive(false);
			}
		}
		if (hintItem.enableColliders)
		{
			hintItem.collidersObj.SetActive(true);
		}
		if (hintItem.indicateTarget)
		{
			if (hintItem.targetButton == null)
			{
				hintItem.targetButton = hintItem.target.GetComponent<UIButton>();
			}
			if (!string.IsNullOrEmpty(hintItem.indicatedSpriteName) && hintItem.targetButton != null)
			{
				hintItem.targetSprite = hintItem.targetButton.tweenTarget.GetComponent<UISprite>();
				hintItem.defaultSpriteName = hintItem.targetSprite.spriteName;
			}
			else
			{
				hintItem.targetSprites = hintItem.target.GetComponentsInChildren<UISprite>();
			}
		}
		hintObject.Show(hintItem);
		inShow = hintItem;
	}
}
