using System.Collections.Generic;

namespace com.amazon.mas.cpt.ads
{
	public interface IDelegator
	{
		void ExecuteSuccess();

		void ExecuteSuccess(Dictionary<string, object> objDict);

		void ExecuteError(AmazonException e);
	}
}
