using UnityEngine;

internal class HintObject : MonoBehaviour
{
	public UILabel label;

	public Transform arrow;

	private HintController.HintItem myHint;

	private bool indicOn;

	private float lastIndic;

	public void Show(HintController.HintItem hint)
	{
		base.gameObject.SetActive(!hint.showLabelByCode);
		base.transform.parent = hint.target.transform;
		base.transform.localPosition = hint.relativeHintPosition;
		label.text = LocalizationStore.Get(hint.hintText);
		if (hint.manualRotateArrow)
		{
			arrow.localRotation = Quaternion.Euler(hint.manualArrowRotation);
		}
		else
		{
			arrow.localRotation = Quaternion.identity;
		}
		label.transform.localPosition = hint.relativeLabelPosition;
		if (label.transform.localPosition.x > 0f)
		{
			arrow.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			arrow.localScale = Vector3.one;
		}
		myHint = hint;
		lastIndic = Time.time;
		if (hint.scaleTween)
		{
			base.transform.localScale = Vector3.one * 0.3f;
		}
		else
		{
			base.transform.localScale = Vector3.one;
		}
	}

	public void Hide()
	{
		if (myHint.indicateTarget)
		{
			if (myHint.targetSprites == null || myHint.targetSprites.Length == 0)
			{
				myHint.targetSprite.spriteName = myHint.defaultSpriteName;
			}
			else
			{
				for (int i = 0; i < myHint.targetSprites.Length; i++)
				{
					myHint.targetSprites[i].color = Color.white;
				}
			}
		}
		myHint = null;
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (myHint.indicateTarget && lastIndic < Time.time)
		{
			lastIndic = Time.time + 0.5f;
			indicOn = !indicOn;
			if (myHint.targetSprites == null || myHint.targetSprites.Length == 0)
			{
				myHint.targetSprite.spriteName = ((!indicOn) ? myHint.defaultSpriteName : myHint.indicatedSpriteName);
			}
			else
			{
				for (int i = 0; i < myHint.targetSprites.Length; i++)
				{
					myHint.targetSprites[i].color = ((!indicOn) ? Color.white : Color.green);
				}
			}
		}
		if (myHint.scaleTween)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(1f, 1f, 1f), 3f * Time.unscaledDeltaTime);
		}
	}
}
