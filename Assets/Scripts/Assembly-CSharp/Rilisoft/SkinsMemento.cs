using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public struct SkinsMemento : IEquatable<SkinsMemento>
	{
		private readonly bool _conflicted;

		[SerializeField]
		private CapeMemento cape;

		[SerializeField]
		private List<SkinMemento> skins;

		[SerializeField]
		private List<string> deletedSkins;

		public bool Conflicted
		{
			get
			{
				return _conflicted;
			}
		}

		public CapeMemento Cape
		{
			get
			{
				return cape;
			}
		}

		public List<SkinMemento> Skins
		{
			get
			{
				if (skins == null)
				{
					skins = new List<SkinMemento>();
				}
				return skins;
			}
		}

		public List<string> DeletedSkins
		{
			get
			{
				if (deletedSkins == null)
				{
					deletedSkins = new List<string>();
				}
				return deletedSkins;
			}
		}

		public SkinsMemento(IEnumerable<SkinMemento> skins, IEnumerable<string> deletedSkins, CapeMemento cape)
			: this(skins, deletedSkins, cape, false)
		{
		}

		public SkinsMemento(IEnumerable<SkinMemento> skins, IEnumerable<string> deletedSkins, CapeMemento cape, bool conflicted)
		{
			this.skins = ((skins != null) ? skins.ToList() : new List<SkinMemento>());
			this.deletedSkins = ((deletedSkins != null) ? deletedSkins.ToList() : new List<string>());
			this.cape = cape;
			_conflicted = conflicted;
		}

		public Dictionary<string, SkinMemento> GetSkinsAsDictionary()
		{
			Dictionary<string, SkinMemento> dictionary = new Dictionary<string, SkinMemento>(Skins.Count);
			foreach (SkinMemento skin in Skins)
			{
				dictionary[skin.Id] = skin;
			}
			return dictionary;
		}

		public bool Equals(SkinsMemento other)
		{
			if (!Cape.Equals(other.Cape))
			{
				return false;
			}
			if (!Skins.SequenceEqual(other.Skins))
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SkinsMemento))
			{
				return false;
			}
			SkinsMemento other = (SkinsMemento)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return Cape.GetHashCode() ^ Skins.GetHashCode() ^ DeletedSkins.GetHashCode();
		}

		public override string ToString()
		{
			string[] value = Skins.Select((SkinMemento s) => string.Format("\"{0}\"", s.Name)).ToArray();
			string text = string.Join(",", value);
			string text2 = string.Join(",", DeletedSkins.ToArray());
			return string.Format(CultureInfo.InvariantCulture, "{{ \"skins\":[{0}], \"cape\":\"{1}\", \"deletedSkins\":[{2}] }}", text, cape, text2);
		}

		internal static SkinsMemento Merge(SkinsMemento left, SkinsMemento right)
		{
			HashSet<string> hashSet = new HashSet<string>(left.DeletedSkins.Concat(right.DeletedSkins));
			Dictionary<string, SkinMemento> dictionary = new Dictionary<string, SkinMemento>();
			foreach (SkinMemento skin in left.Skins)
			{
				if (!hashSet.Contains(skin.Id))
				{
					dictionary[skin.Id] = skin;
				}
			}
			foreach (SkinMemento skin2 in right.Skins)
			{
				if (!hashSet.Contains(skin2.Id))
				{
					dictionary[skin2.Id] = skin2;
				}
			}
			bool conflicted = left.Conflicted || right.Conflicted;
			CapeMemento capeMemento = CapeMemento.ChooseCape(left.Cape, right.Cape);
			return new SkinsMemento(dictionary.Values.ToList(), hashSet, capeMemento, conflicted);
		}
	}
}
