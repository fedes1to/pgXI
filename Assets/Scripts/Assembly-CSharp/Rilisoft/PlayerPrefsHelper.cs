using System;
using System.Security.Cryptography;
using System.Text;

namespace Rilisoft
{
	public sealed class PlayerPrefsHelper : IDisposable
	{
		private bool _disposed;

		private readonly HMAC _hmac;

		private readonly string _hmacPrefsKey;

		internal PlayerPrefsHelper()
		{
			using (HashAlgorithm hashAlgorithm = new SHA256Managed())
			{
				byte[] bytes = Encoding.UTF8.GetBytes("PrefsKey");
				byte[] array = hashAlgorithm.ComputeHash(bytes);
				_hmacPrefsKey = BitConverter.ToString(array).Replace("-", string.Empty);
				_hmacPrefsKey = _hmacPrefsKey.Substring(0, Math.Min(32, _hmacPrefsKey.Length)).ToLower();
				byte[] bytes2 = Encoding.UTF8.GetBytes("HmacKey");
				byte[] key = hashAlgorithm.ComputeHash(bytes2);
				_hmac = new HMACSHA256(key);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_hmac.Clear();
				_disposed = true;
			}
		}
	}
}
