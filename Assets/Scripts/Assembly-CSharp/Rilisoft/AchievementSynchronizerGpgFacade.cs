using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Rilisoft.NullExtensions;
using UnityEngine;

namespace Rilisoft
{
	internal struct AchievementSynchronizerGpgFacade
	{
		private abstract class Callback
		{
			protected AchievementProgressSyncObject _resolved;

			protected DataSource DefaultDataSource
			{
				get
				{
					return DataSource.ReadNetworkOnly;
				}
			}

			internal abstract void HandleOpenCompleted(SavedGameRequestStatus requestStatus, ISavedGameMetadata metadata);

			protected abstract void HandleAuthenticationCompleted(bool succeeded);

			protected abstract void TrySetException(Exception ex);

			internal void HandleOpenConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenConflict('{2}', '{3}')", typeof(AchievementSynchronizerGpgFacade).Name, GetType().Name, original.Description, unmerged.Description);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					AchievementProgressSyncObject achievementProgressSyncObject = Parse(originalData);
					AchievementProgressSyncObject achievementProgressSyncObject2 = Parse(unmergedData);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("[Achievements] original: {0}, unmerged: {1}", achievementProgressSyncObject, achievementProgressSyncObject2);
					}
					int count = achievementProgressSyncObject.ProgressData.Count;
					int count2 = achievementProgressSyncObject2.ProgressData.Count;
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
					AchievementProgressSyncObject other = AchievementProgressData.Merge(achievementProgressSyncObject, achievementProgressSyncObject2);
					_resolved = MergeWithResolved(other, true);
					SavedGame.OpenWithManualConflictResolution("Achievements", DefaultDataSource, true, HandleOpenConflict, HandleOpenCompleted);
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}

			protected AchievementProgressSyncObject MergeWithResolved(AchievementProgressSyncObject other, bool forceConflicted)
			{
				AchievementProgressSyncObject achievementProgressSyncObject = ((_resolved == null) ? other : AchievementProgressData.Merge(_resolved, other));
				if (forceConflicted)
				{
					achievementProgressSyncObject.SetConflicted();
				}
				return achievementProgressSyncObject;
			}

