using System;
using Rilisoft;
using UnityEngine;

public class PropertyInfoScreenController : MonoBehaviour
{
	public GameObject description;

	public GameObject descriptionMelee;

	private IDisposable _escapeSubscription;

	public virtual void Show(bool isMelee)
	{
		base.gameObject.SetActive(true);
		((!isMelee) ? description : descriptionMelee).SetActive(true);
		((!isMelee) ? descriptionMelee : description).SetActive(false);
	}

	public virtual void Hide()
	{
		base.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		if (_escapeSubscription != null)
		{
			_escapeSubscription.Dispose();
		}
		_escapeSubscription = BackSystem.Instance.Register(HandleEscape, "Property Info");
	}

	private void OnDisable()
	{
		if (_escapeSubscription != null)
		{
			_escapeSubscription.Dispose();
			_escapeSubscription = null;
		}
	}

	private void HandleEscape()
	{
		Hide();
	}
}
