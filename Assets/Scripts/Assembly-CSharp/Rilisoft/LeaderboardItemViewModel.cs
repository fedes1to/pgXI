using System;
using UnityEngine;

namespace Rilisoft
{
	public sealed class LeaderboardItemViewModel
	{
		private static LeaderboardItemViewModel _empty = new LeaderboardItemViewModel
		{
			Id = string.Empty,
			Nickname = string.Empty,
			ClanLogo = string.Empty
		};

		private string _clanLogo = string.Empty;

		private Lazy<Texture2D> _clanLogoTexture = new Lazy<Texture2D>(() => null);

		public string Id { get; set; }

		public int Rank { get; set; }

		public string Nickname { get; set; }

		public int WinCount { get; set; }

		public int Place { get; set; }

		public bool Highlight { get; set; }

		public string ClanName { get; set; }

		public string ClanLogo
		{
			get
			{
				return _clanLogo;
			}
			set
			{
				if (!(value == _clanLogo))
				{
					_clanLogo = value;
					_clanLogoTexture = new Lazy<Texture2D>(() => CreateLogoFromBase64String(value));
				}
			}
		}

		public Texture2D ClanLogoTexture
		{
			get
			{
				if (_clanLogoTexture.Value == null)
				{
					string currentClanLogo = ClanLogo;
					_clanLogoTexture = new Lazy<Texture2D>(() => CreateLogoFromBase64String(currentClanLogo));
				}
				return _clanLogoTexture.Value;
			}
		}

		public static LeaderboardItemViewModel Empty
		{
			get
			{
				return _empty;
			}
		}

		internal static Texture2D CreateLogoFromBase64String(string logo)
		{
			//Discarded unreachable code: IL_0045, IL_0059
			if (string.IsNullOrEmpty(logo))
			{
				return null;
			}
			try
			{
				byte[] data = Convert.FromBase64String(logo);
				Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
				texture2D.filterMode = FilterMode.Point;
				Texture2D texture2D2 = texture2D;
				texture2D2.LoadImage(data);
				texture2D2.Apply();
				return texture2D2;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return null;
			}
		}
	}
}
