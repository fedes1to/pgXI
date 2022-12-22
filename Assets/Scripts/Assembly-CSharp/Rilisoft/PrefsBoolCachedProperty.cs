using UnityEngine;

namespace Rilisoft
{
	public class PrefsBoolCachedProperty : CachedPropertyWithKeyBase<bool>
	{
		public PrefsBoolCachedProperty(string prefsKey)
			: base(prefsKey)
		{
		}

		protected override bool GetValue()
		{
			return PlayerPrefs.GetInt(base.PrefsKey, 0).ToBool();
		}

		protected override void SetValue(bool value)
		{
			PlayerPrefs.SetInt(base.PrefsKey, value.ToInt());
		}
	}
}
