using UnityEngine;

namespace Rilisoft
{
	public class PrefsStringCachedProperty : CachedPropertyWithKeyBase<string>
	{
		public PrefsStringCachedProperty(string prefsKey)
			: base(prefsKey)
		{
		}

		protected override string GetValue()
		{
			return PlayerPrefs.GetString(base.PrefsKey, string.Empty);
		}

		protected override void SetValue(string value)
		{
			PlayerPrefs.SetString(base.PrefsKey, value);
		}
	}
}
