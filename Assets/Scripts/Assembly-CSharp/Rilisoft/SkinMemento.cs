using System;
using System.Globalization;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public struct SkinMemento : IEquatable<SkinMemento>
	{
		[SerializeField]
		private string id;

		[SerializeField]
		private string name;

		[SerializeField]
		private string skin;

		private int? _skinHashCode;

		public string Id
		{
			get
			{
				return id ?? string.Empty;
			}
		}

		public string Name
		{
			get
			{
				return name ?? string.Empty;
			}
		}

		public string Skin
		{
			get
			{
				return skin ?? string.Empty;
			}
		}

		public SkinMemento(string id, string name, string skin)
		{
			this.id = id ?? string.Empty;
			this.name = name ?? string.Empty;
			this.skin = skin ?? string.Empty;
			_skinHashCode = null;
		}

		public bool Equals(SkinMemento other)
		{
			if (Id != other.Id)
			{
				return false;
			}
			if (Name != other.Name)
			{
				return false;
			}
			if (GetSkinHashCode() != other.GetSkinHashCode())
			{
				return false;
			}
			if (Skin != other.Skin)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SkinMemento))
			{
				return false;
			}
			SkinMemento other = (SkinMemento)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Name.GetHashCode() ^ GetSkinHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{{ \"id\":{0},\"name\":{1},\"skin\":{2} }}", Id, Name, Skin);
		}

		private int GetSkinHashCode()
		{
			if (!_skinHashCode.HasValue)
			{
				_skinHashCode = Skin.GetHashCode();
			}
			return _skinHashCode.Value;
		}
	}
}
