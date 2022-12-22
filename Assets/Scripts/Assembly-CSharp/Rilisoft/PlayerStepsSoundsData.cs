using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class PlayerStepsSoundsData
	{
		public AudioClip Walk;

		public AudioClip Jump;

		public AudioClip JumpDown;

		public AudioClip WalkMech;

		public AudioClip WalkMechBear;

		public void SetTo(SkinName data)
		{
			data.walkAudio = Walk;
			data.jumpAudio = Jump;
			data.jumpDownAudio = JumpDown;
			data.walkMech = WalkMech;
			data.walkMechBear = WalkMechBear;
		}

		public bool IsSettedTo(SkinName data)
		{
			return data.walkAudio == Walk && data.jumpAudio == Jump && data.jumpDownAudio == JumpDown && data.walkMech == WalkMech && data.walkMechBear == WalkMechBear;
		}

		public static PlayerStepsSoundsData Create(SkinName data)
		{
			PlayerStepsSoundsData playerStepsSoundsData = new PlayerStepsSoundsData();
			playerStepsSoundsData.Walk = data.walkAudio;
			playerStepsSoundsData.Jump = data.jumpAudio;
			playerStepsSoundsData.JumpDown = data.jumpDownAudio;
			playerStepsSoundsData.WalkMech = data.walkMech;
			playerStepsSoundsData.WalkMechBear = data.walkMechBear;
			return playerStepsSoundsData;
		}
	}
}
