using System;
using System.Globalization;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public struct CapeMemento : IEquatable<CapeMemento>
	{
		[SerializeField]
		private long id;

		[SerializeField]
		private string cape;

		private int? _capeHashCode;

		public long Id
		{
			get
			{
				return id;
			}
		}

		public string Cape
		{
			get
			{
				return cape ?? string.Empty;
			}
		}

		public CapeMemento(long id, string cape)
		{
			this.id = id;
			this.cape = cape ?? string.Empty;
			_capeHashCode = null;
		}

		public bool Equals(CapeMemento other)
		{
			if (Id != other.Id)
			{
				return false;
			}
			if (GetCapeHashCode() != other.GetCapeHashCode())
			{
				return false;
			}
			if (Cape != other.Cape)
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
			SkinMemento skinMemento = (SkinMemento)obj;
			return Equals(skinMemento);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ GetCapeHashCode();
		}

		public override string ToString()
		{
			string text = ((Cape.Length > 4) ? Cape.Substring(Cape.Length - 4) : Cape);
			return string.Format(CultureInfo.InvariantCulture, "{{ \"id\":{0},\"cape\":\"{1}\" }}", Id, text);
		}

		internal static CapeMemento ChooseCape(CapeMemento left, CapeMemento right)
		{
			if (string.IsNullOrEmpty(left.Cape) && string.IsNullOrEmpty(right.Cape))
			{
				return (left.Id > right.Id) ? left : right;
			}
			if (!string.IsNullOrEmpty(left.Cape) && !string.IsNullOrEmpty(right.Cape))
			{
				return (left.Id > right.Id) ? left : right;
			}
			if (!string.IsNullOrEmpty(left.Cape))
			{
				return left;
			}
			return right;
		}

		private int GetCapeHashCode()
		{
			if (!_capeHashCode.HasValue)
			{
				_capeHashCode = Cape.GetHashCode();
			}
			return _capeHashCode.Value;
		}
	}
}