			protected static AchievementProgressSyncObject Parse(byte[] data)
			{
				//Discarded unreachable code: IL_0044, IL_0081
				if (data == null || data.Length <= 0)
				{
					return new AchievementProgressSyncObject();
				}
				string @string = Encoding.UTF8.GetString(data, 0, data.Length);
				if (string.IsNullOrEmpty(@string))
				{
					return new AchievementProgressSyncObject();
				}
				try
				{
					return AchievementProgressSyncObject.FromJson(@string);
				}
				catch (ArgumentException exception)
				{
					Debug.LogErrorFormat("Failed to deserialize {0}: \"{1}\"", typeof(AchievementProgressSyncObject).Name, @string);
					Debug.LogException(exception);
					return new AchievementProgressSyncObject();
				}
			}
		}

		private sealed class PushCallback : Callback
		{
			private readonly AchievementProgressSyncObject _memento;

			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> _promise;

			public PushCallback(AchievementProgressSyncObject memento, TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> promise)
			{
				if (memento == null)
				{
					throw new ArgumentNullException("memento");
				}
				_memento = memento;
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleAuthenticationCompleted({2})", typeof(AchievementSynchronizerGpgFacade).Name, GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<ISavedGameMetadata>(SavedGameRequestStatus.AuthenticationError, null));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Achievements", base.DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
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
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenCompleted('{2}', '{3}')", typeof(AchievementSynchronizerGpgFacade).Name, GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						AchievementProgressSyncObject achievementProgressSyncObject = MergeWithResolved(_memento, false);
						string text2 = (achievementProgressSyncObject.Conflicted ? "resolved" : ((_resolved == null) ? "none" : "trivial"));
						string description = string.Format(CultureInfo.InvariantCulture, "device:'{0}', conflict:'{1}'", SystemInfo.deviceModel, text2);
						SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(description).Build();
						string s = AchievementProgressSyncObject.ToJson(achievementProgressSyncObject);
						byte[] bytes = Encoding.UTF8.GetBytes(s);
						SavedGame.CommitUpdate(metadata, updateForMetadata, bytes, HandleCommitCompleted);
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Achievements", base.DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
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
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleCommitCompleted('{2}', '{3}')", typeof(AchievementSynchronizerGpgFacade).Name, GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Achievements", base.DefaultDataSource, true, base.HandleOpenConflict, HandleOpenCompleted);
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
			private readonly TaskCompletionSource<GoogleSavedGameRequestResult<AchievementProgressSyncObject>> _promise;

			public PullCallback(TaskCompletionSource<GoogleSavedGameRequestResult<AchievementProgressSyncObject>> promise)
			{
				_promise = promise ?? new TaskCompletionSource<GoogleSavedGameRequestResult<AchievementProgressSyncObject>>();
			}

			protected override void HandleAuthenticationCompleted(bool succeeded)
			{
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleAuthenticationCompleted({2})", typeof(AchievementSynchronizerGpgFacade).Name, GetType().Name, succeeded);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					if (!succeeded)
					{
						_promise.TrySetResult(new GoogleSavedGameRequestResult<AchievementProgressSyncObject>(SavedGameRequestStatus.AuthenticationError, new AchievementProgressSyncObject()));
					}
					else
					{
						SavedGame.OpenWithManualConflictResolution("Achievements", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
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
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleOpenCompleted('{2}', '{3}')", typeof(AchievementSynchronizerGpgFacade).Name, GetType().Name, requestStatus, text);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
						SavedGame.ReadBinaryData(metadata, HandleReadCompleted);
						break;
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Achievements", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<AchievementProgressSyncObject>(requestStatus, new AchievementProgressSyncObject()));
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
				string callee = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.HandleReadCompleted('{2}', {3})", typeof(AchievementSynchronizerGpgFacade).Name, GetType().Name, requestStatus, (data != null) ? data.Length : 0);
				ScopeLogger scopeLogger = new ScopeLogger(callee, false);
				try
				{
					switch (requestStatus)
					{
					case SavedGameRequestStatus.Success:
					{
						AchievementProgressSyncObject achievementProgressSyncObject = Callback.Parse(data);
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("[Achievements] Incoming: {0}", achievementProgressSyncObject);
						}
						AchievementProgressSyncObject value = MergeWithResolved(achievementProgressSyncObject, false);
						_promise.TrySetResult(new GoogleSavedGameRequestResult<AchievementProgressSyncObject>(requestStatus, value));
						break;
					}
					case SavedGameRequestStatus.TimeoutError:
						SavedGame.OpenWithManualConflictResolution("Achievements", DataSource.ReadNetworkOnly, true, base.HandleOpenConflict, HandleOpenCompleted);
						break;
					case SavedGameRequestStatus.AuthenticationError:
						GpgFacade.Instance.Authenticate(HandleAuthenticationCompleted, true);
						break;
					default:
						_promise.TrySetResult(new GoogleSavedGameRequestResult<AchievementProgressSyncObject>(requestStatus, new AchievementProgressSyncObject()));
						break;
					}
				}
				finally
				{
					scopeLogger.Dispose();
				}
			}
		}

		public const string Filename = "Achievements";

		private static readonly DummySavedGameClient _dummy = new DummySavedGameClient("Achievements");

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

		public Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> Push(AchievementProgressSyncObject achievementMemento)
		{
			//Discarded unreachable code: IL_00f2
			string text = string.Format(CultureInfo.InvariantCulture, "{0}.Push({1})", GetType().Name, achievementMemento.ProgressData.Map((List<AchievementProgressData> l) => l.Count));
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>> taskCompletionSource = new TaskCompletionSource<GoogleSavedGameRequestResult<ISavedGameMetadata>>();
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild && !Application.isEditor);
				try
				{
					PushCallback pushCallback = new PushCallback(achievementMemento, taskCompletionSource);
					SavedGame.OpenWithManualConflictResolution("Achievements", DataSource.ReadNetworkOnly, true, pushCallback.HandleOpenConflict, pushCallback.HandleOpenCompleted);
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

		public Task<GoogleSavedGameRequestResult<AchievementProgressSyncObject>> Pull()
		{
			//Discarded unreachable code: IL_00b3
			string text = GetType().Name + ".Pull()";
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				TaskCompletionSource<GoogleSavedGameRequestResult<AchievementProgressSyncObject>> taskCompletionSource = new TaskCompletionSource<GoogleSavedGameRequestResult<AchievementProgressSyncObject>>();
				ScopeLogger scopeLogger2 = new ScopeLogger(text, "OpenWithManualConflictResolution", Defs.IsDeveloperBuild && !Application.isEditor);
				try
				{
					PullCallback pullCallback = new PullCallback(taskCompletionSource);
					SavedGame.OpenWithManualConflictResolution("Achievements", DataSource.ReadNetworkOnly, true, pullCallback.HandleOpenConflict, pullCallback.HandleOpenCompleted);
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
