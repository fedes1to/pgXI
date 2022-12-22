using System.Collections.Generic;

namespace Rilisoft.Details
{
	internal sealed class ByteArrayComparer : IEqualityComparer<byte[]>
	{
		public bool Equals(byte[] x, byte[] y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i != x.Length; i++)
			{
				if (x[i] != y[i])
				{
					return false;
				}
			}
			return true;
		}

		public int GetHashCode(byte[] obj)
		{
			if (obj == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i != obj.Length; i++)
			{
				int num2 = i % 4;
				num ^= obj[i] << num2;
			}
			return num;
		}
	}
}
