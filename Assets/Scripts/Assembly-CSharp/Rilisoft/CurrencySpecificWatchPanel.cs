using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	internal sealed class CurrencySpecificWatchPanel : MonoBehaviour
	{
		[SerializeField]
		private UILabel watchHeader;

		[SerializeField]
		private UILabel watchTimer;

		[SerializeField]
		private UIButton watchButton;

		public UILabel WatchHeader
		{
			get
			{
				return watchHeader;
			}
		}

		public UILabel WatchTimer
		{
			get
			{
				return watchTimer;
			}
		}

		public UIButton WatchButton
		{
			get
			{
				return watchButton;
			}
		}
	}
}
