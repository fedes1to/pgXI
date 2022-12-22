namespace Rilisoft
{
	public abstract class StoragerCachedPropertyBase<T> : CachedPropertyWithKeyBase<T>
	{
		public bool UseICloud { get; private set; }

		protected StoragerCachedPropertyBase(string prefsKey, bool useICloud = false)
			: base(prefsKey)
		{
			UseICloud = useICloud;
		}
	}
}
