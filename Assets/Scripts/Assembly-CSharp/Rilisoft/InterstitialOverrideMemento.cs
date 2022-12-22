using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	internal sealed class InterstitialOverrideMemento
	{
		public bool? Enabled { get; private set; }

		public HashSet<string> DisabledDevices { get; private set; }

		public double? TimeoutBetweenShowInMinutes { get; private set; }

		internal static InterstitialOverrideMemento FromDictionary(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			InterstitialOverrideMemento interstitialOverrideMemento = new InterstitialOverrideMemento();
			object value;
			if (dictionary.TryGetValue("enabled", out value))
			{
				try
				{
					interstitialOverrideMemento.Enabled = Convert.ToBoolean(value);
				}
				catch (Exception ex)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as boolean. {2}", "enabled", value, ex.Message);
				}
			}
			object value2;
			if (dictionary.TryGetValue("disableDevices", out value2))
			{
				List<object> list = value2 as List<object>;
				if (list != null)
				{
					foreach (object item in list)
					{
						string text = item as string;
						if (!string.IsNullOrEmpty(text))
						{
							if (interstitialOverrideMemento.DisabledDevices == null)
							{
								interstitialOverrideMemento.DisabledDevices = new HashSet<string>();
							}
							interstitialOverrideMemento.DisabledDevices.Add(text);
						}
					}
				}
			}
			object value3;
			if (dictionary.TryGetValue("timeoutBetweenShowInMinutes", out value3))
			{
				try
				{
					interstitialOverrideMemento.TimeoutBetweenShowInMinutes = Convert.ToDouble(value3);
					return interstitialOverrideMemento;
				}
				catch (Exception ex2)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as double. {2}", "timeoutBetweenShowInMinutes", value3, ex2.Message);
					return interstitialOverrideMemento;
				}
			}
			return interstitialOverrideMemento;
		}
	}
}
