using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class DummySavedGameClient : ISavedGameClient
	{
		private int _conflictCounter;

		private readonly string _filename;

		private readonly DummySavedGameMetadata _dummySavedGameMetadata;

		public string Filename
		{
			get
			{
				return _filename;
			}
		}

		public DummySavedGameClient(string filename)
		{
			_filename = filename ?? string.Empty;
			_dummySavedGameMetadata = new DummySavedGameMetadata(_filename);
		}

		public void CommitUpdate(ISavedGameMetadata metadata, SavedGameMetadataUpdate updateForMetadata, byte[] updatedBinaryData, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
		{
			Debug.LogFormat("{0}('{1}').CommitUpdate()", GetType().Name, Filename);
			if (callback != null)
			{
				Action action = delegate
				{
					callback(SavedGameRequestStatus.Success, _dummySavedGameMetadata);
				};
				CoroutineRunner.Instance.StartCoroutine(WaitAndExecuteCoroutine(action));
			}
		}

		public void Delete(ISavedGameMetadata metadata)
		{
			Debug.LogFormat("{0}('{1}').Delete()", GetType().Name, Filename);
		}

		public void FetchAllSavedGames(DataSource source, Action<SavedGameRequestStatus, List<ISavedGameMetadata>> callback)
		{
			Debug.LogFormat("{0}('{1}').FetchAllSavedGames()", GetType().Name, Filename);
			if (callback != null)
			{
				Action action = delegate
				{
					callback(SavedGameRequestStatus.Success, new List<ISavedGameMetadata>());
				};
				CoroutineRunner.Instance.StartCoroutine(WaitAndExecuteCoroutine(action));
			}
		}

		public void OpenWithAutomaticConflictResolution(string filename, DataSource source, ConflictResolutionStrategy resolutionStrategy, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
		{
			Debug.LogFormat("{0}('{1}').OpenWithAutomaticConflictResolution()", GetType().Name, Filename);
			if (callback != null)
			{
				Action action = delegate
				{
					callback(SavedGameRequestStatus.Success, _dummySavedGameMetadata);
				};
				CoroutineRunner.Instance.StartCoroutine(WaitAndExecuteCoroutine(action));
			}
		}

		public void OpenWithManualConflictResolution(string filename, DataSource source, bool prefetchDataOnConflict, ConflictCallback conflictCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback)
		{
			bool flag = _conflictCounter % 2 == 0;
			Debug.LogFormat("{0}('{1}', {2}).OpenWithManualConflictResolution()", GetType().Name, Filename, flag);
			if (flag)
			{
				if (conflictCallback == null)
				{
					return;
				}
				byte[] data = Encoding.UTF8.GetBytes("{}");
				Action action = delegate
				{
					conflictCallback(DummyConflictResolver.Instance, _dummySavedGameMetadata, data, _dummySavedGameMetadata, data);
				};
				CoroutineRunner.Instance.StartCoroutine(WaitAndExecuteCoroutine(action));
			}
			else
			{
				if (completedCallback == null)
				{
					return;
				}
				Action action2 = delegate
				{
					completedCallback(SavedGameRequestStatus.Success, _dummySavedGameMetadata);
				};
				CoroutineRunner.Instance.StartCoroutine(WaitAndExecuteCoroutine(action2));
			}
			_conflictCounter++;
		}

		public void ReadBinaryData(ISavedGameMetadata metadata, Action<SavedGameRequestStatus, byte[]> completedCallback)
		{
			Debug.LogFormat("{0}('{1}').ReadBinaryData()", GetType().Name, Filename);
			if (completedCallback != null)
			{
				byte[] binaryData = Encoding.UTF8.GetBytes("{}");
				Action action = delegate
				{
					completedCallback(SavedGameRequestStatus.Success, binaryData);
				};
				CoroutineRunner.Instance.StartCoroutine(WaitAndExecuteCoroutine(action));
			}
		}

		public void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI, bool showDeleteSaveUI, Action<SelectUIStatus, ISavedGameMetadata> callback)
		{
			Debug.LogFormat("{0}('{1}').ShowSelectSavedGameUI()", GetType().Name, Filename);
			if (callback != null)
			{
				Action action = delegate
				{
					callback(SelectUIStatus.SavedGameSelected, _dummySavedGameMetadata);
				};
				CoroutineRunner.Instance.StartCoroutine(WaitAndExecuteCoroutine(action));
			}
		}

		private IEnumerator WaitAndExecuteCoroutine(Action action)
		{
			yield return null;
			action();
		}
	}
}
