using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class AdPointsConfigMemento
	{
		public ChestInLobbyPointMemento ChestInLobby { get; private set; }

		public FreeSpinPointMemento FreeSpin { get; private set; }

		public ReturnInConnectSceneAdPointMemento ReturnInConnectScene { get; private set; }

		public CampaignAdPointMemento Campaign { get; private set; }

		public SurvivalArenaAdPointMemento SurvivalArena { get; private set; }

		public AfterMatchAdPointMemento AfterMatch { get; private set; }

		public PolygonAdPointMemento Polygon { get; private set; }

		internal static AdPointsConfigMemento FromDictionary(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			AdPointsConfigMemento adPointsConfigMemento = new AdPointsConfigMemento();
			object value;
			dictionary.TryGetValue("chestInLobby", out value);
			adPointsConfigMemento.ChestInLobby = ChestInLobbyPointMemento.FromObject("chestInLobby", value);
			object value2;
			dictionary.TryGetValue("freeSpin", out value2);
			adPointsConfigMemento.FreeSpin = FreeSpinPointMemento.FromObject("freeSpin", value2);
			object value3;
			dictionary.TryGetValue("returnInConnectScene", out value3);
			adPointsConfigMemento.ReturnInConnectScene = ReturnInConnectSceneAdPointMemento.FromObject("returnInConnectScene", value3);
			object value4;
			dictionary.TryGetValue("campaign", out value4);
			adPointsConfigMemento.Campaign = CampaignAdPointMemento.FromObject("campaign", value4);
			object value5;
			dictionary.TryGetValue("survivalArena", out value5);
			adPointsConfigMemento.SurvivalArena = SurvivalArenaAdPointMemento.FromObject("survivalArena", value5);
			object value6;
			dictionary.TryGetValue("afterMatch", out value6);
			adPointsConfigMemento.AfterMatch = AfterMatchAdPointMemento.FromObject("afterMatch", value6);
			object value7;
			dictionary.TryGetValue("polygon", out value7);
			adPointsConfigMemento.Polygon = PolygonAdPointMemento.FromObject("polygon", value7);
			return adPointsConfigMemento;
		}
	}
}
