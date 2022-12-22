namespace Rilisoft
{
	public class SaltedFloat
	{
		private float[] _values = new float[5];

		private int _index;

		public float value
		{
			get
			{
				return _values[_index];
			}
			set
			{
				if (++_index >= _values.Length)
				{
					_index = 0;
				}
				_values[_index] = value;
			}
		}

		public SaltedFloat(float value)
		{
			this.value = value;
		}

		public SaltedFloat()
			: this(0f)
		{
		}
	}
}
