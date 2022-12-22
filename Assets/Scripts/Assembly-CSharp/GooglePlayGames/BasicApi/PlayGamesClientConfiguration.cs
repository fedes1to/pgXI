using System.Collections.Generic;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi
{
	public struct PlayGamesClientConfiguration
	{
		public class Builder
		{
			private bool mEnableSaveGames;

			private bool mRequireGooglePlus;

			private List<string> mScopes;

			private InvitationReceivedDelegate mInvitationDelegate = delegate
			{
			};

			private MatchDelegate mMatchDelegate = delegate
			{
			};

			private string mRationale;

			public Builder EnableSavedGames()
			{
				mEnableSaveGames = true;
				return this;
			}

			public Builder RequireGooglePlus()
			{
				mRequireGooglePlus = true;
				return this;
			}

			public Builder AddOauthScope(string scope)
			{
				if (mScopes == null)
				{
					mScopes = new List<string>();
				}
				mScopes.Add(scope);
				return this;
			}

			public Builder WithInvitationDelegate(InvitationReceivedDelegate invitationDelegate)
			{
				mInvitationDelegate = Misc.CheckNotNull(invitationDelegate);
				return this;
			}

			public Builder WithMatchDelegate(MatchDelegate matchDelegate)
			{
				mMatchDelegate = Misc.CheckNotNull(matchDelegate);
				return this;
			}

			public Builder WithPermissionRationale(string rationale)
			{
				mRationale = rationale;
				return this;
			}

			public PlayGamesClientConfiguration Build()
			{
				mRequireGooglePlus = GameInfo.RequireGooglePlus();
				return new PlayGamesClientConfiguration(this);
			}

			internal bool HasEnableSaveGames()
			{
				return mEnableSaveGames;
			}

			internal bool HasRequireGooglePlus()
			{
				return mRequireGooglePlus;
			}

			internal string[] getScopes()
			{
				return (mScopes != null) ? mScopes.ToArray() : new string[0];
			}

			internal MatchDelegate GetMatchDelegate()
			{
				return mMatchDelegate;
			}

			internal InvitationReceivedDelegate GetInvitationDelegate()
			{
				return mInvitationDelegate;
			}

			internal string GetPermissionRationale()
			{
				return mRationale;
			}
		}

		public static readonly PlayGamesClientConfiguration DefaultConfiguration = new Builder().WithPermissionRationale("Select email address to send to this game or hit cancel to not share.").Build();

		private readonly bool mEnableSavedGames;

		private readonly bool mRequireGooglePlus;

		private readonly string[] mScopes;

		private readonly InvitationReceivedDelegate mInvitationDelegate;

		private readonly MatchDelegate mMatchDelegate;

		private readonly string mPermissionRationale;

		public bool EnableSavedGames
		{
			get
			{
				return mEnableSavedGames;
			}
		}

		public bool RequireGooglePlus
		{
			get
			{
				return mRequireGooglePlus;
			}
		}

		public string[] Scopes
		{
			get
			{
				return mScopes;
			}
		}

		public InvitationReceivedDelegate InvitationDelegate
		{
			get
			{
				return mInvitationDelegate;
			}
		}

		public MatchDelegate MatchDelegate
		{
			get
			{
				return mMatchDelegate;
			}
		}

		public string PermissionRationale
		{
			get
			{
				return mPermissionRationale;
			}
		}

		private PlayGamesClientConfiguration(Builder builder)
		{
			mEnableSavedGames = builder.HasEnableSaveGames();
			mInvitationDelegate = builder.GetInvitationDelegate();
			mMatchDelegate = builder.GetMatchDelegate();
			mPermissionRationale = builder.GetPermissionRationale();
			mRequireGooglePlus = builder.HasRequireGooglePlus();
			mScopes = builder.getScopes();
		}
	}
}
