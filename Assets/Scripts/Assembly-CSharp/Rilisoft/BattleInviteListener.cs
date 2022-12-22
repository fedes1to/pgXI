using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class BattleInviteListener
	{
		private static readonly BattleInviteListener s_instance = new BattleInviteListener();

		private readonly HashSet<string> _incomingInviteFriendIds = new HashSet<string>();

		private readonly Dictionary<string, float> _outgoingInviteTimestamps = new Dictionary<string, float>();

		private TimeSpan? _outgoingInviteTimeout;

		public static BattleInviteListener Instance
		{
			get
			{
				return s_instance;
			}
		}

		internal TimeSpan OutgoingInviteTimeout
		{
			get
			{
				TimeSpan? outgoingInviteTimeout = _outgoingInviteTimeout;
				return (!outgoingInviteTimeout.HasValue) ? DefaultOutgoingInviteTimeout : outgoingInviteTimeout.Value;
			}
			set
			{
				_outgoingInviteTimeout = value;
			}
		}

		private static TimeSpan DefaultOutgoingInviteTimeout
		{
			get
			{
				return (!Application.isEditor) ? TimeSpan.FromMinutes(15.0) : TimeSpan.FromSeconds(15.0);
			}
		}

		internal bool CallToFriendEnabled(string friendId)
		{
			if (string.IsNullOrEmpty(friendId))
			{
				return false;
			}
			float? outgoingInviteTimestamp = GetOutgoingInviteTimestamp(friendId);
			if (!outgoingInviteTimestamp.HasValue)
			{
				return true;
			}
			if ((double)(Time.realtimeSinceStartup - outgoingInviteTimestamp.Value) < OutgoingInviteTimeout.TotalSeconds)
			{
				return false;
			}
			return true;
		}

		internal float? GetOutgoingInviteTimestamp(string friendId)
		{
			if (friendId == null)
			{
				return null;
			}
			float value;
			if (_outgoingInviteTimestamps.TryGetValue(friendId, out value))
			{
				return value;
			}
			return null;
		}

		internal void SetOutgoingInviteTimestamp(string friendId, float timestamp)
		{
			if (friendId != null)
			{
				_outgoingInviteTimestamps[friendId] = timestamp;
			}
		}

		internal IEnumerable<string> GetFriendIds()
		{
			return _incomingInviteFriendIds;
		}

		internal void ClearIncomingInvites()
		{
			_incomingInviteFriendIds.Clear();
		}

		internal void NotifyBattleIncomingInvite(string friendId, string nickname)
		{
			if (!string.IsNullOrEmpty(friendId) && !string.IsNullOrEmpty(nickname))
			{
				_incomingInviteFriendIds.Add(friendId);
				InfoWindowController.Instance.ShowBattleInvite(nickname);
			}
		}
	}
}
