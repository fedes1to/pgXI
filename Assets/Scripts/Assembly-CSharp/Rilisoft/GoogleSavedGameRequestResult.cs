using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi.SavedGame;

namespace Rilisoft
{
	public struct GoogleSavedGameRequestResult<TValue> : IEquatable<GoogleSavedGameRequestResult<TValue>>
	{
		private readonly SavedGameRequestStatus _requestStatus;

		private readonly TValue _value;

		public SavedGameRequestStatus RequestStatus
		{
			get
			{
				return _requestStatus;
			}
		}

		public TValue Value
		{
			get
			{
				return _value;
			}
		}

		public GoogleSavedGameRequestResult(SavedGameRequestStatus requestStatus, TValue value)
		{
			_requestStatus = requestStatus;
			_value = value;
		}

		public bool Equals(GoogleSavedGameRequestResult<TValue> other)
		{
			if (!_requestStatus.Equals(other.RequestStatus))
			{
				return false;
			}
			if (!EqualityComparer<TValue>.Default.Equals(other.Value))
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GoogleSavedGameRequestResult<TValue>))
			{
				return false;
			}
			GoogleSavedGameRequestResult<TValue> other = (GoogleSavedGameRequestResult<TValue>)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return RequestStatus.GetHashCode() ^ Value.GetHashCode();
		}
	}
}
