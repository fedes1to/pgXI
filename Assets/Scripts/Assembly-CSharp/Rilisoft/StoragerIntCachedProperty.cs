namespace Rilisoft
{
	public class StoragerIntCachedProperty : StoragerCachedPropertyBase<int>
	{
		public StoragerIntCachedProperty(string prefsKey, bool useICloud = false)
			: base(prefsKey, useICloud)
		{
		}

		protected override int GetValue()
		{
			return Storager.getInt(base.PrefsKey, base.UseICloud);
		}

		protected override void SetValue(int value)
		{
			Storager.setInt(base.PrefsKey, value, base.UseICloud);
		}
	}
}
