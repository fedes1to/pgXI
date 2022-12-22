using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class FogSettings
	{
		public bool Active;

		public FogMode Mode;

		public Color Color;

		public float Start;

		public float End;

		public FogSettings FromCurrent()
		{
			Active = RenderSettings.fog;
			Mode = RenderSettings.fogMode;
			Color = RenderSettings.fogColor;
			Start = RenderSettings.fogStartDistance;
			End = RenderSettings.fogEndDistance;
			return this;
		}

		public void SetToCurrent()
		{
			RenderSettings.fog = Active;
			RenderSettings.fogMode = Mode;
			RenderSettings.fogColor = Color;
			RenderSettings.fogStartDistance = Start;
			RenderSettings.fogEndDistance = End;
		}
	}
}
