namespace Rilisoft
{
	public abstract class CachedPropertyWithKeyBase<T> : CachedProperty<T>
	{
		public string PrefsKey { get; protected set; }

		protected CachedPropertyWithKeyBase(string prefsKey)
		{
			PrefsKey = prefsKey;
		}
	}
}
