using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class ToggleGroupHalper : MonoBehaviour
	{
		[SerializeField]
		private int _toggleGroup;

		private Dictionary<UIToggle, EventDelegate> _subscribers = new Dictionary<UIToggle, EventDelegate>();

		public event Action<string, bool> OnSelectedToggleChanged;

		private void OnEnable()
		{
			if (_toggleGroup == 0)
			{
				Debug.LogError("toggle group not setted");
				return;
			}
			foreach (KeyValuePair<UIToggle, EventDelegate> subscriber in _subscribers)
			{
				if (subscriber.Key != null && subscriber.Value != null)
				{
					subscriber.Key.onChange.Remove(subscriber.Value);
				}
			}
			_subscribers.Clear();
			List<UIToggle> toggles = GetToggles();
			if (toggles.Count < 1)
			{
				Debug.LogError("toggles not found");
				return;
			}
			foreach (UIToggle toggle in GetToggles())
			{
				if (!_subscribers.ContainsKey(toggle))
				{
					string name = toggle.name.ToString();
					EventDelegate eventDelegate = new EventDelegate(delegate
					{
						OnToggleStateChanged(name);
					});
					_subscribers.Add(toggle, eventDelegate);
					toggle.onChange.Add(eventDelegate);
				}
			}
		}

		public List<UIToggle> GetToggles()
		{
			return (from t in base.gameObject.GetComponentsInChildren<UIToggle>()
				where t.@group == _toggleGroup
				select t).ToList();
		}

		public UIToggle GetSelectedToggle()
		{
			return GetToggles().FirstOrDefault((UIToggle t) => t.value);
		}

		private void OnToggleStateChanged(string name)
		{
			if (this.OnSelectedToggleChanged != null)
			{
				UIToggle uIToggle = GetToggles().FirstOrDefault((UIToggle t) => t.gameObject.name == name);
				if (uIToggle != null)
				{
					this.OnSelectedToggleChanged(name, uIToggle.value);
				}
			}
		}

		public void SelectToggle(string goName)
		{
			UIToggle uIToggle = GetToggles().FirstOrDefault((UIToggle t) => t.gameObject.name == goName);
			if (uIToggle != null && !uIToggle.value)
			{
				uIToggle.value = true;
			}
		}
	}
}
