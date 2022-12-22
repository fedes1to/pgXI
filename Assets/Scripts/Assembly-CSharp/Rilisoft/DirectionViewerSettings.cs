using System;

namespace Rilisoft
{
	[Serializable]
	public class DirectionViewerSettings
	{
		public DirectionViewTargetType ForType;

		public float LookRadius = 10f;

		public float CircleRadius = 200f;
	}
}
