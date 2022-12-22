using System;

namespace Rilisoft
{
	internal interface IAead
	{
		int MaxOverhead { get; }

		int MinOverhead { get; }

		bool Authenticate(ArraySegment<byte> taggedCiphertext, byte[] salt);

		ArraySegment<byte> Decrypt(ArraySegment<byte> taggedCiphertext, byte[] salt, byte[] outputBuffer);

		ArraySegment<byte> Encrypt(ArraySegment<byte> plaintext, byte[] salt, byte[] outputBuffer);
	}
}
