using System;
using System.Collections.Generic;
using System.Globalization;
using Rilisoft.MiniJson;

namespace Rilisoft
{
	[Serializable]
	internal sealed class AdsConfigMemento
	{
		private Exception _exception;

		private readonly Dictionary<string, PlayerStateMemento> _playerStates = new Dictionary<string, PlayerStateMemento>();

		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		public CheaterConfigMemento CheaterConfig { get; private set; }

		public Dictionary<string, PlayerStateMemento> PlayerStates
		{
			get
			{
				return _playerStates;
			}
		}

		public InterstitialConfigMemento InterstitialConfig { get; private set; }

		public FakeInterstitialConfigMemento FakeInterstitialConfig { get; private set; }

		public VideoConfigMemento VideoConfig { get; private set; }

		public AdPointsConfigMemento AdPointsConfig { get; private set; }

		private AdsConfigMemento(Exception exception)
		{
			_exception = exception;
		}

		internal static AdsConfigMemento FromJson(string json)
		{
			//Discarded unreachable code: IL_005a, IL_00db, IL_00f0
			if (json == null)
			{
				return new AdsConfigMemento(new ArgumentNullException("json"));
			}
			if (json.Trim() == string.Empty)
			{
				return new AdsConfigMemento(new ArgumentException("Json is empty.", "json"));
			}
			object obj;
			try
			{
				obj = Json.Deserialize(json);
			}
			catch (Exception exception)
			{
				return new AdsConfigMemento(exception);
			}
			if (obj == null)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Failed to deserialize json: `{0}`", json);
				return new AdsConfigMemento(new ArgumentException(message, "json"));
			}
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				string message2 = string.Format(CultureInfo.InvariantCulture, "Failed to interpret json as dictionary: `{0}`", json);
				return new AdsConfigMemento(new ArgumentException(message2, "json"));
			}
			try
			{
				return FromDictionary(dictionary);
			}
			catch (Exception exception2)
			{
				return new AdsConfigMemento(exception2);
			}
		}

		internal static AdsConfigMemento FromDictionary(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			AdsConfigMemento adsConfigMemento = new AdsConfigMemento(null);
			object value;
			if (!dictionary.TryGetValue("cheater", out value))
			{
				adsConfigMemento.CheaterConfig = new CheaterConfigMemento();
			}
			else
			{
				Dictionary<string, object> dictionary2 = value as Dictionary<string, object>;
				if (dictionary2 == null)
				{
					adsConfigMemento.CheaterConfig = new CheaterConfigMemento();
				}
				else
				{
					adsConfigMemento.CheaterConfig = CheaterConfigMemento.FromDictionary(dictionary2);
				}
			}
			object value2;
			if (dictionary.TryGetValue("playerStates", out value2))
			{
				Dictionary<string, object> dictionary3 = value2 as Dictionary<string, object>;
				if (dictionary3 != null)
				{
					foreach (KeyValuePair<string, object> item in dictionary3)
					{
						Dictionary<string, object> dictionary4 = item.Value as Dictionary<string, object>;
						if (dictionary4 != null)
						{
							adsConfigMemento.PlayerStates[item.Key] = PlayerStateMemento.FromDictionary(item.Key, dictionary4);
						}
					}
				}
			}
			object value3;
			if (!dictionary.TryGetValue("interstitials", out value3))
			{
				adsConfigMemento.InterstitialConfig = new InterstitialConfigMemento();
			}
			else
			{
				Dictionary<string, object> dictionary5 = value3 as Dictionary<string, object>;
				if (dictionary5 == null)
				{
					adsConfigMemento.InterstitialConfig = new InterstitialConfigMemento();
				}
				else
				{
					adsConfigMemento.InterstitialConfig = InterstitialConfigMemento.FromDictionary(dictionary5);
				}
			}
			object value4;
			if (!dictionary.TryGetValue("fakeInterstitials", out value4))
			{
				adsConfigMemento.FakeInterstitialConfig = new FakeInterstitialConfigMemento();
			}
			else
			{
				Dictionary<string, object> dictionary6 = value4 as Dictionary<string, object>;
				if (dictionary6 == null)
				{
					adsConfigMemento.FakeInterstitialConfig = new FakeInterstitialConfigMemento();
				}
				else
				{
					adsConfigMemento.FakeInterstitialConfig = FakeInterstitialConfigMemento.FromDictionary(dictionary6);
				}
			}
			object value5;
			if (!dictionary.TryGetValue("video", out value5))
			{
				adsConfigMemento.VideoConfig = new VideoConfigMemento();
			}
			else
			{
				Dictionary<string, object> dictionary7 = value5 as Dictionary<string, object>;
				if (dictionary7 == null)
				{
					adsConfigMemento.VideoConfig = new VideoConfigMemento();
				}
				else
				{
					adsConfigMemento.VideoConfig = VideoConfigMemento.FromDictionary(dictionary7);
				}
			}
			object value6;
			if (!dictionary.TryGetValue("points", out value6))
			{
				adsConfigMemento.AdPointsConfig = new AdPointsConfigMemento();
			}
			else
			{
				Dictionary<string, object> dictionary8 = value6 as Dictionary<string, object>;
				if (dictionary8 == null)
				{
					adsConfigMemento.AdPointsConfig = new AdPointsConfigMemento();
				}
				else
				{
					adsConfigMemento.AdPointsConfig = AdPointsConfigMemento.FromDictionary(dictionary8);
				}
			}
			return adsConfigMemento;
		}

		private void TrySetException(Exception value)
		{
			if (_exception == null)
			{
				_exception = value;
			}
		}

		private static Exception CreateParsingException(string key)
		{
			string message = string.Format(CultureInfo.InvariantCulture, "Failed to interpret node as dictionary: `{0}`", key);
			return new InvalidOperationException(message);
		}
	}
}
