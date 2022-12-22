using UnityEngine;

namespace Rilisoft
{
	internal sealed class SpriteAnimation : UISpriteAnimation
	{
		public bool SnapPixels
		{
			get
			{
				return mSnap;
			}
			set
			{
				mSnap = value;
			}
		}

		protected override void Update()
		{
			if (Application.isPlaying && base.isPlaying && base.frames >= 2 && !((float)base.framesPerSecond <= 0f))
			{
				int num = Mathf.FloorToInt(Time.realtimeSinceStartup * (float)base.framesPerSecond);
				mIndex = num % base.frames;
				mSprite.spriteName = mSpriteNames[mIndex];
				if (mSnap)
				{
					mSprite.MakePixelPerfect();
				}
			}
		}
	}
}
