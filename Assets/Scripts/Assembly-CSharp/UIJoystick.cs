using UnityEngine;

public class UIJoystick : MonoBehaviour
{
	public const int NO_TOUCH_ID = -100;

	public Transform target;

	public float radius;

	public Vector2 value;

	private HungerGameController _hungerGameController;

	private bool _isHunger;

	private float? _actualRadius;

	private TouchPadInJoystick touchPadInJoystick;

	private UIWidget _joystickWidget;

	private bool _grabTouches;

	private Vector2 _touchWorldPos;

	private Vector2 _touchPrevWorldPos;

	private int _touchId;

	public float ActualRadius
	{
		get
		{
			return (!_actualRadius.HasValue) ? radius : _actualRadius.Value;
		}
		set
		{
			_actualRadius = value;
		}
	}

	public float ActualRadiusSq
	{
		get
		{
			float actualRadius = ActualRadius;
			return actualRadius * actualRadius;
		}
	}

	private void Awake()
	{
		_joystickWidget = GetComponent<UIWidget>();
		touchPadInJoystick = GetComponent<TouchPadInJoystick>();
		_isHunger = Defs.isHunger;
	}

	private void Start()
	{
		ChangeSide();
		PauseNGUIController.PlayerHandUpdated += ChangeSide;
		if (_isHunger)
		{
			_hungerGameController = GameObject.FindGameObjectWithTag("HungerGameController").GetComponent<HungerGameController>();
		}
		UpdateVisibility();
		Reset();
	}

	private void OnDestroy()
	{
		PauseNGUIController.PlayerHandUpdated -= ChangeSide;
	}

	private void Update()
	{
		ProcessInput();
		UpdateVisibility();
	}

	private void ProcessInput()
	{
		if (_grabTouches)
		{
			ProcessTouches();
		}
		else if (!Defs.isMouseControl)
		{
			Reset();
		}
	}

	private void ProcessTouches()
	{
		if (_touchId != -100)
		{
			Touch? touchById = GetTouchById(_touchId);
			if (touchById.HasValue)
			{
				_touchWorldPos = touchById.Value.position;
			}
		}
		if (_touchId != -100)
		{
			Vector2 delta = _touchPrevWorldPos - _touchWorldPos;
			_touchPrevWorldPos = _touchWorldPos;
			OnDrag2(delta);
			if (touchPadInJoystick.isShooting)
			{
				value = Vector2.zero;
				target.localPosition = Vector3.zero;
			}
		}
		else
		{
			Reset();
		}
	}

	public static Touch? GetTouchById(int touchId)
	{
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.fingerId == touchId && (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
			{
				return touch;
			}
		}
		return null;
	}

	private void OnPress(bool isDown)
	{
		if ((isDown && _touchId == -100) || (!isDown && _touchId != -100))
		{
			_grabTouches = isDown;
			_touchId = ((!isDown) ? (-100) : UICamera.currentTouchID);
			_touchWorldPos = ((!isDown) ? Vector2.zero : UICamera.currentTouch.pos);
			_touchPrevWorldPos = _touchWorldPos;
		}
	}

	public void SetJoystickActive(bool joyActive)
	{
		base.enabled = joyActive;
		if (!joyActive)
		{
			Reset();
		}
	}

	private void UpdateVisibility()
	{
		bool flag = (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0 || TrainingController.stepTraining >= TrainingState.TapToMove) && (!_isHunger || _hungerGameController.isGo);
		_joystickWidget.alpha = ((!flag) ? 0f : 1f);
	}

	private void ChangeSide()
	{
		base.transform.parent.GetComponent<UIAnchor>().side = ((!GlobalGameController.LeftHanded) ? UIAnchor.Side.BottomRight : UIAnchor.Side.BottomLeft);
		Reset();
	}

	public void Reset()
	{
		value = Vector2.zero;
		target.localPosition = Vector3.zero;
		_grabTouches = false;
		_touchId = -100;
		_touchWorldPos = Vector2.zero;
		_touchPrevWorldPos = _touchWorldPos;
	}

	private void OnDrag2(Vector2 delta)
	{
		target.position = UICamera.currentCamera.ScreenToWorldPoint(_touchWorldPos);
		target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, 0f);
		float magnitude = target.localPosition.magnitude;
		if (magnitude > ActualRadius)
		{
			target.localPosition = Vector3.ClampMagnitude(target.localPosition, ActualRadius);
		}
		value = target.localPosition;
		value = value / ActualRadius * Mathf.InverseLerp(ActualRadius, 2f, 1f);
	}
}
