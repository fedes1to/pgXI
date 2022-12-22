using System;
using System.Collections;
using System.Collections.Generic;

namespace Rilisoft
{
	public class ObservableList<T> : List<T>, IEnumerable, IEnumerable<T>, IList<T>, ICollection<T>
	{
		public Action<int, T> OnItemInserted;

		public Action<int, T> OnItemRemoved;

		public ObservableList()
		{
		}

		public ObservableList(IEnumerable<T> collection)
		{
			base.AddRange(collection);
		}

		public void Add(T item, bool silent = false)
		{
			base.Add(item);
			if (OnItemInserted != null && !silent)
			{
				OnItemInserted(Count - 1, item);
			}
		}

		public void AddRange(IEnumerable<T> collection, bool silent = false)
		{
			base.AddRange(collection);
			if (OnItemInserted == null || silent)
			{
				return;
			}
			int num = 0;
			foreach (T item in collection)
			{
				OnItemInserted(num, item);
				num++;
			}
		}

		public void Insert(int index, T item, bool silent = false)
		{
			base.Insert(index, item);
			if (OnItemInserted != null && !silent)
			{
				OnItemInserted(index, item);
			}
		}

		public void RemoveAt(int index, bool silent = false)
		{
			T arg = this[index];
			base.RemoveAt(index);
			if (OnItemRemoved != null && !silent)
			{
				OnItemRemoved(index, arg);
			}
		}

		public void Remove(T item, bool silent = false)
		{
			int arg = IndexOf(item);
			base.Remove(item);
			if (OnItemRemoved != null && !silent)
			{
				OnItemRemoved(arg, item);
			}
		}

		public void RemoveRange(int idx, int count, bool silent = false)
		{
			if (OnItemRemoved != null && !silent)
			{
				List<T> list = new List<T>();
				for (int i = 0; i <= count; i++)
				{
					list.Add(base[idx + i]);
				}
				base.RemoveRange(idx, count);
				int num = 0;
				{
					foreach (T item in list)
					{
						OnItemRemoved(idx + num, item);
						num++;
					}
					return;
				}
			}
			base.RemoveRange(idx, count);
		}
	}
}
