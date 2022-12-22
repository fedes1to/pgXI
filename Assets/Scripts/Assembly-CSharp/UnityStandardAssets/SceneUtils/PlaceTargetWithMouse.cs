using UnityEngine;

namespace UnityStandardAssets.SceneUtils
{
	public class PlaceTargetWithMouse : MonoBehaviour
	{
		public float surfaceOffset = 1.5f;

		public GameObject setTargetOn;

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				base.transform.position = hitInfo.point + hitInfo.normal * surfaceOffset;
				if (setTargetOn != null)
				{
					setTargetOn.SendMessage("SetTarget", base.transform);
				}
			}
		}
	}
}
