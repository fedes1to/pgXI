using UnityEngine;
using UnityEngine.Events;

namespace Rilisoft
{
	[RequireComponent(typeof(UIButton))]
	public class ButtonEventsHandler : MonoBehaviour
	{
		public UnityEvent OnClickEvent;

		private void OnClick()
		{
			if (OnClickEvent != null)
			{
				OnClickEvent.Invoke();
			}
		}
	}
}
