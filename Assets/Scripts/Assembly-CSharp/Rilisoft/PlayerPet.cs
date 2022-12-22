using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class PlayerPet
	{
		[SerializeField]
		public int Points;

		public string PetName;

		[SerializeField]
		private string _infoId;

		private PetInfo _info;

		[SerializeField]
		private long m_timestamp;

		public long NameTimestamp
		{
			get
			{
				return m_timestamp;
			}
			set
			{
				m_timestamp = value;
			}
		}

		public string InfoId
		{
			get
			{
				return _infoId;
			}
			set
			{
				_infoId = value;
				_info = null;
			}
		}

		public PetInfo Info
		{
			get
			{
				if (_info == null)
				{
					_info = ((!PetsManager.Infos.ContainsKey(InfoId)) ? null : PetsManager.Infos[_infoId]);
				}
				return _info;
			}
		}

		internal static PlayerPet Merge(PlayerPet left, PlayerPet right)
		{
			return null;
		}
	}
}
