using System;
using UnityEngine;

namespace Rilisoft
{
	[RequireComponent(typeof(Collider))]
	[ExecuteInEditMode]
	public abstract class AreaBase : MonoBehaviour
	{
		public const string AREA_OBJECT_TAG = "Area";

		public const string AREA_OBJECT_LAYER = "Ignore Raycast";

		[ReadOnly]
		[SerializeField]
		private bool _isActive;

		[SerializeField]
		private string _description;

		protected virtual void Awake()
		{
			Collider component = GetComponent<Collider>();
			if (component == null)
			{
				throw new Exception("Collider not found");
			}
			if (!component.isTrigger)
			{
				Debug.LogWarningFormat("[AREA SYSTEM] collider now is trigger, go:'{0}'", base.gameObject.name);
				component.isTrigger = true;
			}
			int num = LayerMask.NameToLayer("Ignore Raycast");
			if (base.gameObject.layer != num)
			{
				base.gameObject.layer = num;
			}
			if (!base.gameObject.CompareTag("Area"))
			{
				base.gameObject.tag = "Area";
			}
		}

		public virtual void CheckIn(GameObject to)
		{
			_isActive = true;
		}

		public virtual void CheckOut(GameObject from)
		{
			_isActive = false;
		}
	}
}
