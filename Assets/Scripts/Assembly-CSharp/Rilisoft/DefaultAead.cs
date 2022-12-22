using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Rilisoft
{
	internal sealed class DefaultAead : IAead
	{
		private sealed class CustomRandomNumberGenerator
		{
			private readonly RNGCryptoServiceProvider _prng = new RNGCryptoServiceProvider();

			public void GetBytes(byte[] data)
			{
				if (data != null && data.Length != 0)
				{
					_prng.GetBytes(data);
				}
			}
		}

		private const int AesBlockLength = 16;

		private const int AesKeyLength = 32;

		private const int TagLength = 20;

		private static readonly byte[] s_emptyByteArray = new byte[0];

		private readonly byte[] _aesKeyTweak;

		private readonly byte[] _pbkdfSalt;

		private readonly byte[] _aesKeyBuffer;

		private readonly byte[] _aesIVBuffer;

		private readonly SymmetricAlgorithm _aes;

		private readonly CustomRandomNumberGenerator _prng = new CustomRandomNumberGenerator();

		public int MaxOverhead
		{
			get
			{
				return NonceLength + 20 + 16;
			}
		}

		public int MinOverhead
		{
			get
			{
				return NonceLength + 20;
			}
		}

		private int AesIVLength
		{
			get
			{
				return 16;
			}
		}

		private int NonceLength
		{
			get
			{
				return 32 + AesIVLength;
			}
		}

		public DefaultAead(byte[] masterKey)
		{
			if (masterKey == null)
			{
				throw new ArgumentNullException("masterKey");
			}
			_aes = Rijndael.Create();
			_aes.KeySize = 256;
			_aes.BlockSize = 128;
			_aes.Mode = CipherMode.CFB;
			_aes.Padding = PaddingMode.PKCS7;
			int num = Math.Max(32, 8);
			if (masterKey.Length < num)
			{
				throw new ArgumentException("Master key should be at least " + num + " bytes.", "masterKey");
			}
			_aesKeyBuffer = new byte[32];
			_aesIVBuffer = new byte[AesIVLength];
			_aesKeyTweak = new byte[32];
			Array.Copy(masterKey, 0, _aesKeyTweak, 0, _aesKeyTweak.Length);
			_pbkdfSalt = new byte[8];
			Array.Copy(masterKey, masterKey.Length - 8, _pbkdfSalt, 0, _pbkdfSalt.Length);
		}

		public bool Authenticate(ArraySegment<byte> ciphertext, byte[] salt)
		{
			if (ciphertext.Count < NonceLength + 20)
			{
				return false;
			}
			byte[] macKey = GenerateMacKey(salt);
			return AuthenticateCore(ciphertext, macKey);
		}

		private bool AuthenticateCore(ArraySegment<byte> ciphertext, byte[] macKey)
		{
			ArraySegment<byte> arraySegment = new ArraySegment<byte>(ciphertext.Array, ciphertext.Count - 20, 20);
			byte[] array = null;
			using (HMACSHA1 hMACSHA = new HMACSHA1(macKey, true))
			{
				array = hMACSHA.ComputeHash(ciphertext.Array, 0, ciphertext.Count - 20);
			}
			return ConstantTimeEqual(arraySegment.Array, arraySegment.Offset, array, 0, array.Length);
		}

		public ArraySegment<byte> Decrypt(ArraySegment<byte> taggedCiphertext, byte[] salt, byte[] outputBuffer)
		{
			//Discarded unreachable code: IL_01a7, IL_01bb, IL_01cf, IL_01e3
			if (outputBuffer == null)
			{
				throw new ArgumentNullException("outputBuffer");
			}
			if (outputBuffer.Length < taggedCiphertext.Count - MinOverhead)
			{
				throw new ArgumentException("Output buffer is too short: " + outputBuffer.Length, "outputBuffer");
			}
			byte[] macKey = GenerateMacKey(salt);
			if (!AuthenticateCore(taggedCiphertext, macKey))
			{
				return default(ArraySegment<byte>);
			}
			try
			{
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(taggedCiphertext.Array, 0, 32);
				ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(taggedCiphertext.Array, arraySegment.Count, AesIVLength);
				ArraySegment<byte> arraySegment3 = new ArraySegment<byte>(taggedCiphertext.Array, NonceLength, taggedCiphertext.Count - NonceLength - 20);
				for (int i = 0; i != _aesKeyBuffer.Length; i++)
				{
					_aesKeyBuffer[i] = (byte)(_aesKeyTweak[i] ^ arraySegment.Array[arraySegment.Offset + i]);
				}
				Array.Copy(arraySegment2.Array, arraySegment2.Offset, _aesIVBuffer, 0, _aesIVBuffer.Length);
				using (ICryptoTransform transform = _aes.CreateDecryptor(_aesKeyBuffer, _aesIVBuffer))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
						{
							cryptoStream.Write(arraySegment3.Array, arraySegment3.Offset, arraySegment3.Count);
							cryptoStream.Close();
							byte[] array = memoryStream.ToArray();
							ArraySegment<byte> result = new ArraySegment<byte>(outputBuffer, 0, array.Length);
							Array.Copy(array, 0, result.Array, result.Offset, result.Count);
							return result;
						}
					}
				}
			}
			finally
			{
				Array.Clear(_aesKeyBuffer, 0, _aesKeyBuffer.Length);
				Array.Clear(_aesIVBuffer, 0, _aesIVBuffer.Length);
			}
		}

		public ArraySegment<byte> Encrypt(ArraySegment<byte> plaintext, byte[] salt, byte[] outputBuffer)
		{
			//Discarded unreachable code: IL_021a, IL_022e, IL_0242, IL_0254
			if (outputBuffer == null)
			{
				throw new ArgumentNullException("outputBuffer");
			}
			if (outputBuffer.Length < plaintext.Count + MaxOverhead)
			{
				throw new ArgumentException("Output buffer is too short: " + outputBuffer.Length, "outputBuffer");
			}
			try
			{
				_prng.GetBytes(_aesKeyBuffer);
				_prng.GetBytes(_aesIVBuffer);
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(outputBuffer, 0, 32);
				ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(outputBuffer, arraySegment.Count, AesIVLength);
				Array.Copy(_aesKeyBuffer, 0, arraySegment.Array, arraySegment.Offset, arraySegment.Count);
				Array.Copy(_aesIVBuffer, 0, arraySegment2.Array, arraySegment2.Offset, arraySegment2.Count);
				for (int i = 0; i != _aesKeyBuffer.Length; i++)
				{
					_aesKeyBuffer[i] = (byte)(_aesKeyTweak[i] ^ _aesKeyBuffer[i]);
				}
				using (ICryptoTransform transform = _aes.CreateEncryptor(_aesKeyBuffer, _aesIVBuffer))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
						{
							cryptoStream.Write(plaintext.Array, plaintext.Offset, plaintext.Count);
							cryptoStream.Close();
							byte[] array = memoryStream.ToArray();
							ArraySegment<byte> arraySegment3 = new ArraySegment<byte>(outputBuffer, NonceLength, array.Length);
							Array.Copy(array, 0, arraySegment3.Array, arraySegment3.Offset, arraySegment3.Count);
							ArraySegment<byte> result = new ArraySegment<byte>(outputBuffer, 0, NonceLength + arraySegment3.Count + 20);
							byte[] key = GenerateMacKey(salt);
							using (HMACSHA1 hMACSHA = new HMACSHA1(key, true))
							{
								ArraySegment<byte> arraySegment4 = new ArraySegment<byte>(outputBuffer, result.Count - 20, 20);
								byte[] sourceArray = hMACSHA.ComputeHash(result.Array, 0, NonceLength + arraySegment3.Count);
								Array.Copy(sourceArray, 0, arraySegment4.Array, arraySegment4.Offset, arraySegment4.Count);
							}
							return result;
						}
					}
				}
			}
			finally
			{
				Array.Clear(_aesKeyBuffer, 0, _aesKeyBuffer.Length);
				Array.Clear(_aesIVBuffer, 0, _aesIVBuffer.Length);
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		private static bool ConstantTimeEqual(byte[] left, int leftOffset, byte[] right, int rightOffset, int length)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (leftOffset < 0)
			{
				throw new ArgumentOutOfRangeException("leftOffset", "leftOffset < 0");
			}
			if (rightOffset < 0)
			{
				throw new ArgumentOutOfRangeException("rightOffset", "rightOffset < 0");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "length < 0");
			}
			if (leftOffset + length > left.Length)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "leftOffset + length > left.Length: {0} + {1} > {2}", leftOffset, length, left.Length));
			}
			if (rightOffset + length > right.Length)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "rightOffset + length > right.Length: {0} + {1} > {2}", rightOffset, length, right.Length));
			}
			int num = 0;
			for (int i = 0; i != length; i++)
			{
				num |= left[leftOffset + i] ^ right[rightOffset + i];
			}
			return num == 0;
		}

		private byte[] GenerateMacKey(byte[] salt)
		{
			byte[] password = salt ?? s_emptyByteArray;
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, _pbkdfSalt, 1);
			return rfc2898DeriveBytes.GetBytes(64);
		}
	}
}
