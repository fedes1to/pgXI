using System.Collections.Generic;

namespace com.amazon.mas.cpt.ads
{
	public sealed class AdDismissedDelegator : IDelegator
	{
		public readonly AdDismissedDelegate responseDelegate;

		public AdDismissedDelegator(AdDismissedDelegate responseDelegate)
		{
			this.responseDelegate = responseDelegate;
		}

		public void ExecuteSuccess()
		{
		}

		public void ExecuteSuccess(Dictionary<string, object> objectDictionary)
		{
			responseDelegate(Ad.CreateFromDictionary(objectDictionary));
		}

		public void ExecuteError(AmazonException e)
		{
		}
	}
}
