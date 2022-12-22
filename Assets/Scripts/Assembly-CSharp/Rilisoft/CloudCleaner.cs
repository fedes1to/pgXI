using System;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	[Obsolete]
	internal sealed class CloudCleaner
	{
		private static CloudCleaner _instance;

		public static CloudCleaner Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CloudCleaner();
				}
				return _instance;
			}
		}

		public void CleanSavedGameFile(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentException("Filename should not be empty", filename);
			}
			if (GpgFacade.Instance.SavedGame == null)
			{
				Debug.LogWarning("Saved game client is null.");
				return;
			}
			Action<ISavedGameMetadata> commit = delegate(ISavedGameMetadata metadata)
			{
				byte[] updatedBinaryData = new byte[0];
				SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(string.Format("Cleaned by '{0}'", SystemInfo.deviceModel)).Build();
				GpgFacade.Instance.SavedGame.CommitUpdate(metadata, updateForMetadata, updatedBinaryData, delegate(SavedGameRequestStatus writeStatus, ISavedGameMetadata closeMetadata)
				{
					Debug.LogFormat("------ Cleaned after conflict '{0}': {1} '{2}'", filename, writeStatus, closeMetadata.GetDescription());
				});
			};
			Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback = delegate(SavedGameRequestStatus openStatus, ISavedGameMetadata openMetadata)
			{
				Debug.LogFormat("------ Open '{0}': {1} '{2}'", filename, openStatus, openMetadata.GetDescription());
				if (openStatus == SavedGameRequestStatus.Success)
				{
					commit(openMetadata);
				}
			};
			ConflictCallback conflictCallback = delegate(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
			{
				resolver.ChooseMetadata(unmerged);
				Debug.LogFormat("------ Partially resolved using unmerged metadata '{0}': '{1}'", filename, unmerged.GetDescription());
				commit(unmerged);
			};
			Debug.LogFormat("------ Trying to open '{0}'...", filename);
			GpgFacade.Instance.SavedGame.OpenWithManualConflictResolution(filename, DataSource.ReadNetworkOnly, true, conflictCallback, completedCallback);
		}
	}
}
