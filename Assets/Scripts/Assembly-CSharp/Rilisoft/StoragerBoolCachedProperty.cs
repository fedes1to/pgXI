namespace Rilisoft
{
	public class StoragerBoolCachedProperty : StoragerCachedPropertyBase<bool>
	{
		public StoragerBoolCachedProperty(string prefsKey, bool useICloud = false)
			: base(prefsKey, useICloud)
		{
		}

		protected override bool GetValue()
		{
			return Storager.getInt(base.PrefsKey, base.UseICloud).ToBool();
		}

		protected override void SetValue(bool value)
		{
			Storager.setInt(base.PrefsKey, value.ToInt(), base.UseICloud);
		}
	}
}
