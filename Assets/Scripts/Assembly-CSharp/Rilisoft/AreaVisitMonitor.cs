using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[RequireComponent(typeof(Collider))]
	public class AreaVisitMonitor : MonoBehaviour
	{
		[SerializeField]
		[ReadOnly]
		private List<AreaBase> _activeAreas = new List<AreaBase>();

		private void Awake()
		{
			Collider component = GetComponent<Collider>();
			if (component == null)
			{
				throw new Exception("Collider not found");
			}
			if (!component.isTrigger)
			{
				Debug.LogWarningFormat("[AREA SYSTEM] collider now is trigger go:'{0}'", base.gameObject.name);
				component.isTrigger = true;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			AreaBase component = other.GetComponent<AreaBase>();
			if (!(component == null))
			{
				_activeAreas.Add(component);
				component.CheckIn(base.gameObject);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			AreaBase component = other.GetComponent<AreaBase>();
			if (!(component == null))
			{
				if (_activeAreas.Contains(component))
				{
					_activeAreas.Remove(component);
				}
				component.CheckOut(base.gameObject);
			}
		}

		private void OnDisable()
		{
			_activeAreas.ForEach(delegate(AreaBase a)
			{
				a.CheckOut(base.gameObject);
			});
			_activeAreas.Clear();
		}

		private void Update()
		{
		}
	}
}
