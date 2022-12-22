using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	public struct SkinsSynchronizerGoogleSavedGameFacade
	{
		private abstract class Callback
		{
			protected SkinsMemento? _resolved;

			protected abstract DataSource DefaultDataSource { get; }

			internal abstract void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata);

			protected abstract void HandleAuthenticationCompleted(bool succeeded);

			protected abstract void TrySetException(Exception ex);

			internal void HandleOpenConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenConflict('{2}', '{3}')", typeof(SkinsSynchronizerGoogleSavedGameFacade).Name, GetType().Name, original.Description, unmerged.Description);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
						return;
					}
					SkinsMemento skinsMemento = Parse(originalData);
					SkinsMemento skinsMemento2 = Parse(unmergedData);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("[Skins] Original: {0}, unmerged: {1}", skinsMemento, skinsMemento2);
					}
					HashSet<string> hashSet = new HashSet<string>(skinsMemento.Skins.Select((SkinMemento s) => s.Id));
					HashSet<string> hashSet2 = new HashSet<string>(skinsMemento2.Skins.Select((SkinMemento s) => s.Id));
					if (hashSet.IsSupersetOf(hashSet2))
					{
						resolver.ChooseMetadata(original);
						_resolved = MergeWithResolved(skinsMemento, false);
					}
					else if (hashSet.IsProperSubsetOf(hashSet2))
					{
						resolver.ChooseMetadata(unmerged);
						_resolved = MergeWithResolved(skinsMemento2, false);
					}
					else
					{
						ISavedGameMetadata savedGameMetadata;
						if (hashSet.Count >= hashSet2.Count)
						{
							savedGameMetadata = original;
						}
						else
						{
							savedGameMetadata = unmerged;
						}
						ISavedGameMetadata chosenMetadata = savedGameMetadata;
						resolver.ChooseMetadata(chosenMetadata);
						SkinsMemento skins = SkinsMemento.Merge(skinsMemento, skinsMemento2);
						_resolved = MergeWithResolved(skins, true);
					}
					SavedGame.OpenWithManualConflictResolution("Skins", DefaultDataSource, true, HandleOpenConflict, HandleOpenCompleted);
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected SkinsMemento MergeWithResolved(SkinsMemento skins, bool forceConflicted)
			{
				SkinsMemento result = ((!_resolved.HasValue) ? skins : SkinsMemento.Merge(_resolved.Value, skins));
				if (forceConflicted)
				{
					return new SkinsMemento(result.Skins, result.DeletedSkins, result.Cape, true);
				}
				return result;
			}

			protected static SkinsMemento Parse(byte[] data)
			{
				//Discarded unreachable code: IL_004e, IL_0091
				if (data == null || data.Length <= 0)
				{
					return default(SkinsMemento);
				}
				string @string = Encoding.UTF8.GetString(data, 0, data.Length);
				if (string.IsNullOrEmpty(@string))
				{
					return default(SkinsMemento);
				}
				try
				{
					return JsonUtility.FromJson<SkinsMemento>(@string);
				}
				catch (ArgumentException exception)
				{
					Debug.LogErrorFormat("Failed to deserialize {0}: \"{1}\"", typeof(SkinsMemento).Name, @string);
					Debug.LogException(exception);
					return default(SkinsMemento);
				}
			}
		}

		private sealed class PushCallback : Callback
		{
			private readonly SkinsMemento _skins;

			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> _promise;

			protected override DataSource DefaultDataSource
			{
				get
				{
					return DataSource.ReadNetworkOnly;
				}
			}

			public PushCallback(SkinsMemento skins, TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> promise)
			{
				_skins = skins;
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleAuthenticationCompleted({2})", typeof(SkinsSynchronizerGoogleSavedGameFacade).Name, GetType().Name, succeeded);
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
						SavedGame.OpenWithManualConflictResolution("Skins", DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
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
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenCompleted('{2}', '{3}')", typeof(SkinsSynchronizerGoogleSavedGameFacade).Name, GetType().Name, requestStatus, text);
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
						SkinsMemento skinsMemento = MergeWithResolved(_skins, false);
						string text2 = (skinsMemento.Conflicted ? "resolved" : ((!_resolved.HasValue) ? "none" : "trivial"));
						string description = string.Format(CultureInfo.InvariantCulture, "device:'{0}', conflict:'{1}'", SystemInfo.deviceModel, text2);
						SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(description).Build();
						string s = JsonUtility.ToJson(skinsMemento);
						byte[] bytes = Encoding.UTF8.GetBytes(s);
						SavedGame.CommitUpdate(metadata, updateForMetadata, bytes, HandleCommitCompleted);
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Skins", DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
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
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleCommitCompleted('{2}', '{3}')", typeof(SkinsSynchronizerGoogleSavedGameFacade).Name, GetType().Name, requestStatus, text);
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
							SavedGame.OpenWithManualConflictResolution("Skins", DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
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
			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<SkinsMemento>> _promise;

			protected override DataSource DefaultDataSource
			{
				get
				{
					return DataSource.ReadNetworkOnly;
				}
			}

			public PullCallback(TaskCompletionSource<GoogleSavedGameRequestResult<SkinsMemento>> promise)
			{
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<SkinsMemento>>();
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleAuthenticationCompleted({2})", typeof(SkinsSynchronizerGoogleSavedGameFacade).Name, GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<SkinsMemento>(SavedGameRequestStatus.AuthenticationError, default(SkinsMemento)));
					}
					else if (SavedGame == null)
					{
						TrySetException(new InvalidOperationException("SavedGameClient is null."));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Skins", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
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
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenCompleted('{2}', '{3}')", typeof(SkinsSynchronizerGoogleSavedGameFacade).Name, GetType().Name, requestStatus, text);
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
						SavedGame.OpenWithManualConflictResolution("Skins", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<SkinsMemento>(requestStatus, default(SkinsMemento)));
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
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleReadCompleted('{2}', {3})", typeof(SkinsSynchronizerGoogleSavedGameFacade).Name, GetType().Name, requestStatus, (data != null) ? data.Length : 0);
				ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						SkinsMemento skinsMemento = Callback.Parse(data);
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("[Skins] Incoming: {0}", skinsMemento);
						}
						SkinsMemento value = MergeWithResolved(skinsMemento, false);
						_promise.TrySetResult(new GoogleSavedGameRequestResult<SkinsMemento>(requestStatus, value));
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						if (SavedGame == null)
						{
							TrySetException(new InvalidOperationException("SavedGameClient is null."));
						}
						else
						{
							SavedGame.OpenWithManualConflictResolution("Skins", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						}
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<SkinsMemento>(requestStatus, default(SkinsMemento)));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}
		}

		public const string Filename = "Skins";

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

		public Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> Push(SkinsMemento skins)
		{
			//Discarded unreachable code: IL_00d3
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.Push({1})", GetType().Name, skins);
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> taskCompletionSource = new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
				if (SavedGame == null)
				{
					taskCompletionSource.TrySetException(new InvalidOperationException("SavedGameClient is null."));
					return taskCompletionSource.Task;
				}
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild);
				try
				{
					PushCallback pushCallback = new PushCallback(skins, taskCompletionSource);
					SavedGame.OpenWithManualConflictResolution("Skins", DataSource.ReadNetworkOnly, true, pushCallback.HandleOpenConflict, pushCallback.HandleOpenCompleted);
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

		public Task<GoogleSavedGameRequestResult<SkinsMemento>> Pull()
		{
			//Discarded unreachable code: IL_00cb
			string text = GetType().Name + ".Pull()";
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<SkinsMemento>> taskCompletionSource = new TaskCompletionSource<GoogleSavedGameRequestResult<SkinsMemento>>();
				if (SavedGame == null)
				{
					taskCompletionSource.TrySetException(new InvalidOperationException("SavedGameClient is null."));
					return taskCompletionSource.Task;
				}
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild);
				try
				{
					PullCallback pullCallback = new PullCallback(taskCompletionSource);
					SavedGame.OpenWithManualConflictResolution("Skins", DataSource.ReadNetworkOnly, true, pullCallback.HandleOpenConflict, pullCallback.HandleOpenCompleted);
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
