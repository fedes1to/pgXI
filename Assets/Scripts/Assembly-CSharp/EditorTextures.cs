using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

internal sealed class EditorTextures : MonoBehaviour
{
	private Color colorForEraser = new Color(1f, 1f, 1f, 1f);

	public UITexture canvasTexture;

	private bool isMouseDown;

	private Vector2 oldEditPixelPos = new Vector2(-1f, -1f);

	private bool isSetNewTexture;

	public UITexture fonCanvas;

	public ButtonHandler prevHistoryButton;

	public ButtonHandler nextHistoryButton;

	private UIButton prevHistoryUIButton;

	private UIButton nextHistoryUIButton;

	public ArrayList arrHistory = new ArrayList();

	public int currentHistoryIndex;

	private bool saveToHistory;

	public GameObject saveFrame;

	private bool symmetry;

	private void Start()
	{
		if (prevHistoryButton != null)
		{
			prevHistoryButton.Clicked += HandlePrevHistoryButtonClicked;
			prevHistoryUIButton = prevHistoryButton.gameObject.GetComponent<UIButton>();
		}
		if (nextHistoryButton != null)
		{
			nextHistoryButton.Clicked += HandleNextHistoryButtonClicked;
			nextHistoryUIButton = nextHistoryButton.gameObject.GetComponent<UIButton>();
		}
	}

	public void ToggleSymmetry(bool isSymmetry)
	{
		symmetry = isSymmetry;
	}

	public void SetStartCanvas(Texture2D _texure)
	{
		canvasTexture.mainTexture = CreateCopyTexture(CreateCopyTexture(_texure));
		float num = 400f / (float)canvasTexture.mainTexture.width;
		float num2 = 400f / (float)canvasTexture.mainTexture.height;
		int num3 = ((!(num < num2)) ? Mathf.RoundToInt(num2) : Mathf.RoundToInt(num));
		canvasTexture.width = canvasTexture.mainTexture.width * num3;
		canvasTexture.height = canvasTexture.mainTexture.height * num3;
		UpdateFonCanvas();
		arrHistory.Clear();
		AddCanvasTextureInHistory();
	}

	private void HandlePrevHistoryButtonClicked(object sender, EventArgs e)
	{
		if (currentHistoryIndex > 0)
		{
			currentHistoryIndex--;
		}
		UpdateTextureFromHistory();
	}

	private void HandleNextHistoryButtonClicked(object sender, EventArgs e)
	{
		if (currentHistoryIndex < arrHistory.Count - 1)
		{
			currentHistoryIndex++;
		}
		UpdateTextureFromHistory();
	}

	private void UpdateTextureFromHistory()
	{
		canvasTexture.mainTexture = CreateCopyTexture((Texture2D)arrHistory[currentHistoryIndex]);
	}

	public void AddCanvasTextureInHistory()
	{
		while (currentHistoryIndex < arrHistory.Count - 1)
		{
			arrHistory.RemoveAt(arrHistory.Count - 1);
		}
		arrHistory.Add(CreateCopyTexture((Texture2D)canvasTexture.mainTexture));
		if (arrHistory.Count > 30)
		{
			arrHistory.RemoveAt(0);
		}
		currentHistoryIndex = arrHistory.Count - 1;
	}

	private void Update()
	{
		if (prevHistoryUIButton != null && nextHistoryUIButton != null)
		{
			if (prevHistoryUIButton.isEnabled != (currentHistoryIndex != 0))
			{
				prevHistoryUIButton.isEnabled = currentHistoryIndex != 0;
			}
			if (nextHistoryUIButton.isEnabled != currentHistoryIndex < arrHistory.Count - 1)
			{
				nextHistoryUIButton.isEnabled = currentHistoryIndex < arrHistory.Count - 1;
			}
		}
		int touchCount = Input.touchCount;
		Touch touch = ((touchCount <= 0) ? default(Touch) : Input.GetTouch(0));
		if (isMouseDown && ((touchCount > 0 && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)) || Input.GetMouseButtonUp(0)))
		{
			Vector2 pos = ((touchCount <= 0) ? new Vector2(Input.mousePosition.x, Input.mousePosition.y) : new Vector2(touch.position.x, touch.position.y));
			if (IsCanvasConteinPosition(pos))
			{
				OnCanvasClickUp();
			}
			isMouseDown = false;
			oldEditPixelPos = new Vector2(-1f, -1f);
			AddCanvasTextureInHistory();
		}
		if (isSetNewTexture)
		{
			isSetNewTexture = false;
			UpdateFonCanvas();
		}
		if ((touchCount <= 0 || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && !isMouseDown && !Input.GetMouseButtonDown(0))
		{
			return;
		}
		Vector2 pos2 = ((touchCount <= 0) ? new Vector2(Input.mousePosition.x, Input.mousePosition.y) : new Vector2(touch.position.x, touch.position.y));
		if (IsCanvasConteinPosition(pos2))
		{
			isMouseDown = true;
			Vector2 editPixelPos = GetEditPixelPos(pos2);
			if (!editPixelPos.Equals(oldEditPixelPos))
			{
				oldEditPixelPos = editPixelPos;
				EditCanvas(editPixelPos);
			}
		}
	}

