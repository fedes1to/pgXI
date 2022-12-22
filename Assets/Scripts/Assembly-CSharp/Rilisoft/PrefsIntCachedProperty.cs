using UnityEngine;

namespace Rilisoft
{
	public class PrefsIntCachedProperty : CachedPropertyWithKeyBase<int>
	{
		public PrefsIntCachedProperty(string prefsKey)
			: base(prefsKey)
		{
		}

		protected override int GetValue()
		{
			return PlayerPrefs.GetInt(base.PrefsKey, 0);
		}

		protected override void SetValue(int value)
		{
			PlayerPrefs.SetInt(base.PrefsKey, value);
		}
	}
}
