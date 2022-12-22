using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	public struct TrophiesSynchronizerGoogleSavedGameFacade
	{
		private abstract class Callback
		{
			protected TrophiesMemento? _resolvedTrophies;

			internal abstract void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata);

			protected abstract void HandleAuthenticationCompleted(bool succeeded);

			protected abstract void TrySetException(Exception ex);

			internal void HandleOpenConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
			{
				//Discarded unreachable code: IL_019e
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleOpenConflict('{1}', '{2}')", GetType().Name, original.Description, unmerged.Description);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
						return;
					}
					TrophiesMemento trophiesMemento = ParseTrophies(originalData);
					TrophiesMemento trophiesMemento2 = ParseTrophies(unmergedData);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("[Trophies] Original: {0}, unmerged: {1}", trophiesMemento, trophiesMemento2);
					}
					if (trophiesMemento.TrophiesNegative >= trophiesMemento2.TrophiesNegative && trophiesMemento.TrophiesPositive >= trophiesMemento2.TrophiesPositive)
					{
						resolver.ChooseMetadata(original);
						_resolvedTrophies = MergeWithResolved(trophiesMemento, false);
					}
					else if (trophiesMemento.TrophiesNegative <= trophiesMemento2.TrophiesNegative && trophiesMemento.TrophiesPositive <= trophiesMemento2.TrophiesPositive)
					{
						resolver.ChooseMetadata(unmerged);
						_resolvedTrophies = MergeWithResolved(trophiesMemento2, false);
					}
					else
					{
						ISavedGameMetadata savedGameMetadata;
						if (trophiesMemento.Trophies >= trophiesMemento2.Trophies)
						{
							savedGameMetadata = original;
						}
						else
						{
							savedGameMetadata = unmerged;
						}
						ISavedGameMetadata chosenMetadata = savedGameMetadata;
						resolver.ChooseMetadata(chosenMetadata);
						TrophiesMemento trophies = TrophiesMemento.Merge(trophiesMemento, trophiesMemento2);
						_resolvedTrophies = MergeWithResolved(trophies, true);
					}
					SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, HandleOpenConflict, HandleOpenCompleted);
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected TrophiesMemento MergeWithResolved(TrophiesMemento trophies, bool forceConflicted)
			{
				TrophiesMemento result = ((!_resolvedTrophies.HasValue) ? trophies : TrophiesMemento.Merge(_resolvedTrophies.Value, trophies));
				if (forceConflicted)
				{
					return new TrophiesMemento(result.TrophiesNegative, result.TrophiesPositive, result.Season, true);
				}
				return result;
			}

			protected static TrophiesMemento ParseTrophies(byte[] data)
			{
				//Discarded unreachable code: IL_0043, IL_0086
				if (data != null && data.Length > 0)
				{
					string @string = Encoding.UTF8.GetString(data, 0, data.Length);
					if (string.IsNullOrEmpty(@string))
					{
						return default(TrophiesMemento);
					}
					try
					{
						return JsonUtility.FromJson<TrophiesMemento>(@string);
					}
					catch (ArgumentException exception)
					{
						Debug.LogErrorFormat("Failed to deserialize {0}: \"{1}\"", typeof(TrophiesMemento).Name, @string);
						Debug.LogException(exception);
						return default(TrophiesMemento);
					}
				}
				return default(TrophiesMemento);
			}
		}

		private sealed class PushCallback : Callback
		{
			private readonly TrophiesMemento _trophies;

			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> _promise;

			public PushCallback(TrophiesMemento trophies, TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> promise)
			{
				_trophies = trophies;
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
			}

			internal override void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleOpenCompleted('{1}', '{2}')", GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
						return;
					}
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						TrophiesMemento trophiesMemento = MergeWithResolved(_trophies, false);
						string text2 = (trophiesMemento.Conflicted ? "resolved" : ((!_resolvedTrophies.HasValue) ? "none" : "trivial"));
						string description = string.Format(CultureInfo.InvariantCulture, "device:'{0}', conflict:'{1}'", SystemInfo.deviceModel, text2);
						SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(description).Build();
						string s = JsonUtility.ToJson(trophiesMemento);
						byte[] bytes = Encoding.UTF8.GetBytes(s);
						SavedGame.CommitUpdate(metadata, updateForMetadata, bytes, HandleCommitCompleted);
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
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

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleAuthenticationCompleted({1})", GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<ISavedGameMetadata>(SavedGameRequestStatus.AuthenticationError, null));
					}
					else if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
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

			private void HandleCommitCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleCommitCompleted('{1}', '{2}')", GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.TimeoutError:
						if (SavedGame == null)
						{
							TrySetException(new InvalidOperationException("SavedGameClient is null."));
						}
						else
						{
							SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						}
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
			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>> _promise;

			public PullCallback(TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>> promise)
			{
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>>();
			}

			internal override void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata)
			{
				string text = ((metadata == null) ? string.Empty : metadata.Description);
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleOpenCompleted('{1}', '{2}')", GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
						return;
					}
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
						SavedGame.ReadBinaryData(metadata, HandleReadCompleted);
						break;
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<TrophiesMemento>(requestStatus, default(TrophiesMemento)));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleAuthenticationCompleted({1})", GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<TrophiesMemento>(SavedGameRequestStatus.AuthenticationError, default(TrophiesMemento)));
					}
					else if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
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

			private void HandleReadCompleted(SavedGameRequestStatus requestStatus, byte[] data)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandleReadCompleted('{1}', {2})", GetType().Name, requestStatus, (data != null) ? data.Length : 0);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						TrophiesMemento trophiesMemento = Callback.ParseTrophies(data);
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("[Trophies] Incoming: {0}", trophiesMemento);
						}
						TrophiesMemento value = MergeWithResolved(trophiesMemento, false);
						_promise.TrySetResult(new GoogleSavedGameRequestResult<TrophiesMemento>(requestStatus, value));
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						if (SavedGame == null)
						{
							TrySetException(new InvalidOperationException("SavedGameClient is null."));
						}
						else
						{
							SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						}
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<TrophiesMemento>(requestStatus, default(TrophiesMemento)));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}
		}

		public const string Filename = "Trophies";

		private const string SavedGameClientIsNullMessage = "SavedGameClient is null.";

		private static ISavedGameClient SavedGame
		{
			get
			{
				//Discarded unreachable code: IL_0010, IL_001d
				try
				{
					return GpgFacade.Instance.SavedGame;
				}
				catch (NullReferenceException)
				{
					return null;
				}
			}
		}

		public Task<GoogleSavedGameRequestResult<TrophiesMemento>> Pull()
		{
			//Discarded unreachable code: IL_00c8
			string text = GetType().Name + ".Pull()";
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>> taskCompletionSource = new TaskCompletionSource<GoogleSavedGameRequestResult<TrophiesMemento>>();
				PullCallback pullCallback = new PullCallback(taskCompletionSource);
				if (SavedGame == null)
				{
					taskCompletionSource.TrySetException(new InvalidOperationException("SavedGameClient is null."));
					return taskCompletionSource.Task;
				}
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild);
				try
				{
					SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, pullCallback.HandleOpenConflict, pullCallback.HandleOpenCompleted);
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

		public Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> Push(TrophiesMemento trophies)
		{
			//Discarded unreachable code: IL_00d0
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.Push({1})", GetType().Name, trophies);
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> taskCompletionSource = new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
				PushCallback pushCallback = new PushCallback(trophies, taskCompletionSource);
				if (SavedGame == null)
				{
					taskCompletionSource.TrySetException(new InvalidOperationException("SavedGameClient is null."));
					return taskCompletionSource.Task;
				}
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild);
				try
				{
					SavedGame.OpenWithManualConflictResolution("Trophies", DataSource.ReadNetworkOnly, true, pushCallback.HandleOpenConflict, pushCallback.HandleOpenCompleted);
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
