using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class TrainingBlinking : MonoBehaviour
	{
		private readonly List<KeyValuePair<UISprite, Color>> _sprites = new List<KeyValuePair<UISprite, Color>>(5);

		public void SetSprites(IList<UISprite> sprites)
		{
			RestoreColorTints();
			_sprites.Clear();
			if (sprites == null)
			{
				return;
			}
			foreach (UISprite sprite in sprites)
			{
				if (!(sprite == null))
				{
					_sprites.Add(new KeyValuePair<UISprite, Color>(sprite, sprite.color));
				}
			}
		}

		private void OnDisable()
		{
			RestoreColorTints();
			_sprites.Clear();
			Object.Destroy(this);
		}

		private void OnDestroy()
		{
			RestoreColorTints();
			_sprites.Clear();
		}

		private void Update()
		{
			float interpolationCoefficient = GetInterpolationCoefficient(Time.time);
			foreach (KeyValuePair<UISprite, Color> sprite in _sprites)
			{
				sprite.Key.color = Color.Lerp(sprite.Value, Color.green, interpolationCoefficient);
			}
		}

		private static float GetInterpolationCoefficient(float time)
		{
			float num = time - Mathf.Floor(time);
			return 1f - num * num;
		}

		private void RestoreColorTints()
		{
			foreach (KeyValuePair<UISprite, Color> sprite in _sprites)
			{
				sprite.Key.color = sprite.Value;
			}
		}
	}
}
