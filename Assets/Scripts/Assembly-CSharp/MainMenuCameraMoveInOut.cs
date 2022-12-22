using UnityEngine;

[RequireComponent(typeof(Camera))]
internal sealed class MainMenuCameraMoveInOut : MonoBehaviour
{
	public abstract class State
	{
	}

	public sealed class IdleState : State
	{
	}

	public sealed class ActiveState : State
	{
	}

	public sealed class TransitionState : State
	{
	}

	private Vector3 _initialPosition;

	private Quaternion _initialRotation;

	private State _currentState = new IdleState();

	internal State CurrentState
	{
		get
		{
			return _currentState;
		}
	}

	public void HandleClickTrigger()
	{
		if (CurrentState is IdleState)
		{
			_currentState = new TransitionState();
			_currentState = new ActiveState();
		}
		else
		{
			string message = string.Format("Ignoring click while in {0} state.", _currentState);
			Debug.Log(message);
		}
	}

	public void HandleBackRequest()
	{
		if (CurrentState is ActiveState)
		{
			_currentState = new TransitionState();
			_currentState = new IdleState();
		}
		else
		{
			string message = string.Format("Ignoring click while in {0} state.", _currentState);
			Debug.LogWarning(message);
		}
	}

	public void Reset()
	{
		base.gameObject.transform.position = _initialPosition;
		base.gameObject.transform.rotation = _initialRotation;
		_currentState = new IdleState();
	}

	private void Awake()
	{
		_initialPosition = base.gameObject.transform.position;
		_initialRotation = base.gameObject.transform.rotation;
	}
}
