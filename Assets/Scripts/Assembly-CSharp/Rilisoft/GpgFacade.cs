using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class GpgFacade
	{
		private readonly PlayGamesPlatform _playGamesPlatformInstance;

		private static readonly Lazy<GpgFacade> s_instance = new Lazy<GpgFacade>(() => new GpgFacade());

		public static GpgFacade Instance
		{
			get
			{
				return s_instance.Value;
			}
		}

		public PlayGamesPlatform PlayGamesPlatform
		{
			get
			{
				return _playGamesPlatformInstance;
			}
		}

		public ISavedGameClient SavedGame
		{
			get
			{
				//Discarded unreachable code: IL_0011, IL_001e
				try
				{
					return PlayGamesPlatform.SavedGame;
				}
				catch (NullReferenceException)
				{
					return null;
				}
			}
		}

		public event EventHandler SignedOut;

		private GpgFacade()
		{
			try
			{
				PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
				PlayGamesPlatform.InitializeInstance(configuration);
				PlayGamesPlatform.DebugLogEnabled = Defs.IsDeveloperBuild && BuildSettings.BuildTargetPlatform == RuntimePlatform.Android;
				PlayGamesPlatform.Activate();
				_playGamesPlatformInstance = PlayGamesPlatform.Instance;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public void Initialize()
		{
		}

		public void Authenticate(Action<bool> callback, bool silent)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			PlayGamesPlatform.Authenticate(callback, silent);
		}

		public void IncrementAchievement(string achievementId, int steps, Action<bool> callback)
		{
			if (achievementId == null)
			{
				throw new ArgumentNullException("achievementId");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			PlayGamesPlatform.IncrementAchievement(achievementId, steps, callback);
		}

		public bool IsAuthenticated()
		{
			return PlayGamesPlatform.IsAuthenticated();
		}

		public void SignOut()
		{
			PlayGamesPlatform.SignOut();
			EventHandler signedOut = this.SignedOut;
			if (signedOut != null)
			{
				signedOut(this, EventArgs.Empty);
			}
		}
	}
}
