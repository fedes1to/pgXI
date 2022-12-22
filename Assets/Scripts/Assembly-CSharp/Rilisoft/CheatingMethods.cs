using System;

namespace Rilisoft
{
	[Flags]
	internal enum CheatingMethods
	{
		None = 0,
		SignatureTampering = 1,
		CoinThreshold = 2,
		GemThreshold = 4
	}
}
