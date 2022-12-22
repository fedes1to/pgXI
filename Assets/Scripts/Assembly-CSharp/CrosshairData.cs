using System;
using UnityEngine;

[Serializable]
public class CrosshairData
{
	[Serializable]
	public class aimSprite
	{
		public string spriteName;

		public Vector2 spriteSize;

		public Vector2 offset;

		public aimSprite(string name, Vector2 size, Vector2 pos)
		{
			spriteName = name;
			spriteSize = size;
			offset = pos;
		}
	}

	public int ID;

	public string Name;

	public Texture2D PreviewTexture;

	public aimSprite center = new aimSprite("aim_1", new Vector2(6f, 6f), new Vector2(0f, 0f));

	public aimSprite up = new aimSprite("aim_2", new Vector2(6f, 10f), new Vector2(0f, 5f));

	public aimSprite leftUp = new aimSprite(string.Empty, new Vector2(12f, 12f), new Vector2(8f, 8f));

	public aimSprite left = new aimSprite("aim_2", new Vector2(10f, 6f), new Vector2(5f, 0f));

	public aimSprite leftDown = new aimSprite(string.Empty, new Vector2(0f, 0f), new Vector2(0f, 0f));

	public aimSprite down = new aimSprite("aim_3", new Vector2(6f, 10f), new Vector2(0f, 5f));
}
