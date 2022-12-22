using System;

namespace Rilisoft
{
	internal class FinishedEventArgs : EventArgs
	{
		public static readonly FinishedEventArgs Success = new FinishedEventArgs(true);

		public static readonly FinishedEventArgs Failure = new FinishedEventArgs(false);

		private readonly bool _succeeded;

		public bool Succeeded
		{
			get
			{
				return _succeeded;
			}
		}

		public FinishedEventArgs(bool succeeded)
		{
			_succeeded = succeeded;
		}
	}
}
