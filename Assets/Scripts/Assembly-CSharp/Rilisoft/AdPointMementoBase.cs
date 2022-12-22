using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rilisoft
{
	[Serializable]
	internal abstract class AdPointMementoBase
	{
		private readonly string _id;

		private readonly Dictionary<string, Dictionary<string, object>> _overrides = new Dictionary<string, Dictionary<string, object>>();

		public string Id
		{
			get
			{
				return _id;
			}
		}

		public bool Enabled { get; private set; }

		public Dictionary<string, Dictionary<string, object>> Overrides
		{
			get
			{
				return _overrides;
			}
		}

		public AdPointMementoBase(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			_id = id;
		}

		public int GetDisabledReasonCode(string category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			bool? enabledOverride = GetEnabledOverride(category);
			if (enabledOverride.HasValue)
			{
				if (!enabledOverride.Value)
				{
					return 10;
				}
			}
			else if (!Enabled)
			{
				return 20;
			}
			return 0;
		}

		public string GetDisabledReason(string category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			bool? enabledOverride = GetEnabledOverride(category);
			if (enabledOverride.HasValue)
			{
				if (!enabledOverride.Value)
				{
					return string.Format(CultureInfo.InvariantCulture, "`{0}` explicitely disabled for category `{1}`.", Id, category);
				}
			}
			else if (!Enabled)
			{
				return string.Format(CultureInfo.InvariantCulture, "`{0}` just disabled", Id);
			}
			return string.Empty;
		}

		protected void Reset(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			Enabled = false;
			_overrides.Clear();
			bool? boolean = ParsingHelper.GetBoolean(dictionary, "enabled");
			if (boolean.HasValue)
			{
				Enabled = boolean.Value;
			}
			object value;
			if (!dictionary.TryGetValue("overrides", out value))
			{
				return;
			}
			Dictionary<string, object> dictionary2 = value as Dictionary<string, object>;
			if (dictionary2 == null)
			{
				return;
			}
			foreach (KeyValuePair<string, object> item in dictionary2)
			{
				Dictionary<string, object> dictionary3 = item.Value as Dictionary<string, object>;
				if (dictionary3 != null)
				{
					Overrides[item.Key] = dictionary3;
				}
			}
		}

		protected bool? GetBooleanOverride(string nodeKey, string category)
		{
			//Discarded unreachable code: IL_002a, IL_003f
			object nodeObjectOverride = GetNodeObjectOverride(nodeKey, category);
			if (nodeObjectOverride == null)
			{
				return null;
			}
			try
			{
				return Convert.ToBoolean(nodeObjectOverride);
			}
			catch
			{
				return null;
			}
		}

		protected int? GetInt32Override(string nodeKey, string category)
		{
			//Discarded unreachable code: IL_002a, IL_003f
			object nodeObjectOverride = GetNodeObjectOverride(nodeKey, category);
			if (nodeObjectOverride == null)
			{
				return null;
			}
			try
			{
				return Convert.ToInt32(nodeObjectOverride);
			}
			catch
			{
				return null;
			}
		}

		protected double? GetDoubleOverride(string nodeKey, string category)
		{
			//Discarded unreachable code: IL_002a, IL_003f
			object nodeObjectOverride = GetNodeObjectOverride(nodeKey, category);
			if (nodeObjectOverride == null)
			{
				return null;
			}
			try
			{
				return Convert.ToDouble(nodeObjectOverride);
			}
			catch
			{
				return null;
			}
		}

		protected string GetStringOverride(string nodeKey, string category)
		{
			object nodeObjectOverride = GetNodeObjectOverride(nodeKey, category);
			if (nodeObjectOverride == null)
			{
				return null;
			}
			return nodeObjectOverride as string;
		}

		protected object GetNodeObjectOverride(string nodeKey, string category)
		{
			if (nodeKey == null)
			{
				throw new ArgumentNullException("nodeKey");
			}
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			Dictionary<string, object> value;
			if (!Overrides.TryGetValue(category, out value))
			{
				return null;
			}
			object value2;
			if (!value.TryGetValue(nodeKey, out value2))
			{
				return null;
			}
			return value2;
		}

		private bool? GetEnabledOverride(string category)
		{
			return GetBooleanOverride("enabled", category);
		}
	}
}
