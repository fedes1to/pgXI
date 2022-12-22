using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class WeaponSkinTexture
	{
		[StringTexture]
		public string Raw;

		public int W;

		public int H;

		public FilterMode FilterMode;

		public string[] ToObjects = new string[0];

		public string ShaderPropertyName = "_MainTex";

		private Texture2D _texture;

		public Texture2D Texture
		{
			get
			{
				if (_texture == null)
				{
					byte[] data = Convert.FromBase64String(Raw);
					_texture = new Texture2D(W, H, TextureFormat.RGBA32, false);
					_texture.LoadImage(data);
					_texture.filterMode = FilterMode;
					_texture.Apply();
				}
				return _texture;
			}
		}

		public WeaponSkinTexture(string raw, int w, int h, string[] toObjects = null)
		{
			Raw = raw;
			W = w;
			H = h;
			ToObjects = toObjects;
		}
	}
}
