using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	internal sealed class CheaterConfigMemento
	{
		public bool CheckSignatureTampering { get; private set; }

		public int CoinThreshold { get; private set; }

		public int GemThreshold { get; private set; }

		public CheaterConfigMemento()
		{
			CoinThreshold = int.MaxValue;
			GemThreshold = int.MaxValue;
		}

		internal static CheaterConfigMemento FromDictionary(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			CheaterConfigMemento cheaterConfigMemento = new CheaterConfigMemento();
			object value;
			if (dictionary.TryGetValue("checkSignatureTampering", out value))
			{
				try
				{
					cheaterConfigMemento.CheckSignatureTampering = Convert.ToBoolean(value);
				}
				catch (Exception ex)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as boolean. {2}", "checkSignatureTampering", value, ex.Message);
				}
			}
			object value2;
			if (dictionary.TryGetValue("coinThreshold", out value2))
			{
				try
				{
					cheaterConfigMemento.CoinThreshold = Convert.ToInt32(value2);
				}
				catch (Exception ex2)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as int. {2}", "coinThreshold", value2, ex2.Message);
				}
			}
			object value3;
			if (dictionary.TryGetValue("gemThreshold", out value3))
			{
				try
				{
					cheaterConfigMemento.GemThreshold = Convert.ToInt32(value3);
					return cheaterConfigMemento;
				}
				catch (Exception ex3)
				{
					Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as int. {2}", "gemThreshold", value3, ex3.Message);
					return cheaterConfigMemento;
				}
			}
			return cheaterConfigMemento;
		}
	}
}
