using System.Collections;
using System.Collections.Generic;

namespace Rilisoft
{
	public sealed class Awaiter
	{
		private readonly List<IEnumerator> _iters = new List<IEnumerator>();

		private readonly List<IEnumerator> _itersToRemove = new List<IEnumerator>();

		public void Register(IEnumerator iter)
		{
			if (iter != null && !_iters.Contains(iter))
			{
				_iters.Add(iter);
			}
		}

		public void Remove(IEnumerator iter)
		{
			if (iter != null && _iters.Contains(iter))
			{
				_iters.Remove(iter);
			}
		}

		public void Tick()
		{
			int count = _iters.Count;
			for (int i = 0; i < count; i++)
			{
				IEnumerator enumerator = _iters[i];
				if (!enumerator.MoveNext())
				{
					_itersToRemove.Add(enumerator);
				}
			}
			int count2 = _itersToRemove.Count;
			if (count2 > 0)
			{
				for (int j = 0; j < count2; j++)
				{
					IEnumerator item = _itersToRemove[j];
					_iters.Remove(item);
				}
				_itersToRemove.Clear();
			}
		}
	}
}
