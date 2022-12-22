using UnityEngine;

namespace Rilisoft
{
	public class StoragerStringCachedProperty : StoragerCachedPropertyBase<string>
	{
		public StoragerStringCachedProperty(string prefsKey, bool useICloud = false)
			: base(prefsKey, useICloud)
		{
		}

		protected override string GetValue()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && !Storager.hasKey(base.PrefsKey))
			{
				Storager.setString(base.PrefsKey, string.Empty, base.UseICloud);
			}
			return Storager.getString(base.PrefsKey, base.UseICloud);
		}

		protected override void SetValue(string value)
		{
			Storager.setString(base.PrefsKey, value, base.UseICloud);
		}
	}
}
