using System;

namespace Rilisoft
{
	public sealed class AdNotAwailableException : Exception
	{
		public AdNotAwailableException(string message)
			: base(message)
		{
		}
	}
}
