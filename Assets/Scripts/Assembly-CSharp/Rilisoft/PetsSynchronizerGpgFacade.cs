using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	internal struct PetsSynchronizerGpgFacade
	{
		private abstract class Callback
		{
			protected PlayerPets _resolved;

			protected abstract DataSource DefaultDataSource { get; }

			internal abstract void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata);

			protected abstract void HandleAuthenticationCompleted(bool succeeded);

			protected abstract void TrySetException(Exception ex);

			internal void HandleOpenConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenConflict('{2}', '{3}')", typeof(PetsSynchronizerGpgFacade).Name, GetType().Name, original.Description, unmerged.Description);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					PlayerPets playerPets = Parse(originalData);
					PlayerPets playerPets2 = Parse(unmergedData);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("[Pets] original: {0}, unmerged: {1}", playerPets, playerPets2);
					}
					int count = playerPets.Pets.Count;
					int count2 = playerPets2.Pets.Count;
					ISavedGameMetadata savedGameMetadata;
					if (count >= count2)
					{
						savedGameMetadata = original;
					}
					else
					{
						savedGameMetadata = unmerged;
					}
					ISavedGameMetadata chosenMetadata = savedGameMetadata;
					resolver.ChooseMetadata(chosenMetadata);
					PlayerPets other = PlayerPets.Merge(playerPets, playerPets2);
					_resolved = MergeWithResolved(other, true);
					SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DefaultDataSource, true, HandleOpenConflict, HandleOpenCompleted);
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected PlayerPets MergeWithResolved(PlayerPets other, bool forceConflicted)
			{
				PlayerPets playerPets = ((_resolved == null) ? other : PlayerPets.Merge(_resolved, other));
				if (forceConflicted)
				{
					playerPets.Conflicted = true;
				}
				return playerPets;
			}

			protected static PlayerPets Parse(byte[] data)
			{
				//Discarded unreachable code: IL_0044, IL_0081
				if (data == null || data.Length <= 0)
				{
					return new PlayerPets();
				}
				string @string = Encoding.UTF8.GetString(data, 0, data.Length);
				if (string.IsNullOrEmpty(@string))
				{
					return new PlayerPets();
				}
				try
				{
					return JsonUtility.FromJson<PlayerPets>(@string);
				}
				catch (ArgumentException exception)
				{
					Debug.LogErrorFormat("Failed to deserialize {0}: \"{1}\"", typeof(PlayerPets).Name, @string);
					Debug.LogException(exception);
					return new PlayerPets();
				}
			}
		}

		private sealed class PushCallback : Callback
		{
			private readonly PlayerPets _petsMemento;

			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> _promise;

			protected override DataSource DefaultDataSource
			{
				get
				{
					return DataSource.ReadNetworkOnly;
				}
			}

			public PushCallback(PlayerPets petsMemento, TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> promise)
			{
				_petsMemento = petsMemento;
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleAuthenticationCompleted({2})", typeof(PetsSynchronizerGpgFacade).Name, GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<ISavedGameMetadata>(SavedGameRequestStatus.AuthenticationError, null));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected override void TrySetException(Exception ex)
			{
				_promise.TrySetException(ex);
			}

			internal override void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenCompleted('{2}', '{3}')", typeof(PetsSynchronizerGpgFacade).Name, GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						PlayerPets playerPets = MergeWithResolved(_petsMemento, false);
						string text2 = (playerPets.Conflicted ? "resolved" : ((_resolved == null) ? "none" : "trivial"));
						string description = string.Format(CultureInfo.InvariantCulture, "device:'{0}', conflict:'{1}'", SystemInfo.deviceModel, text2);
						SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(description).Build();
						string s = JsonUtility.ToJson(playerPets);
						byte[] bytes = Encoding.UTF8.GetBytes(s);
						SavedGame.CommitUpdate(metadata, updateForMetadata, bytes, HandleCommitCompleted);
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<ISavedGameMetadata>(requestStatus, metadata));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			private void HandleCommitCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleCommitCompleted('{2}', '{3}')", typeof(PetsSynchronizerGpgFacade).Name, GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
					{
						GoogleSavedGameRequestResult<ISavedGameMetadata> result = new GoogleSavedGameRequestResult<ISavedGameMetadata>(requestStatus, metadata);
						_promise.TrySetResult(result);
						break;
					}
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}
		}

		private sealed class PullCallback : Callback
		{
			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<PlayerPets>> _promise;

			protected override DataSource DefaultDataSource
			{
				get
				{
					return DataSource.ReadNetworkOnly;
				}
			}

			public PullCallback(TaskCompletionSource<GoogleSavedGameRequestResult<PlayerPets>> promise)
			{
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<PlayerPets>>();
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleAuthenticationCompleted({2})", typeof(PetsSynchronizerGpgFacade).Name, GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<PlayerPets>(SavedGameRequestStatus.AuthenticationError, new PlayerPets()));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected override void TrySetException(Exception ex)
			{
				_promise.TrySetException(ex);
			}

			internal override void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenCompleted('{2}', '{3}')", typeof(PetsSynchronizerGpgFacade).Name, GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
						SavedGame.ReadBinaryData(metadata, HandleReadCompleted);
						break;
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<PlayerPets>(requestStatus, new PlayerPets()));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			private void HandleReadCompleted(SavedGameRequestStatus requestStatus, byte[] data)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleReadCompleted('{2}', {3})", typeof(PetsSynchronizerGpgFacade).Name, GetType().Name, requestStatus, (data != null) ? data.Length : 0);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						PlayerPets playerPets = Callback.Parse(data);
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("[Pets] Incoming: {0}", playerPets);
						}
						PlayerPets value = MergeWithResolved(playerPets, false);
						_promise.TrySetResult(new GoogleSavedGameRequestResult<PlayerPets>(requestStatus, value));
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<PlayerPets>(requestStatus, new PlayerPets()));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}
		}

		public const string Filename = "Pets.PlayerPets";

		private static readonly DummySavedGameClient _dummy = new DummySavedGameClient("Pets.PlayerPets");

		private static ISavedGameClient SavedGame
		{
			get
			{
				//Discarded unreachable code: IL_002a, IL_003b
				try
				{
					if (GpgFacade.Instance.SavedGame == null)
					{
						return _dummy;
					}
					return GpgFacade.Instance.SavedGame;
				}
				catch (NullReferenceException)
				{
					return _dummy;
				}
			}
		}

		public Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> Push(PlayerPets petsMemento)
		{
			//Discarded unreachable code: IL_00d5
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.Push({1})", GetType().Name, petsMemento.Pets.Count);
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> taskCompletionSource = new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild && !Application.isEditor);
				try
				{
					PushCallback pushCallback = new PushCallback(petsMemento, taskCompletionSource);
					SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DataSource.ReadNetworkOnly, true, pushCallback.HandleOpenConflict, pushCallback.HandleOpenCompleted);
				}
				finally
				{
					scopeLogger2.Dispose();
				}
				return taskCompletionSource.Task;
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		public Task<GoogleSavedGameRequestResult<PlayerPets>> Pull()
		{
			//Discarded unreachable code: IL_00b3
			string text = GetType().Name + ".Pull()";
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<PlayerPets>> taskCompletionSource = new TaskCompletionSource<GoogleSavedGameRequestResult<PlayerPets>>();
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild && !Application.isEditor);
				try
				{
					PullCallback pullCallback = new PullCallback(taskCompletionSource);
					SavedGame.OpenWithManualConflictResolution("Pets.PlayerPets", DataSource.ReadNetworkOnly, true, pullCallback.HandleOpenConflict, pullCallback.HandleOpenCompleted);
				}
				finally
				{
					scopeLogger2.Dispose();
				}
				return taskCompletionSource.Task;
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}
	}
}
