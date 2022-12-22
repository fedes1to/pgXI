using System;
using System.IO;
using System.Security.Cryptography;

namespace Rilisoft
{
	internal sealed class AesFacade
	{
		private readonly SymmetricAlgorithm _aes;

		public AesFacade(byte[] masterKey)
		{
			if (masterKey == null)
			{
				throw new ArgumentNullException("masterKey");
			}
			byte[] salt = new byte[8] { 76, 130, 161, 24, 36, 100, 21, 150 };
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(masterKey, salt, 1);
			byte[] bytes = rfc2898DeriveBytes.GetBytes(48);
			byte[] array = new byte[32];
			Array.Copy(bytes, 0, array, 0, array.Length);
			byte[] array2 = new byte[16];
			Array.Copy(bytes, array.Length, array2, 0, array2.Length);
			_aes = Rijndael.Create();
			_aes.KeySize = 256;
			_aes.BlockSize = 128;
			_aes.Key = array;
			_aes.IV = array2;
			_aes.Mode = CipherMode.CFB;
			_aes.Padding = PaddingMode.PKCS7;
		}

		public byte[] Decrypt(byte[] ciphertext)
		{
			//Discarded unreachable code: IL_004c, IL_005e, IL_0070
			if (ciphertext == null)
			{
				throw new ArgumentNullException("ciphertext");
			}
			using (ICryptoTransform transform = _aes.CreateDecryptor())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(ciphertext, 0, ciphertext.Length);
						cryptoStream.Close();
						return memoryStream.ToArray();
					}
				}
			}
		}

		public byte[] Encrypt(byte[] plaintext)
		{
			//Discarded unreachable code: IL_004c, IL_005e, IL_0070
			if (plaintext == null)
			{
				throw new ArgumentNullException("plaintext");
			}
			using (ICryptoTransform transform = _aes.CreateEncryptor())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(plaintext, 0, plaintext.Length);
						cryptoStream.Close();
						return memoryStream.ToArray();
					}
				}
			}
		}
	}
}
