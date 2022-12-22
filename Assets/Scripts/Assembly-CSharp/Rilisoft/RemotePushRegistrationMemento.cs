using System;
using System.Globalization;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	internal struct RemotePushRegistrationMemento : IEquatable<RemotePushRegistrationMemento>
	{
		[SerializeField]
		private string registrationId;

		[SerializeField]
		private string registrationTime;

		[SerializeField]
		private string version;

		public string RegistrationId
		{
			get
			{
				return registrationId ?? string.Empty;
			}
		}

		public string RegistrationTime
		{
			get
			{
				return registrationTime ?? string.Empty;
			}
		}

		public string Version
		{
			get
			{
				return version ?? string.Empty;
			}
		}

		public RemotePushRegistrationMemento(string registrationId, DateTime registrationTime, string version)
		{
			this.registrationId = registrationId ?? string.Empty;
			this.registrationTime = registrationTime.ToString("s", CultureInfo.InvariantCulture);
			this.version = version ?? string.Empty;
		}

		public bool Equals(RemotePushRegistrationMemento other)
		{
			if (RegistrationTime != other.RegistrationTime)
			{
				return false;
			}
			if (Version != other.Version)
			{
				return false;
			}
			if (RegistrationId != other.RegistrationId)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RemotePushRegistrationMemento))
			{
				return false;
			}
			RemotePushRegistrationMemento other = (RemotePushRegistrationMemento)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return RegistrationTime.GetHashCode() ^ Version.GetHashCode() ^ RegistrationId.GetHashCode();
		}
	}
}
