using UnityEngine;

namespace Rilisoft
{
	public class PrefsFloatCachedProperty : CachedPropertyWithKeyBase<float>
	{
		public PrefsFloatCachedProperty(string prefsKey)
			: base(prefsKey)
		{
		}

		protected override float GetValue()
		{
			return PlayerPrefs.GetFloat(base.PrefsKey, 0f);
		}

		protected override void SetValue(float value)
		{
			PlayerPrefs.SetFloat(base.PrefsKey, value);
		}
	}
}
