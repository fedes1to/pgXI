using System;

namespace Rilisoft
{
	public sealed class AdRequestException : Exception
	{
		public AdRequestException(string message)
			: base(message)
		{
		}

		public AdRequestException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
