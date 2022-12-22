using System;

namespace Rilisoft
{
	internal struct ProfilerSample : IDisposable, IEquatable<ProfilerSample>
	{
		private bool _disposed;

		public ProfilerSample(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			_disposed = false;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
			}
		}

		public bool Equals(ProfilerSample other)
		{
			return true;
		}
	}
}
