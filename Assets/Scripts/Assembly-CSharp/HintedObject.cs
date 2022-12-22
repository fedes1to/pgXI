using UnityEngine;

[ExecuteInEditMode]
public class HintedObject : MonoBehaviour
{
	public enum ArrowPos
	{
		botRight,
		botCenter,
		botLeft,
		leftBot,
		leftCenter,
		leftTop,
		rightTop,
		rightCenter,
		rightBot,
		topLeft,
		topCenter,
		topRight
	}

	public int fontSize = 20;

	public string term = "hint";

	public float timeToShowHint = 0.2f;

	public MenuHintObject hintObj;

	public Vector3 position;

	public ArrowPos arrowPos;

	public bool showOnPress;

	public bool preview;

	private float timer;

	private bool press;

	private Transform tempTransform;

	private bool isShowing;

	private void OnPress(bool pressed)
	{
		timer = timeToShowHint;
		press = pressed;
		if (!pressed && hintObj.isActiveAndEnabled)
		{
			CloseHint();
		}
	}

	public void ShowHint()
	{
		isShowing = true;
		hintObj.gameObject.SetActive(true);
		hintObj.body.transform.parent = base.transform;
		hintObj.botRightArrow.SetActive(arrowPos == ArrowPos.botRight);
		hintObj.botCenterArrow.SetActive(arrowPos == ArrowPos.botCenter);
		hintObj.botLeftArrow.SetActive(arrowPos == ArrowPos.botLeft);
		hintObj.leftBotArrow.SetActive(arrowPos == ArrowPos.leftBot);
		hintObj.leftCenterArrow.SetActive(arrowPos == ArrowPos.leftCenter);
		hintObj.leftTopArrow.SetActive(arrowPos == ArrowPos.leftTop);
		hintObj.rightTopArrow.SetActive(arrowPos == ArrowPos.rightTop);
		hintObj.rightCenterArrow.SetActive(arrowPos == ArrowPos.rightCenter);
		hintObj.rightBotArrow.SetActive(arrowPos == ArrowPos.rightBot);
		hintObj.topLeftArrow.SetActive(arrowPos == ArrowPos.topLeft);
		hintObj.topCenterArrow.SetActive(arrowPos == ArrowPos.topCenter);
		hintObj.topRightArrow.SetActive(arrowPos == ArrowPos.topRight);
		hintObj.label.text = LocalizationStore.Get(term);
		hintObj.label.fontSize = fontSize;
		hintObj.label.transform.localPosition = new Vector3(0f, 0f);
		hintObj.body.transform.localPosition = position;
		if (Application.isPlaying)
		{
			hintObj.tween.PlayForward();
		}
		if (arrowPos == ArrowPos.leftTop || arrowPos == ArrowPos.rightTop || arrowPos == ArrowPos.topCenter || arrowPos == ArrowPos.topLeft || arrowPos == ArrowPos.topRight)
		{
			hintObj.label.pivot = UIWidget.Pivot.TopRight;
		}
		else if (arrowPos == ArrowPos.leftCenter || arrowPos == ArrowPos.rightCenter)
		{
			hintObj.label.pivot = UIWidget.Pivot.Right;
		}
		else
		{
			hintObj.label.pivot = UIWidget.Pivot.BottomRight;
		}
	}

	public void CloseHint()
	{
		isShowing = false;
		hintObj.gameObject.SetActive(false);
		if (Application.isPlaying)
		{
			hintObj.tween.ResetToBeginning();
		}
		timer = timeToShowHint;
		hintObj.body.transform.parent = hintObj.transform;
	}

	private void Update()
	{
		if (press && showOnPress)
		{
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				ShowHint();
			}
		}
		if (isShowing && showOnPress && !press)
		{
			CloseHint();
		}
		if (!Application.isPlaying)
		{
			if (preview)
			{
				ShowHint();
			}
			if (isShowing && !preview)
			{
				CloseHint();
			}
		}
	}
}
