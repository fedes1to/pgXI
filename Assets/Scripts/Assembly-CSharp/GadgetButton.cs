using UnityEngine;

public class GadgetButton : MonoBehaviour
{
	public const int DefaultCircleSize = 90;

	public const int DefaultSpaceBetweenCircles = 20;

	public GameObject thirdAvailableGadgetCell;

	public GameObject thirdAvailableGadgetFrame;

	public GameObject yazichok;

	[Header("Duration Sprites")]
	public UISprite duration;

	public UISprite duration1;

	public UISprite duration2;

	[Header("Cooldown Sprites")]
	public UISprite cooldown;

	public UISprite cooldown1;

	public UISprite cooldown2;

	[Header("Cooldown Sprites")]
	public GameObject cooldownEnds;

	public GameObject cooldown1Ends;

	public GameObject cooldown2Ends;

	[Header("Count Labels")]
	public UILabel count;

	public UILabel count1;

	public UILabel count2;

	[Header("Gadget Icons")]
	public UITexture gadgetIcon;

	public UITexture gadgetIcon1;

	public UITexture gadgetIcon2;

	[Header("Containers With Count Labels")]
	public GameObject counter;

	public GameObject counter1;

	public GameObject counter2;

	public Animator gadgetAnimator;

	public bool isOpen;

	public Transform ContainerForScale;

	private UISprite _cachedSprite;

	public UISprite cachedSprite
	{
		get
		{
			if (_cachedSprite == null)
			{
				_cachedSprite = GetComponent<UISprite>();
			}
			return _cachedSprite;
		}
	}

	public void OpenGadgetPanel(bool isOpen)
	{
		gadgetAnimator.SetBool("IsOpen", isOpen);
		JoystickController.rightJoystick.gadgetPanelVisible = isOpen;
	}

	public void OnPanelOpen()
	{
	}

	private void Update()
	{
		float num = (float)cachedSprite.width / 90f;
		ContainerForScale.localScale = new Vector3(num, num, num);
	}
}
