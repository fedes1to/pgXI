namespace Rilisoft
{
	public class AchievementProgress
	{
		private SaltedInt _points = new SaltedInt(19077818);

		private SaltedInt _stage = new SaltedInt(250606678);

		private AchievementProgressData _data;

		public int Points
		{
			get
			{
				return _points.Value;
			}
		}

		public int Stage
		{
			get
			{
				return _stage.Value;
			}
		}

		public AchievementProgressData Data
		{
			get
			{
				_data.Points = _points.Value;
				_data.Stage = _stage.Value;
				return _data;
			}
			set
			{
				_data = value;
				_points.Value = _data.Points;
				_stage.Value = _data.Stage;
			}
		}

		public AchievementProgress(AchievementProgressData data)
		{
			if (data == null)
			{
				Achievement.LogMsg("AchievementProgressData is null");
			}
			else
			{
				Data = data;
			}
		}

		public void IncrementPoints(int inc = 1)
		{
			_points.Value += inc;
		}

		public void IncrementStage(int inc = 1)
		{
			_stage.Value += inc;
		}

		public bool CustomDataExists(string key)
		{
			if (key.IsNullOrEmpty())
			{
				return false;
			}
			return Data.CustomData.ContainsKey(key);
		}

		public object CustomDataGet(string key)
		{
			if (key.IsNullOrEmpty())
			{
				return null;
			}
			return (!_data.CustomData.ContainsKey(key)) ? null : Data.CustomData[key];
		}

		public void CustomDataSet(string key, object val = null)
		{
			if (key.IsNullOrEmpty())
			{
				return;
			}
			if (val == null)
			{
				val = string.Empty;
			}
			if (Data.CustomData.ContainsKey(key))
			{
				if (Data.CustomData[key] != val)
				{
					Data.CustomData[key] = val;
				}
			}
			else
			{
				Data.CustomData.Add(key, val);
			}
		}

		public void CustomDataRemove(string key)
		{
			if (!key.IsNullOrEmpty())
			{
				key = key.ToLower();
				if (Data.CustomData.ContainsKey(key))
				{
					Data.CustomData.Remove(key);
				}
			}
		}
	}
}
