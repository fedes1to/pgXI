using UnityEngine;

public class PreviewSkin : MonoBehaviour
{
	public Camera previewCamera;

	private Vector2 touchPosition;

	private bool isTapDown;

	private GameObject selectedGameObject;

	private float sideMargin = 100f;

	private float topBottMargins = 120f;

	private Rect swipeZone;

	private Vector3 rememberedScale;

	private Vector3 rememberedBodyOffs;

	private void Start()
	{
		swipeZone = new Rect(sideMargin, topBottMargins, (float)Screen.width - sideMargin * 2f, (float)Screen.height - topBottMargins * 2f);
	}

	private void Update()
	{
		if (!isTapDown && Input2.touchCount > 0 && Input2.GetTouch(0).phase == TouchPhase.Began)
		{
			touchPosition = ((Input2.touchCount <= 0) ? new Vector2(Input.mousePosition.x, Input.mousePosition.y) : Input2.GetTouch(0).position);
			if (swipeZone.Contains(touchPosition))
			{
				isTapDown = true;
				selectedGameObject = GameObjectOnTouch(touchPosition);
				if (selectedGameObject != null)
				{
					Highlight(selectedGameObject);
				}
			}
			return;
		}
		if (isTapDown && Input2.touchCount > 0 && Input2.GetTouch(0).phase == TouchPhase.Moved)
		{
			float num = ((Input2.touchCount <= 0) ? (touchPosition.x - Input.mousePosition.x) : (touchPosition.x - Input2.GetTouch(0).position.x));
			if (selectedGameObject != null && Mathf.Abs(num) > 2f)
			{
				Unhighlight(selectedGameObject);
				selectedGameObject = null;
			}
			else
			{
				float num2 = 0.5f;
				base.transform.Rotate(0f, num2 * num, 0f, Space.Self);
				touchPosition = ((Input2.touchCount <= 0) ? new Vector2(Input.mousePosition.x, Input.mousePosition.y) : Input2.GetTouch(0).position);
			}
		}
		if (Input2.touchCount <= 0 || (Input2.GetTouch(0).phase != TouchPhase.Ended && Input2.GetTouch(0).phase != TouchPhase.Canceled))
		{
			return;
		}
		if (selectedGameObject != null)
		{
			ButtonClickSound.Instance.PlayClick();
			Unhighlight(selectedGameObject);
			if (SkinEditorController.sharedController != null)
			{
				SkinEditorController.sharedController.SelectPart(selectedGameObject.name);
			}
			selectedGameObject = null;
		}
		isTapDown = false;
	}

	public GameObject GameObjectOnTouch(Vector2 touchPosition)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(previewCamera.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, 0f)), out hitInfo))
		{
			return hitInfo.collider.gameObject;
		}
		return null;
	}

	public void Highlight(GameObject go)
	{
		Renderer component = go.GetComponent<Renderer>();
		if (!(component == null))
		{
			Color color = component.materials[0].color;
			component.materials[0].color = new Color(color.r, color.g, color.b, 0.6f);
		}
	}

	public void Unhighlight(GameObject go)
	{
		Renderer component = go.GetComponent<Renderer>();
		if (!(component == null))
		{
			Color color = component.materials[0].color;
			component.materials[0].color = new Color(color.r, color.g, color.b, 1f);
		}
	}

	private void OnEnable()
	{
		isTapDown = false;
		selectedGameObject = null;
	}

	private void OnDisable()
	{
		isTapDown = false;
		selectedGameObject = null;
	}
}
