using System;
using System.Threading.Tasks;

namespace Rilisoft
{
	public class PurchasesSavingEventArgs : EventArgs
	{
		public Task<bool> Future { get; private set; }

		public PurchasesSavingEventArgs(Task<bool> future)
		{
			Future = future;
		}
	}
}
