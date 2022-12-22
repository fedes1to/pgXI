using UnityEngine;

internal sealed class GetColorInPalitra : MonoBehaviour
{
	public UITexture canvasTexture;

	public UISprite newColor;

	public UIButton okColorInPalitraButton;

	private void Update()
	{
		bool flag = false;
		Vector2 pos = Vector2.zero;
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			flag = touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled;
			pos = new Vector2(touch.position.x, touch.position.y);
		}
		if (flag && IsCanvasConteinPosition(pos))
		{
			Vector2 editPixelPos = GetEditPixelPos(pos);
			Color pixel = ((Texture2D)canvasTexture.mainTexture).GetPixel(Mathf.RoundToInt(editPixelPos.x), Mathf.RoundToInt(editPixelPos.y));
			newColor.color = pixel;
			okColorInPalitraButton.defaultColor = pixel;
			okColorInPalitraButton.pressed = pixel;
			okColorInPalitraButton.hover = pixel;
		}
	}

	private bool IsCanvasConteinPosition(Vector2 pos)
	{
		float num = (float)Screen.height / 768f;
		Vector2 vector = new Vector2(((float)Screen.width - (float)canvasTexture.width * num) * 0.5f + canvasTexture.transform.localPosition.x * num, ((float)Screen.height - (float)canvasTexture.height * num) * 0.5f + canvasTexture.transform.localPosition.y * num);
		return new Rect(vector.x, vector.y, (float)canvasTexture.width * num, (float)canvasTexture.height * num).Contains(pos);
	}

	private Vector2 GetEditPixelPos(Vector2 pos)
	{
		float num = (float)Screen.height / 768f;
		Vector2 vector = pos - new Vector2(((float)Screen.width - (float)canvasTexture.width * num) * 0.5f + canvasTexture.transform.localPosition.x * num, ((float)Screen.height - (float)canvasTexture.height * num) * 0.5f + canvasTexture.transform.localPosition.y * num);
		return new Vector2(Mathf.FloorToInt(vector.x / ((float)canvasTexture.width * num) * (float)canvasTexture.mainTexture.width), Mathf.FloorToInt(vector.y / ((float)canvasTexture.height * num) * (float)canvasTexture.mainTexture.height));
	}
}
