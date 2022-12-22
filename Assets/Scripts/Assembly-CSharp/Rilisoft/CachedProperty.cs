namespace Rilisoft
{
	public abstract class CachedProperty<T>
	{
		private T _value;

		protected bool ValueIsReaded { get; private set; }

		public T Value
		{
			get
			{
				if (!ValueIsReaded)
				{
					_value = GetValue();
					ValueIsReaded = true;
				}
				return _value;
			}
			set
			{
				_value = value;
				SetValue(_value);
			}
		}

		public bool HasValue
		{
			get
			{
				return ValueIsReaded;
			}
		}

		protected abstract T GetValue();

		protected abstract void SetValue(T value);
	}
}
