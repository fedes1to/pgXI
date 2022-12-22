namespace GooglePlayGames.Native.Cwrapper
{
	internal static class Types
	{
		internal enum DataSource
		{
			CACHE_OR_NETWORK = 1,
			NETWORK_ONLY
		}

		internal enum LogLevel
		{
			VERBOSE = 1,
			INFO,
			WARNING,
			ERROR
		}

		internal enum AuthOperation
		{
			SIGN_IN = 1,
			SIGN_OUT
		}

		internal enum ImageResolution
		{
			ICON = 1,
			HI_RES
		}

		internal enum AchievementType
		{
			STANDARD = 1,
			INCREMENTAL
		}

		internal enum AchievementState
		{
			HIDDEN = 1,
			REVEALED,
			UNLOCKED
		}

		internal enum EventVisibility
		{
			HIDDEN = 1,
			REVEALED
		}

		internal enum LeaderboardOrder
		{
			LARGER_IS_BETTER = 1,
			SMALLER_IS_BETTER
		}

		internal enum LeaderboardStart
		{
			TOP_SCORES = 1,
			PLAYER_CENTERED
		}

		internal enum LeaderboardTimeSpan
		{
			DAILY = 1,
			WEEKLY,
			ALL_TIME
		}

		internal enum LeaderboardCollection
		{
			PUBLIC = 1,
			SOCIAL
		}

		internal enum ParticipantStatus
		{
			INVITED = 1,
			JOINED,
			DECLINED,
			LEFT,
			NOT_INVITED_YET,
			FINISHED,
			UNRESPONSIVE
		}

		internal enum MatchResult
		{
			DISAGREED = 1,
			DISCONNECTED,
			LOSS,
			NONE,
			TIE,
			WIN
		}

		internal enum MatchStatus
		{
			INVITED = 1,
			THEIR_TURN,
			MY_TURN,
			PENDING_COMPLETION,
			COMPLETED,
			CANCELED,
			EXPIRED
		}

		internal enum QuestState
		{
			UPCOMING = 1,
			OPEN,
			ACCEPTED,
			COMPLETED,
			EXPIRED,
			FAILED
		}

		internal enum QuestMilestoneState
		{
			NOT_STARTED = 1,
			NOT_COMPLETED,
			COMPLETED_NOT_CLAIMED,
			CLAIMED
		}

		internal enum MultiplayerEvent
		{
			UPDATED = 1,
			UPDATED_FROM_APP_LAUNCH,
			REMOVED
		}

		internal enum MultiplayerInvitationType
		{
			TURN_BASED = 1,
			REAL_TIME
		}

		internal enum RealTimeRoomStatus
		{
			INVITING = 1,
			CONNECTING,
			AUTO_MATCHING,
			ACTIVE,
			DELETED
		}

		internal enum SnapshotConflictPolicy
		{
			MANUAL = 1,
			LONGEST_PLAYTIME,
			LAST_KNOWN_GOOD,
			MOST_RECENTLY_MODIFIED
		}
	}
}
