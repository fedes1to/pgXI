using System.Collections.Generic;

namespace com.amazon.mas.cpt.ads
{
	public sealed class AdLoadedDelegator : IDelegator
	{
		public readonly AdLoadedDelegate responseDelegate;

		public AdLoadedDelegator(AdLoadedDelegate responseDelegate)
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
