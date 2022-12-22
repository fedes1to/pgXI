using System.Collections;
using UnityEngine;

public class CoinsAddIndic : MonoBehaviour
{
	private const float blinkR = 255f;

	private const float blinkG = 255f;

	private const float blinkB = 0f;

	private const float blinkA = 115f;

	private const float blinkGemsR = 0f;

	private const float blinkGemsG = 0f;

	private const float blinkGemsB = 255f;

	private const float blinkGemsA = 115f;

	public bool stopBlinkingOnEnable;

	public bool isGems;

	public bool isX3;

	private UISprite _ind;

	private bool blinking;

	public bool remembeState;

	private int backgroundAdd;

	private bool isSurvival;

	private UISprite ind
	{
		get
		{
			if (_ind == null)
			{
				_ind = GetComponent<UISprite>();
			}
			return _ind;
		}
	}

	private Color BlinkColor
	{
		get
		{
			return (!isGems) ? new Color(1f, 1f, 0f, 23f / 51f) : new Color(0f, 0f, 1f, 23f / 51f);
		}
	}

	private void Start()
	{
		isSurvival = Defs.IsSurvival;
		if (remembeState)
		{
			CoinsMessage.CoinsLabelDisappeared -= BackgroundEventAdd;
			CoinsMessage.CoinsLabelDisappeared += BackgroundEventAdd;
		}
	}

	private void OnEnable()
	{
		CoinsMessage.CoinsLabelDisappeared += IndicateCoinsAdd;
		if (ind != null)
		{
			ind.color = NormalColor();
		}
		if (backgroundAdd > 0)
		{
			blinking = false;
			IndicateCoinsAdd(backgroundAdd == 1, 2);
			backgroundAdd = 0;
		}
		if (blinking && !stopBlinkingOnEnable)
		{
			StartCoroutine(blink());
		}
		else if (stopBlinkingOnEnable)
		{
			blinking = false;
		}
	}

	private void OnDisable()
	{
		CoinsMessage.CoinsLabelDisappeared -= IndicateCoinsAdd;
	}

	private void OnDestroy()
	{
		if (remembeState)
		{
			CoinsMessage.CoinsLabelDisappeared -= BackgroundEventAdd;
		}
	}

	private void IndicateCoinsAdd(bool gems, int count)
	{
		if (isGems == gems && !blinking)
		{
			StartCoroutine(blink());
		}
	}

	private Color NormalColor()
	{
		return (!isX3) ? new Color(0f, 0f, 0f, 23f / 51f) : new Color(1f, 0f, 0f, 0.5882353f);
	}

	private IEnumerator blink()
	{
		if (ind == null)
		{
			Debug.LogWarning("Indicator sprite is null.");
			yield return null;
		}
		blinking = true;
		try
		{
			for (int i = 0; i < 15; i++)
			{
				ind.color = BlinkColor;
				yield return null;
				yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(0.1f));
				ind.color = NormalColor();
				yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(0.1f));
			}
			ind.color = NormalColor();
		}
		finally
		{
			blinking = false;
		}
	}

	private void BackgroundEventAdd(bool gems, int count)
	{
		if (isGems == gems && (BankController.Instance == null || !BankController.Instance.InterfaceEnabled) && GiftBannerWindow.instance != null && GiftBannerWindow.instance.IsShow)
		{
			if (gems && isGems)
			{
				backgroundAdd = 1;
			}
			if (!gems && !isGems)
			{
				backgroundAdd = 2;
			}
		}
	}
}
