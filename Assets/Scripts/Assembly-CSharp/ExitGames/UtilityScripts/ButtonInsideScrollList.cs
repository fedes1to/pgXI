using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExitGames.UtilityScripts
{
	public class ButtonInsideScrollList : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private ScrollRect scrollRect;

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (scrollRect != null)
			{
				scrollRect.StopMovement();
				scrollRect.enabled = false;
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (scrollRect != null && !scrollRect.enabled)
			{
				scrollRect.enabled = true;
			}
		}

		private void Start()
		{
			scrollRect = GetComponentInParent<ScrollRect>();
		}
	}
}
