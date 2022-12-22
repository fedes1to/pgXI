using UnityEngine;

public class PropertiesHideSHow : MonoBehaviour
{
	private const float shownPositionY = -300f;

	public bool isHidden;

	public float yAxysDelta;

	public float animTime;

	public Transform target;

	public Transform arrow;

	public UILabel text;

	private float animTimer;

	private Vector3 _startPosition;

	private bool _startPositionInitialized;

	private bool isAnimated;

	private Vector3 HiddenPosition
	{
		get
		{
			return new Vector3(startPosition.x, -300f - yAxysDelta, startPosition.z);
		}
	}

	private Vector3 startPosition
	{
		get
		{
			if (!_startPositionInitialized)
			{
				_startPositionInitialized = true;
				_startPosition = new Vector3(0f, -300f, 0f);
			}
			return _startPosition;
		}
	}

	private void Update()
	{
		if (!isAnimated)
		{
			return;
		}
		if (animTimer > 0f)
		{
			animTimer -= Time.unscaledDeltaTime;
			if (isHidden)
			{
				target.localPosition = Vector3.Lerp(HiddenPosition, startPosition, animTimer / animTime);
			}
			else
			{
				target.localPosition = Vector3.Lerp(startPosition, HiddenPosition, animTimer / animTime);
			}
		}
		else
		{
			isAnimated = false;
		}
	}

	public void OnHideClick()
	{
		isHidden = !isHidden;
		isAnimated = true;
		animTimer = animTime;
		AdjustUI();
	}

	public void SetState(bool isShown)
	{
		isHidden = !isShown;
		isAnimated = false;
		if (isShown)
		{
			target.localPosition = startPosition;
		}
		else
		{
			target.localPosition = HiddenPosition;
		}
		AdjustUI();
	}

	private void AdjustUI()
	{
		if (isHidden)
		{
			text.text = "SHOW";
			arrow.eulerAngles = new Vector3(0f, 0f, -90f);
		}
		else
		{
			text.text = "HIDE";
			arrow.eulerAngles = new Vector3(0f, 0f, 90f);
		}
	}
}
