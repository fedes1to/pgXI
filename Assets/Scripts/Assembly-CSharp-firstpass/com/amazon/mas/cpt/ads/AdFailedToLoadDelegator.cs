using System.Collections.Generic;

namespace com.amazon.mas.cpt.ads
{
	public sealed class AdFailedToLoadDelegator : IDelegator
	{
		public readonly AdFailedToLoadDelegate responseDelegate;

		public AdFailedToLoadDelegator(AdFailedToLoadDelegate responseDelegate)
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
