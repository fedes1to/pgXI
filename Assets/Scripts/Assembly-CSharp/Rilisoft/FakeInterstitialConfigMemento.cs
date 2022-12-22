using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class FakeInterstitialConfigMemento
	{
		private const string MaxShowCountDuringSessionKey = "maxShowCountDuringSession";

		private const string ShowFrequencyKey = "showFrequency";

		private readonly List<string> _imageUrls = new List<string>();

		private readonly List<string> _redirectUrls = new List<string>();

		private readonly Dictionary<string, Dictionary<string, object>> _overrides = new Dictionary<string, Dictionary<string, object>>();

		public List<string> ImageUrls
		{
			get
			{
				return _imageUrls;
			}
		}

		public List<string> RedirectUrls
		{
			get
			{
				return _redirectUrls;
			}
		}

		private bool Enabled { get; set; }

		private int MinLevel { get; set; }

		private int MaxLevel { get; set; }

		private int ShowFrequency { get; set; }

		private int MaxShowCountDuringSession { get; set; }

		private Dictionary<string, Dictionary<string, object>> Overrides
		{
			get
			{
				return _overrides;
			}
		}

		internal static FakeInterstitialConfigMemento FromDictionary(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			FakeInterstitialConfigMemento fakeInterstitialConfigMemento = new FakeInterstitialConfigMemento();
			bool? boolean = ParsingHelper.GetBoolean(dictionary, "enabled");
			fakeInterstitialConfigMemento.Enabled = boolean.HasValue && boolean.Value;
			List<object> list = ParsingHelper.GetObject(dictionary, "imageUrls") as List<object>;
			if (list != null)
			{
				foreach (object item in list)
				{
					string text = item as string;
					if (!string.IsNullOrEmpty(text))
					{
						fakeInterstitialConfigMemento.ImageUrls.Add(text);
					}
				}
			}
			List<object> list2 = ParsingHelper.GetObject(dictionary, "redirectUrls") as List<object>;
			if (list2 != null)
			{
				foreach (object item2 in list2)
				{
					string text2 = item2 as string;
					if (!string.IsNullOrEmpty(text2))
					{
						fakeInterstitialConfigMemento.RedirectUrls.Add(text2);
					}
				}
			}
			int? @int = ParsingHelper.GetInt32(dictionary, "minLevel");
			fakeInterstitialConfigMemento.MinLevel = ((!@int.HasValue) ? 1 : @int.Value);
			int? int2 = ParsingHelper.GetInt32(dictionary, "maxLevel");
			fakeInterstitialConfigMemento.MaxLevel = ((!int2.HasValue) ? int.MaxValue : int2.Value);
			int? int3 = ParsingHelper.GetInt32(dictionary, "showFrequency");
			fakeInterstitialConfigMemento.ShowFrequency = (int3.HasValue ? int3.Value : 0);
			int? int4 = ParsingHelper.GetInt32(dictionary, "maxShowCountDuringSession");
			fakeInterstitialConfigMemento.MaxShowCountDuringSession = (int4.HasValue ? int4.Value : 0);
			Dictionary<string, object> dictionary2 = ParsingHelper.GetObject(dictionary, "overrides") as Dictionary<string, object>;
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<string, object> item3 in dictionary2)
				{
					Dictionary<string, object> dictionary3 = item3.Value as Dictionary<string, object>;
					if (dictionary3 != null)
					{
						fakeInterstitialConfigMemento.Overrides[item3.Key] = dictionary3;
					}
				}
				return fakeInterstitialConfigMemento;
			}
			return fakeInterstitialConfigMemento;
		}

		internal string GetDisabledReason(string category, int level, int fakeInterstitialCount, int totalInterstitialCount, bool realInterstitialsEnabled)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
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
			if (ImageUrls.Count == 0)
			{
				return "Image URLs list is empty.";
			}
			if (RedirectUrls.Count == 0)
			{
				return "Redirect URLs list is empty";
			}
			int? @int = ParsingHelper.GetInt32(value, "minLevel");
			if (@int.HasValue)
			{
				if (level < @int.Value)
				{
					return string.Format("Level {0} < {1} for category `{2}`.", level, @int.Value, category);
				}
			}
			else if (level < MinLevel)
			{
				return string.Format("Level {0} < {1}.", level, @int.Value);
			}
			int? int2 = ParsingHelper.GetInt32(value, "maxLevel");
			if (int2.HasValue)
			{
				if (level > int2.Value)
				{
					return string.Format("Level {0} > {1} for category `{2}`.", level, int2.Value, category);
				}
			}
			else if (level > MaxLevel)
			{
				return string.Format("Level {0} > {1}.", level, int2.Value);
			}
			int? int3 = ParsingHelper.GetInt32(value, "showFrequency");
			if (realInterstitialsEnabled)
			{
				if (int3.HasValue)
				{
					if (int3.Value == 0)
					{
						return "showFrequencyOverride.Value == 0";
					}
					if (totalInterstitialCount % int3.Value != 0)
					{
						return string.Format("{0}: {1} % {2} != 0 for category `{3}`.", "showFrequency", totalInterstitialCount, int3.Value, category);
					}
				}
				else
				{
					if (ShowFrequency == 0)
					{
						return "ShowFrequency == 0";
					}
					if (totalInterstitialCount % ShowFrequency != 0)
					{
						return string.Format("{0}: {1} % {2} != 0 ", "showFrequency", totalInterstitialCount, int3.Value);
					}
				}
			}
			int? int4 = ParsingHelper.GetInt32(value, "maxShowCountDuringSession");
			if (int4.HasValue)
			{
				if (fakeInterstitialCount >= int4.Value)
				{
					return string.Format("{0}: {1} >= {2} for category `{3}`", fakeInterstitialCount, int4.Value, "maxShowCountDuringSession", category);
				}
			}
			else if (fakeInterstitialCount >= MaxShowCountDuringSession)
			{
				return string.Format("{0}: {1} >= {2}", fakeInterstitialCount, int4.Value, "maxShowCountDuringSession");
			}
			return string.Empty;
		}
	}
}
