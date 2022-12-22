using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class CampaignAdPointMemento : LevelCompleteAdPointMementoBase
	{
		public CampaignAdPointMemento(string id)
			: base(id)
		{
		}

		internal static CampaignAdPointMemento FromObject(string id, object obj)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (obj == null)
			{
				return null;
			}
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return null;
			}
			CampaignAdPointMemento campaignAdPointMemento = new CampaignAdPointMemento(id);
			campaignAdPointMemento.Reset(dictionary);
			return campaignAdPointMemento;
		}
	}
}
