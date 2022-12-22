using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rilisoft
{
	[Serializable]
	internal sealed class InterstitialConfigMemento
	{
		private readonly HashSet<string> _disabledDevices = new HashSet<string>();

		private readonly Dictionary<string, InterstitialOverrideMemento> _overrides = new Dictionary<string, InterstitialOverrideMemento>();

		private bool Enabled { get; set; }

		private HashSet<string> DisabledDevices
		{
			get
			{
				return _disabledDevices;
			}
		}

		private double TimeoutBetweenShowInMinutes { get; set; }

		private Dictionary<string, InterstitialOverrideMemento> Overrides
		{
			get
			{
				return _overrides;
			}
		}

		public bool GetEnabled(string category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			InterstitialOverrideMemento value;
			if (Overrides.TryGetValue(category, out value) && value != null)
			{
				bool? enabled = value.Enabled;
				return (!enabled.HasValue) ? Enabled : enabled.Value;
			}
			return Enabled;
		}

		internal double GetTimeoutBetweenShowInMinutes(string category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			InterstitialOverrideMemento value;
			if (Overrides.TryGetValue(category, out value) && value != null)
			{
				double? timeoutBetweenShowInMinutes = value.TimeoutBetweenShowInMinutes;
				return (!timeoutBetweenShowInMinutes.HasValue) ? TimeoutBetweenShowInMinutes : timeoutBetweenShowInMinutes.Value;
			}
			return TimeoutBetweenShowInMinutes;
		}

		public int GetDisabledReasonCode(string category, string device, double timeSpanSinceLastShowInMinutes)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			InterstitialOverrideMemento value;
			Overrides.TryGetValue(category, out value);
			if (value != null && value.Enabled.HasValue)
			{
				if (!value.Enabled.Value)
				{
					return 1;
				}
			}
			else if (!Enabled)
			{
				return 2;
			}
			if (value != null && value.DisabledDevices != null)
			{
				if (value.DisabledDevices.Contains(device))
				{
					return 3;
				}
			}
			else if (DisabledDevices.Contains(device))
			{
				return 4;
			}
			if (value != null && value.TimeoutBetweenShowInMinutes.HasValue)
			{
				if (timeSpanSinceLastShowInMinutes < value.TimeoutBetweenShowInMinutes.Value)
				{
					return 5;
				}
			}
			else if (timeSpanSinceLastShowInMinutes < TimeoutBetweenShowInMinutes)
			{
				return 6;
			}
			return 0;
		}

		public string GetDisabledReason(string category, string device, double timeSpanSinceLastShowInMinutes)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			InterstitialOverrideMemento value;
			Overrides.TryGetValue(category, out value);
			if (value != null && value.Enabled.HasValue)
			{
				if (!value.Enabled.Value)
				{
					return string.Format("Explicitely disabled for category `{0}`.", category);
				}
			}
			else if (!Enabled)
			{
				return "Just disabled.";
			}
			if (value != null && value.DisabledDevices != null)
			{
				if (value.DisabledDevices.Contains(device))
				{
					return string.Format(CultureInfo.InvariantCulture, "Explicitely disabled for category `{0}` and device `{1}`.", category, device);
				}
			}
			else if (DisabledDevices.Contains(device))
			{
				return string.Format(CultureInfo.InvariantCulture, "Disabled for device `{0}`.", device);
			}
			if (value != null && value.TimeoutBetweenShowInMinutes.HasValue)
			{
				if (timeSpanSinceLastShowInMinutes < value.TimeoutBetweenShowInMinutes.Value)
				{
					return string.Format(CultureInfo.InvariantCulture, "Overridden timeout for category `{0}`: {1:f2} < {2:f2}.", category, timeSpanSinceLastShowInMinutes, value.TimeoutBetweenShowInMinutes.Value);
				}
			}
			else if (timeSpanSinceLastShowInMinutes < TimeoutBetweenShowInMinutes)
			{
				return string.Format(CultureInfo.InvariantCulture, "Timeout: {0:f2} < {1:f2}.", timeSpanSinceLastShowInMinutes, TimeoutBetweenShowInMinutes);
			}
			return string.Empty;
		}

		internal static InterstitialConfigMemento FromDictionary(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			InterstitialConfigMemento interstitialConfigMemento = new InterstitialConfigMemento();
			bool? boolean = ParsingHelper.GetBoolean(dictionary, "enabled");
			if (boolean.HasValue)
			{
				interstitialConfigMemento.Enabled = boolean.Value;
			}
			List<object> list = ParsingHelper.GetObject(dictionary, "disableDevices") as List<object>;
			if (list != null)
			{
				foreach (object item in list)
				{
					string text = item as string;
					if (!string.IsNullOrEmpty(text))
					{
						interstitialConfigMemento.DisabledDevices.Add(text);
					}
				}
			}
			double? @double = ParsingHelper.GetDouble(dictionary, "timeoutBetweenShowInMinutes");
			interstitialConfigMemento.TimeoutBetweenShowInMinutes = ((!@double.HasValue) ? 15.0 : @double.Value);
			Dictionary<string, object> dictionary2 = ParsingHelper.GetObject(dictionary, "overrides") as Dictionary<string, object>;
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<string, object> item2 in dictionary2)
				{
					Dictionary<string, object> dictionary3 = item2.Value as Dictionary<string, object>;
					if (dictionary3 != null)
					{
						InterstitialOverrideMemento value = InterstitialOverrideMemento.FromDictionary(dictionary3);
						interstitialConfigMemento.Overrides[item2.Key] = value;
					}
				}
				return interstitialConfigMemento;
			}
			return interstitialConfigMemento;
		}
	}
}
