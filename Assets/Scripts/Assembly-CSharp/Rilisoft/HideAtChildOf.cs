using System.Collections.Generic;
using Unity.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class HideAtChildOf : MonoBehaviour
	{
		[SerializeField]
		private string _rootObjectName;

		private void Start()
		{
			if (_rootObjectName.IsNullOrEmpty())
			{
				return;
			}
			_rootObjectName = _rootObjectName.ToLower();
			IEnumerable<GameObject> enumerable = base.gameObject.Ancestors();
			foreach (GameObject item in enumerable)
			{
				if (item.name.ToLower() == _rootObjectName)
				{
					base.gameObject.SetActive(false);
					break;
				}
			}
		}
	}
}
