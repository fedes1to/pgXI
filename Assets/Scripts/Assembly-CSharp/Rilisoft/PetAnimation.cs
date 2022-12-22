using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class PetAnimation
	{
		[SerializeField]
		public PetAnimationType Type;

		[SerializeField]
		public string AnimationName;

		[SerializeField]
		public float SpeedModificator = 1f;

		[SerializeField]
		[ReadOnly]
		public float CurrentPlaySpeed;
	}
}
