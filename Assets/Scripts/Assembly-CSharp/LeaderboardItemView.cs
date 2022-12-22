using System;
using System.Globalization;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class LeaderboardItemView : MonoBehaviour
{
	private const string HighlightColor = "FFFF00";

	public string _id;

	public UISprite rankSprite;

	public UILabel nicknameLabel;

	public UILabel winCountLabel;

	public UILabel placeLabel;

	public UITexture clanLogo;

	public UILabel clanNameLabel;

	public UISprite highlightSprite;

	public UILabel levelLabel;

	public UISprite background;

	public event EventHandler<ClickedEventArgs> Clicked;

	public void NewReset(LeaderboardItemViewModel viewModel)
	{
		LeaderboardItemViewModel leaderboardItemViewModel = viewModel ?? LeaderboardItemViewModel.Empty;
		this.Clicked = null;
		_id = leaderboardItemViewModel.Id;
		UILabel.Effect effectStyle = UILabel.Effect.Outline;
		highlightSprite.Do(delegate(UISprite h)
		{
			h.gameObject.SetActive(viewModel.Highlight);
		});
		placeLabel.Do(delegate(UILabel l)
		{
			l.text = viewModel.Place.ToString(CultureInfo.InvariantCulture);
			l.effectStyle = effectStyle;
		});
		nicknameLabel.Do(delegate(UILabel l)
		{
			l.text = viewModel.Nickname ?? string.Empty;
			l.effectStyle = effectStyle;
		});
		levelLabel.Do(delegate(UILabel l)
		{
			l.text = viewModel.Rank.ToString(CultureInfo.InvariantCulture);
			l.effectStyle = effectStyle;
		});
		clanLogo.Do(delegate(UITexture s)
		{
			LeaderboardScript.SetClanLogo(s, viewModel.ClanLogoTexture);
		});
		clanNameLabel.Do(delegate(UILabel l)
		{
			l.text = ((!string.IsNullOrEmpty(viewModel.ClanName)) ? viewModel.ClanName : LocalizationStore.Get("Key_1500"));
			l.effectStyle = effectStyle;
		});
		winCountLabel.Do(delegate(UILabel l)
		{
			l.text = viewModel.WinCount.ToString(CultureInfo.InvariantCulture);
			l.effectStyle = effectStyle;
		});
		if (background != null)
		{
			if ((float)viewModel.Place % 2f > 0f)
			{
				Color color = new Color(0.8f, 0.9f, 1f);
				GetComponent<UIButton>().defaultColor = color;
				background.color = color;
			}
			else
			{
				Color color2 = new Color(1f, 1f, 1f);
				GetComponent<UIButton>().defaultColor = color2;
				background.color = color2;
			}
		}
	}

	public void HandleClick()
	{
		ClickedEventArgs e = new ClickedEventArgs(_id);
		this.Clicked.Do(delegate(EventHandler<ClickedEventArgs> handler)
		{
			handler(this, e);
		});
	}

	[Obsolete]
	public void Reset(LeaderboardItemViewModel viewModel)
	{
		LeaderboardItemViewModel leaderboardItemViewModel = viewModel ?? LeaderboardItemViewModel.Empty;
		Func<object, string> func = delegate(object s)
		{
			string text = s.ToString();
			if (viewModel.Highlight)
			{
				text = string.Format("[{0}]{1}[-]", "FFFF00", text);
			}
			return text;
		};
		if (rankSprite != null)
		{
			rankSprite.spriteName = "Rank_" + Mathf.Clamp(leaderboardItemViewModel.Rank, 1, 31);
		}
		if (clanLogo != null)
		{
			if (!string.IsNullOrEmpty(leaderboardItemViewModel.ClanLogo))
			{
				try
				{
					byte[] data = Convert.FromBase64String(leaderboardItemViewModel.ClanLogo ?? string.Empty);
					Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
					texture2D.LoadImage(data);
					texture2D.filterMode = FilterMode.Point;
					texture2D.Apply();
					Texture mainTexture = clanLogo.mainTexture;
					clanLogo.mainTexture = texture2D;
					if (mainTexture != null)
					{
						UnityEngine.Object.Destroy(mainTexture);
					}
				}
				catch
				{
					Texture mainTexture2 = clanLogo.mainTexture;
					clanLogo.mainTexture = null;
					if (mainTexture2 != null)
					{
						UnityEngine.Object.Destroy(mainTexture2);
					}
				}
			}
			else
			{
				Texture mainTexture3 = clanLogo.mainTexture;
				clanLogo.mainTexture = null;
				if (mainTexture3 != null)
				{
					UnityEngine.Object.Destroy(mainTexture3);
				}
			}
		}
		if (nicknameLabel != null)
		{
			string arg = leaderboardItemViewModel.Nickname ?? string.Empty;
			nicknameLabel.text = func(arg);
		}
		if (winCountLabel != null)
		{
			winCountLabel.text = ((leaderboardItemViewModel != LeaderboardItemViewModel.Empty) ? func((leaderboardItemViewModel.WinCount != int.MinValue) ? Math.Max(leaderboardItemViewModel.WinCount, 0).ToString() : "—") : string.Empty);
		}
		if (placeLabel != null)
		{
			placeLabel.text = ((leaderboardItemViewModel != LeaderboardItemViewModel.Empty) ? func((leaderboardItemViewModel.Place >= 0) ? leaderboardItemViewModel.Place.ToString() : LocalizationStore.Key_0588) : string.Empty);
		}
	}

	[Obsolete]
	public void Reset()
	{
		Reset(null);
	}
}
