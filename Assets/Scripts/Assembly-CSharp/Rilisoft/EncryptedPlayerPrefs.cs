using System;
using System.Collections.Generic;
using System.Text;
using Rilisoft.Details;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class EncryptedPlayerPrefs
	{
		private static readonly UTF8Encoding s_defaultEncoding = new UTF8Encoding(false, false);

		private readonly Dictionary<byte[], string> _wrappedKeys = new Dictionary<byte[], string>(new ByteArrayComparer());

		private WeakReference _bufferWeakReference;

		private readonly AesFacade _aesFacade;

		private readonly DefaultAead _aead;

		public EncryptedPlayerPrefs(byte[] masterKey)
		{
			if (masterKey == null)
			{
				throw new ArgumentNullException("masterKey");
			}
			_aesFacade = new AesFacade(masterKey);
			_aead = new DefaultAead(masterKey);
		}

		public void ClearChache()
		{
			_bufferWeakReference = null;
			_wrappedKeys.Clear();
		}

		public void DeleteKey(string key)
		{
			if (key == null)
			{
				return;
			}
			try
			{
				string key2 = WrapKey(key);
				PlayerPrefs.DeleteKey(key2);
			}
			catch (Exception message)
			{
				Debug.LogWarningFormat("`DeleteKey()` failed with key: `{0}`", key);
				Debug.LogWarning(message);
			}
		}

		public bool HasKey(string key)
		{
			//Discarded unreachable code: IL_001c, IL_0043
			if (key == null)
			{
				return false;
			}
			try
			{
				string key2 = WrapKey(key);
				return PlayerPrefs.HasKey(key2);
			}
			catch (Exception message)
			{
				Debug.LogWarningFormat("`HasKey()` failed with key: `{0}`", key);
				Debug.LogWarning(message);
				return false;
			}
		}

		public string GetString(string key)
		{
			//Discarded unreachable code: IL_00d5, IL_00e7, IL_0115
			if (key == null)
			{
				return string.Empty;
			}
			byte[] bytes = s_defaultEncoding.GetBytes(key);
			try
			{
				string key2 = WrapKey(bytes);
				string @string = PlayerPrefs.GetString(key2);
				if (string.IsNullOrEmpty(@string))
				{
					return string.Empty;
				}
				byte[] array = Convert.FromBase64String(@string);
				if (array.Length - _aead.MinOverhead < 0)
				{
					return string.Empty;
				}
				byte[] buffer = GetBuffer(array.Length - _aead.MinOverhead);
				try
				{
					ArraySegment<byte> arraySegment = _aead.Decrypt(new ArraySegment<byte>(array), bytes, buffer);
					if (arraySegment.Array == null)
					{
						return string.Empty;
					}
					return s_defaultEncoding.GetString(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
				}
				finally
				{
					Array.Clear(buffer, 0, buffer.Length);
				}
			}
			catch (Exception message)
			{
				Debug.LogWarningFormat("`GetString()` failed with key: `{0}`", key);
				Debug.LogWarning(message);
				return string.Empty;
			}
		}

		public void SetString(string key, string value)
		{
			if (key == null)
			{
				return;
			}
			byte[] bytes = s_defaultEncoding.GetBytes(key);
			try
			{
				string key2 = WrapKey(bytes);
				byte[] bytes2 = s_defaultEncoding.GetBytes(value);
				byte[] buffer = GetBuffer(bytes2.Length + _aead.MaxOverhead);
				try
				{
					ArraySegment<byte> arraySegment = _aead.Encrypt(new ArraySegment<byte>(bytes2), bytes, buffer);
					string value2 = Convert.ToBase64String(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
					PlayerPrefs.SetString(key2, value2);
				}
				finally
				{
					Array.Clear(buffer, 0, buffer.Length);
				}
			}
			catch (Exception message)
			{
				Debug.LogWarningFormat("`SetString()` failed with key: `{0}`", key);
				Debug.LogWarning(message);
			}
		}

		private string WrapKey(string key)
		{
			byte[] bytes = s_defaultEncoding.GetBytes(key);
			return WrapKey(bytes);
		}

		private string WrapKey(byte[] keyBytes)
		{
			//Discarded unreachable code: IL_005e, IL_0092
			string value;
			if (_wrappedKeys.TryGetValue(keyBytes, out value))
			{
				return value;
			}
			byte[] buffer = GetBuffer(keyBytes.Length + _aead.MaxOverhead);
			try
			{
				byte[] inArray = _aesFacade.Encrypt(keyBytes);
				string text = "AEAD:" + Convert.ToBase64String(inArray);
				_wrappedKeys[keyBytes] = text;
				return text;
			}
			catch (Exception message)
			{
				string text2 = Convert.ToBase64String(keyBytes);
				Debug.LogWarningFormat("`WrapKey()` failed with key bytes: `{0}`", text2);
				Debug.LogWarning(message);
				return text2;
			}
			finally
			{
				Array.Clear(buffer, 0, buffer.Length);
			}
		}

		private byte[] GetBuffer(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", length, "Must be non-negative.");
			}
			if (_bufferWeakReference != null && _bufferWeakReference.IsAlive)
			{
				byte[] array = (byte[])_bufferWeakReference.Target;
				if (array.Length >= length)
				{
					return array;
				}
			}
			byte[] array2 = new byte[length];
			_bufferWeakReference = new WeakReference(array2, false);
			return array2;
		}
	}
}
