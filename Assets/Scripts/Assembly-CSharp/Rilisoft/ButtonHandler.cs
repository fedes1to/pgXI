using System;
using UnityEngine;

namespace Rilisoft
{
	public sealed class ButtonHandler : MonoBehaviour
	{
		public bool noSound;

		[NonSerialized]
		public bool isEnable = true;

		public bool HasClickedHandlers
		{
			get
			{
				return this.Clicked != null;
			}
		}

		public event EventHandler Clicked;

		private void OnClick()
		{
			if (isEnable)
			{
				if (ButtonClickSound.Instance != null && !noSound)
				{
					ButtonClickSound.Instance.PlayClick();
				}
				EventHandler clicked = this.Clicked;
				if (clicked != null)
				{
					clicked(this, EventArgs.Empty);
				}
			}
		}

		public void DoClick()
		{
			if (isEnable)
			{
				OnClick();
			}
		}
	}
}
