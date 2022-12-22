using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	internal sealed class PlayerStateMemento
	{
		private readonly string _id;

		public string Id
		{
			get
			{
				return _id;
			}
		}

		public int MinDay { get; private set; }

		public int MaxDay { get; private set; }

		public int? MinInGameMinutes { get; private set; }

		public int? MaxInGameMinutes { get; private set; }

		public bool IsPaying { get; private set; }

		public PlayerStateMemento(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			_id = id;
		}

		internal static PlayerStateMemento FromDictionary(string id, Dictionary<string, object> dictionary)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			PlayerStateMemento playerStateMemento = new PlayerStateMemento(id);
			object value;
			if (dictionary.TryGetValue("minDay", out value))
			{
				try
				{
					playerStateMemento.MinDay = Convert.ToInt32(value, NumberFormatInfo.InvariantInfo);
				}
				catch (Exception ex)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as int. {2}", "minDay", value, ex.Message);
				}
			}
			object value2;
			if (dictionary.TryGetValue("maxDay", out value2))
			{
				try
				{
					playerStateMemento.MaxDay = Convert.ToInt32(value2, NumberFormatInfo.InvariantInfo);
				}
				catch (Exception ex2)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as int. {2}", "maxDay", value2, ex2.Message);
				}
			}
			object value3;
			if (dictionary.TryGetValue("minInGameMinutes", out value3))
			{
				try
				{
					playerStateMemento.MinInGameMinutes = Convert.ToInt32(value3, NumberFormatInfo.InvariantInfo);
				}
				catch (Exception ex3)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as int. {2}", "minInGameMinutes", value3, ex3.Message);
				}
			}
			object value4;
			if (dictionary.TryGetValue("maxInGameMinutes", out value4))
			{
				try
				{
					playerStateMemento.MaxInGameMinutes = Convert.ToInt32(value4, NumberFormatInfo.InvariantInfo);
				}
				catch (Exception ex4)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as int. {2}", "maxInGameMinutes", value4, ex4.Message);
				}
			}
			object value5;
			if (dictionary.TryGetValue("paying", out value5))
			{
				try
				{
					playerStateMemento.IsPaying = Convert.ToBoolean(value5);
					return playerStateMemento;
				}
				catch (Exception ex5)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as boolean. {2}", "paying", value5, ex5.Message);
					return playerStateMemento;
				}
			}
			return playerStateMemento;
		}
	}
}
