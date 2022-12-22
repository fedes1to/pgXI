using System.Collections.Generic;

namespace Rilisoft
{
	internal sealed class GameRegimeComparer : IEqualityComparer<ConnectSceneNGUIController.RegimGame>
	{
		public bool Equals(ConnectSceneNGUIController.RegimGame x, ConnectSceneNGUIController.RegimGame y)
		{
			return x == y;
		}

		public int GetHashCode(ConnectSceneNGUIController.RegimGame obj)
		{
			return (int)obj;
		}
	}
}
