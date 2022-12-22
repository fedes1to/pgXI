using UnityEngine;

public sealed class CameraTouchControlScheme_CleanNGUI : CameraTouchControlScheme
{
	public float firstDragClampedMax = 5f;

	private bool _limitDragDelta;

	public override void OnPress(bool isDown)
	{
		_limitDragDelta = isDown;
	}

	public override void OnDrag(Vector2 delta)
	{
		if (_limitDragDelta)
		{
			_limitDragDelta = false;
			_deltaPosition = Vector2.ClampMagnitude(delta, firstDragClampedMax);
		}
		else
		{
			_deltaPosition = delta;
		}
		WeaponManager.sharedManager.myPlayerMoveC.mySkinName.MoveCamera(_deltaPosition);
		Reset();
	}

	public override void Reset()
	{
		_deltaPosition = Vector2.zero;
		_limitDragDelta = false;
	}

	public override void ApplyDeltaTo(Vector2 deltaPosition, Transform yawTransform, Transform pitchTransform, float sensitivity, bool invert)
	{
		Vector2 vector = deltaPosition * sensitivity * 30f / Screen.width;
		yawTransform.Rotate(0f, vector.x, 0f, Space.World);
		pitchTransform.Rotate(((!invert) ? (-1f) : 1f) * vector.y, 0f, 0f);
	}
}