	private void EditCanvas(Vector2 pos)
	{
		if (saveFrame != null && saveFrame.activeSelf)
		{
			return;
		}
		if (SkinEditorController.brashMode == SkinEditorController.BrashMode.Pipette)
		{
			if (SkinEditorController.sharedController != null)
			{
				SkinEditorController.sharedController.newColor.color = ((Texture2D)canvasTexture.mainTexture).GetPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
				SkinEditorController.sharedController.HandleSetColorClicked(null, null);
			}
			return;
		}
		SkinEditorController.isEditingPartSkin = true;
		Texture2D texture2D = CreateCopyTexture(canvasTexture.mainTexture as Texture2D);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		if (SkinEditorController.brashMode == SkinEditorController.BrashMode.Pencil)
		{
			texture2D.SetPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), SkinEditorController.colorForPaint);
			if (symmetry)
			{
				texture2D.SetPixel(texture2D.width - 1 - Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), SkinEditorController.colorForPaint);
			}
		}
		if (SkinEditorController.brashMode == SkinEditorController.BrashMode.Brash)
		{
			texture2D.SetPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), SkinEditorController.colorForPaint);
			if (Mathf.RoundToInt(pos.x) > 0)
			{
				texture2D.SetPixel(Mathf.RoundToInt(pos.x) - 1, Mathf.RoundToInt(pos.y), SkinEditorController.colorForPaint);
			}
			if (Mathf.RoundToInt(pos.x) < texture2D.width - 1)
			{
				texture2D.SetPixel(Mathf.RoundToInt(pos.x) + 1, Mathf.RoundToInt(pos.y), SkinEditorController.colorForPaint);
			}
			if (Mathf.RoundToInt(pos.y) > 0)
			{
				texture2D.SetPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y) - 1, SkinEditorController.colorForPaint);
			}
			if (Mathf.RoundToInt(pos.y) < texture2D.height - 1)
			{
				texture2D.SetPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y) + 1, SkinEditorController.colorForPaint);
			}
			if (symmetry)
			{
				texture2D.SetPixel(texture2D.width - 1 - Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), SkinEditorController.colorForPaint);
				if (Mathf.RoundToInt(pos.x) > 0)
				{
					texture2D.SetPixel(texture2D.width - 1 - Mathf.RoundToInt(pos.x) + 1, Mathf.RoundToInt(pos.y), SkinEditorController.colorForPaint);
				}
				if (Mathf.RoundToInt(pos.x) < texture2D.width - 1)
				{
					texture2D.SetPixel(texture2D.width - 1 - Mathf.RoundToInt(pos.x) - 1, Mathf.RoundToInt(pos.y), SkinEditorController.colorForPaint);
				}
				if (Mathf.RoundToInt(pos.y) > 0)
				{
					texture2D.SetPixel(texture2D.width - 1 - Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y) - 1, SkinEditorController.colorForPaint);
				}
				if (Mathf.RoundToInt(pos.y) < texture2D.height - 1)
				{
					texture2D.SetPixel(texture2D.width - 1 - Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y) + 1, SkinEditorController.colorForPaint);
				}
			}
		}
		if (SkinEditorController.brashMode == SkinEditorController.BrashMode.Eraser)
		{
			texture2D.SetPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), colorForEraser);
		}
		if (SkinEditorController.brashMode == SkinEditorController.BrashMode.Fill)
		{
			int num = Mathf.RoundToInt(pos.x) + Mathf.RoundToInt(pos.y) * texture2D.width;
			Color[] pixels = texture2D.GetPixels();
			Color color = pixels[num];
			if (color != SkinEditorController.colorForPaint)
			{
				int item = num;
				List<int> list = new List<int>();
				list.Add(item);
				while (list.Count > 0)
				{
					int num2 = Mathf.FloorToInt((float)list[0] / (float)texture2D.width);
					int num3 = list[0] - num2 * texture2D.width;
					pixels[list[0]] = SkinEditorController.colorForPaint;
					list.RemoveAt(0);
					if (num3 + 1 < texture2D.width && pixels[num3 + 1 + num2 * texture2D.width] == color && !list.Contains(num3 + 1 + num2 * texture2D.width))
					{
						list.Add(num3 + 1 + num2 * texture2D.width);
					}
					if (num3 - 1 >= 0 && pixels[num3 - 1 + num2 * texture2D.width] == color && !list.Contains(num3 - 1 + num2 * texture2D.width))
					{
						list.Add(num3 - 1 + num2 * texture2D.width);
					}
					if (num2 + 1 < texture2D.height && pixels[num3 + (num2 + 1) * texture2D.width] == color && !list.Contains(num3 + (num2 + 1) * texture2D.width))
					{
						list.Add(num3 + (num2 + 1) * texture2D.width);
					}
					if (num2 - 1 >= 0 && pixels[num3 + (num2 - 1) * texture2D.width] == color && !list.Contains(num3 + (num2 - 1) * texture2D.width))
					{
						list.Add(num3 + (num2 - 1) * texture2D.width);
					}
				}
				texture2D.SetPixels(pixels);
			}
		}
		texture2D.Apply();
		isSetNewTexture = true;
		canvasTexture.mainTexture = texture2D;
	}

	private void OnCanvasClickUp()
	{
		if ((!(saveFrame != null) || !saveFrame.activeSelf) && SkinEditorController.brashMode == SkinEditorController.BrashMode.Pipette && SkinEditorController.sharedController != null)
		{
			SkinEditorController.sharedController.SetColorClickedUp();
		}
	}

	private bool IsCanvasConteinPosition(Vector2 pos)
	{
		bool flag = false;
		float num = (float)Screen.height / 768f;
		Vector2 vector = new Vector2(((float)Screen.width - num * (float)canvasTexture.width) * 0.5f, ((float)Screen.height + num * (float)canvasTexture.height) * 0.5f);
		Vector2 vector2 = new Vector2(((float)Screen.width + num * (float)canvasTexture.width) * 0.5f, ((float)Screen.height - num * (float)canvasTexture.height) * 0.5f);
		if (pos.x > vector.x && pos.x < vector2.x && pos.y < vector.y && pos.y > vector2.y)
		{
			return true;
		}
		return false;
	}

	private Vector2 GetEditPixelPos(Vector2 pos)
	{
		float num = (float)Screen.height / 768f;
		return new Vector2(Mathf.FloorToInt((pos.x - ((float)Screen.width - (float)canvasTexture.width * num) * 0.5f) / ((float)canvasTexture.width * num) * (float)canvasTexture.mainTexture.width), Mathf.FloorToInt((pos.y - ((float)Screen.height - (float)canvasTexture.height * num) * 0.5f) / ((float)canvasTexture.height * num) * (float)canvasTexture.mainTexture.height));
	}

	public static Texture2D CreateCopyTexture(Texture tekTexture)
	{
		return CreateCopyTexture((Texture2D)tekTexture);
	}

	public static Texture2D CreateCopyTexture(Texture2D tekTexture)
	{
		Texture2D texture2D = new Texture2D(tekTexture.width, tekTexture.height, TextureFormat.RGBA32, false);
		texture2D.SetPixels(tekTexture.GetPixels());
		texture2D.filterMode = FilterMode.Point;
		texture2D.Apply();
		return texture2D;
	}

	public void UpdateFonCanvas()
	{
		fonCanvas.width = canvasTexture.width;
		fonCanvas.height = canvasTexture.height;
		fonCanvas.mainTexture = canvasTexture.mainTexture;
	}
}
