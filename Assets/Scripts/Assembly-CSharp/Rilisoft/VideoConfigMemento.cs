using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rilisoft
{
	[Serializable]
	internal sealed class VideoConfigMemento
	{
		private readonly HashSet<string> _disabledDevices = new HashSet<string>();

		private readonly Dictionary<string, Dictionary<string, object>> _overrides = new Dictionary<string, Dictionary<string, object>>();

		public double TimeoutWaitInSeconds { get; private set; }

		private bool Enabled { get; set; }

		private HashSet<string> DisabledDevices
		{
			get
			{
				return _disabledDevices;
			}
		}

		private Dictionary<string, Dictionary<string, object>> Overrides
		{
			get
			{
				return _overrides;
			}
		}

		public string GetDisabledReason(string category, string device)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			Dictionary<string, object> value;
			if (!Overrides.TryGetValue(category, out value))
			{
				value = new Dictionary<string, object>();
			}
			bool? boolean = ParsingHelper.GetBoolean(value, "enabled");
			if (boolean.HasValue)
			{
				if (!boolean.Value)
				{
					return string.Format("Explicitely disabled for category `{0}`.", category);
				}
			}
			else if (!Enabled)
			{
				return "Just disabled.";
			}
			List<object> list = ParsingHelper.GetObject(value, "disableDevices") as List<object>;
			if (list != null && list.OfType<string>().Any((string s) => device == s))
			{
				return string.Format(CultureInfo.InvariantCulture, "Explicitely disabled for category `{0}` and device `{1}`.", category, device);
			}
			if (DisabledDevices.Contains(device))
			{
				return string.Format(CultureInfo.InvariantCulture, "Disabled for device `{0}`.", device);
			}
			return string.Empty;
		}

		internal static VideoConfigMemento FromDictionary(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			VideoConfigMemento videoConfigMemento = new VideoConfigMemento();
			bool? boolean = ParsingHelper.GetBoolean(dictionary, "enabled");
			if (boolean.HasValue)
			{
				videoConfigMemento.Enabled = boolean.Value;
			}
			List<object> list = ParsingHelper.GetObject(dictionary, "disableDevices") as List<object>;
			if (list != null)
			{
				foreach (object item in list)
				{
					string text = item as string;
					if (!string.IsNullOrEmpty(text))
					{
						videoConfigMemento.DisabledDevices.Add(text);
					}
				}
			}
			double? @double = ParsingHelper.GetDouble(dictionary, "timeoutWaitSeconds");
			videoConfigMemento.TimeoutWaitInSeconds = ((!@double.HasValue) ? 7.0 : @double.Value);
			Dictionary<string, object> dictionary2 = ParsingHelper.GetObject(dictionary, "overrides") as Dictionary<string, object>;
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<string, object> item2 in dictionary2)
				{
					Dictionary<string, object> dictionary3 = item2.Value as Dictionary<string, object>;
					if (dictionary3 != null)
					{
						videoConfigMemento.Overrides[item2.Key] = dictionary3;
					}
				}
				return videoConfigMemento;
			}
			return videoConfigMemento;
		}
	}
}
