using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class DummyConflictResolver : IConflictResolver
	{
		private static readonly DummyConflictResolver s_instance = new DummyConflictResolver();

		internal static DummyConflictResolver Instance
		{
			get
			{
				return s_instance;
			}
		}

		private DummyConflictResolver()
		{
		}

		public void ChooseMetadata(ISavedGameMetadata chosenMetadata)
		{
			string text = ((chosenMetadata == null) ? string.Empty : chosenMetadata.Filename);
			Debug.LogFormat("{0}('{1}').ChooseMetadata()", GetType().Name, text);
		}
	}
}
