using System.Collections;
using Rilisoft;
using UnityEngine;

[RequireComponent(typeof(UIInput))]
public class UIInputFocusAtEnable : MonoBehaviour
{
	private const float FOCUS_DELAY = 0.3f;

	[SerializeField]
	[Tooltip("Применить только один раз")]
	private bool _onlyOnce;

	[SerializeField]
	[ReadOnly]
	private UIInput _input;

	private bool _alreadyTurned;

	private void Awake()
	{
		_input = GetComponent<UIInput>();
		if (_input == null)
		{
			Debug.LogError("input not found");
		}
	}

	private void OnEnable()
	{
		if (!_onlyOnce || !_alreadyTurned)
		{
			StartCoroutine(SetSelected());
			_alreadyTurned = true;
		}
	}

	private IEnumerator SetSelected()
	{
		_input.isSelected = false;
		yield return null;
		while (!_input.isSelected)
		{
			yield return new WaitForRealSeconds(0.3f);
			_input.isSelected = true;
		}
	}
}
