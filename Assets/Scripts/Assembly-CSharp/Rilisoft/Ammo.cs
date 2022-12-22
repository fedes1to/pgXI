using System;

namespace Rilisoft
{
	public struct Ammo : IEquatable<Ammo>
	{
		private readonly int _backpack;

		private readonly int _clip;

		public int Backpack
		{
			get
			{
				return _backpack;
			}
		}

		public int Clip
		{
			get
			{
				return _clip;
			}
		}

		public Ammo(int clip, int backpack)
		{
			_clip = clip;
			_backpack = backpack;
		}

		public bool Equals(Ammo other)
		{
			if (Backpack != other.Backpack)
			{
				return false;
			}
			if (Clip != other.Clip)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Ammo))
			{
				return false;
			}
			return ((Ammo)obj).Equals(this);
		}

		public override int GetHashCode()
		{
			return _backpack.GetHashCode() ^ _clip.GetHashCode();
		}

		public override string ToString()
		{
			return Clip + "/" + Backpack;
		}
	}
}
