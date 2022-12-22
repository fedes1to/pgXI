using UnityEngine;

namespace Rilisoft
{
	public class LeprechauntManager : Singleton<LeprechauntManager>
	{
		private readonly StoragerIntCachedProperty _comeTimeSeconds = new StoragerIntCachedProperty("leprechaunt_come_time");

		private readonly StoragerIntCachedProperty _lastDropTimeSeconds = new StoragerIntCachedProperty("leprechaunt_last_drop_time");

		private readonly StoragerIntCachedProperty _lifeTime = new StoragerIntCachedProperty("leprechaunt_lifeTime");

		private readonly StoragerIntCachedProperty _rewardCount = new StoragerIntCachedProperty("leprechaunt_rewardCount");

		private readonly StoragerStringCachedProperty _rewardCurrency = new StoragerStringCachedProperty("leprechaunt_rewardCurrency");

		private readonly StoragerIntCachedProperty _dropDelaySecs = new StoragerIntCachedProperty("leprechaunt_dropDelay");

		private readonly PrefsBoolCachedProperty _needReset = new PrefsBoolCachedProperty("leprechaunt_needReset");

		public int LifeTimeSeconds
		{
			get
			{
				return _lifeTime.Value;
			}
			private set
			{
				_lifeTime.Value = value;
			}
		}

		public int RewardCount
		{
			get
			{
				return _rewardCount.Value;
			}
			private set
			{
				_rewardCount.Value = value;
			}
		}

		public string RewardCurrency
		{
			get
			{
				return _rewardCurrency.Value;
			}
			private set
			{
				_rewardCurrency.Value = value;
			}
		}

		public int DropDelaySeconds
		{
			get
			{
				return _dropDelaySecs.Value;
			}
			private set
			{
				_dropDelaySecs.Value = value;
			}
		}

		public long? CurrentTime
		{
			get
			{
				if (FriendsController.ServerTime < 1)
				{
					return null;
				}
				return FriendsController.ServerTime;
			}
		}

		public bool LeprechauntExists
		{
			get
			{
				return _comeTimeSeconds.Value > 0;
			}
		}

		public int? LeprechauntEndTime
		{
			get
			{
				if (!LeprechauntExists)
				{
					return null;
				}
				return _comeTimeSeconds.Value + LifeTimeSeconds;
			}
		}

		public int? LeprechauntLifeTimeLeft
		{
			get
			{
				if (!CurrentTime.HasValue || !LeprechauntExists)
				{
					return null;
				}
				int? leprechauntEndTime = LeprechauntEndTime;
				int? num = ((!leprechauntEndTime.HasValue) ? null : new int?(leprechauntEndTime.Value - (int)CurrentTime.Value));
				int? result;
				if (num.HasValue && num.Value > 0)
				{
					int? leprechauntEndTime2 = LeprechauntEndTime;
					result = ((!leprechauntEndTime2.HasValue) ? null : new int?(leprechauntEndTime2.Value - (int)CurrentTime.Value));
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		public bool LeprechauntTimeOff
		{
			get
			{
				return _comeTimeSeconds.Value + LifeTimeSeconds < CurrentTime.Value;
			}
		}

		public float? RewardDropSecsLeft
		{
			get
			{
				if (CurrentTime.HasValue)
				{
					return _lastDropTimeSeconds.Value + DropDelaySeconds - CurrentTime.Value;
				}
				return null;
			}
		}

		public bool RewardIsReadyToDrop
		{
			get
			{
				int result;
				if (LeprechauntExists && RewardDropSecsLeft.HasValue)
				{
					float? rewardDropSecsLeft = RewardDropSecsLeft;
					result = ((rewardDropSecsLeft.HasValue && rewardDropSecsLeft.Value <= 0f) ? 1 : 0);
				}
				else
				{
					result = 0;
				}
				return (byte)result != 0;
			}
		}

		public int RewardReadyToDrop
		{
			get
			{
				if (!CurrentTime.HasValue || !LeprechauntEndTime.HasValue)
				{
					return -1;
				}
				return (ElapsedDropIntervals > 0) ? RewardCount : 0;
			}
		}

		private int ElapsedDropIntervals
		{
			get
			{
				if (!CurrentTime.HasValue || !LeprechauntEndTime.HasValue)
				{
					return -1;
				}
				if (LeprechauntEndTime.Value < CurrentTime.Value)
				{
					return 1;
				}
				long num = CurrentTime.Value - _lastDropTimeSeconds.Value;
				return Mathf.CeilToInt(num / DropDelaySeconds);
			}
		}

		private void Update()
		{
			if (_needReset.Value)
			{
				Reset();
			}
		}

		public void SetLeprechaunt(int liveTimeSeconds, string rewardCurrency, int rewardCount, int rewardDropDelaySeconds = 86400)
		{
			Debug.Log(">>> L: set started");
			if (LeprechauntExists)
			{
				Debug.LogError("leprechaun allready exists");
				return;
			}
			Debug.Log(">>> L: exists pass");
			LifeTimeSeconds = liveTimeSeconds;
			RewardCurrency = rewardCurrency;
			RewardCount = rewardCount;
			DropDelaySeconds = rewardDropDelaySeconds;
			Reset();
		}

		private void Reset()
		{
			if (!CurrentTime.HasValue)
			{
				_needReset.Value = true;
				return;
			}
			_needReset.Value = false;
			Debug.Log(">>> L: reset");
			StoragerIntCachedProperty comeTimeSeconds = _comeTimeSeconds;
			long? currentTime = CurrentTime;
			comeTimeSeconds.Value = (int)((!currentTime.HasValue) ? null : new long?(currentTime.Value - DropDelaySeconds)).Value;
			_lastDropTimeSeconds.Value = (int)(CurrentTime.Value - DropDelaySeconds);
			Debug.Log(">>> L: reset to: LifeTimeSeconds: " + LifeTimeSeconds + " RewardCurrency: " + RewardCurrency + " RewardCount " + RewardCount + " DropDelaySeconds " + DropDelaySeconds);
		}

		public void RemoveLeprechaunt()
		{
			if (CurrentTime.HasValue && LeprechauntExists)
			{
				_comeTimeSeconds.Value = -1;
			}
		}

		public void DropReward()
		{
			if (CurrentTime.HasValue && LeprechauntEndTime.HasValue && RewardIsReadyToDrop)
			{
				if (RewardCurrency == "GemsCurrency")
				{
					BankController.AddGems(RewardReadyToDrop);
				}
				else
				{
					BankController.AddCoins(RewardReadyToDrop);
				}
				if (!LeprechauntTimeOff)
				{
					_lastDropTimeSeconds.Value = (int)CurrentTime.Value;
				}
				else
				{
					RemoveLeprechaunt();
				}
			}
		}
	}
}
