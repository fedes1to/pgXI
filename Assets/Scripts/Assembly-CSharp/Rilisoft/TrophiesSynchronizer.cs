using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class TrophiesSynchronizer
	{
		private static readonly TrophiesSynchronizer _instance = new TrophiesSynchronizer();

		public static TrophiesSynchronizer Instance
		{
			get
			{
				return _instance;
			}
		}

		private bool Ready
		{
			get
			{
				return true;
			}
		}

		public event EventHandler Updated;

		private TrophiesSynchronizer()
		{
		}

		public Coroutine Push()
		{
			if (!Ready)
			{
				return null;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return CoroutineRunner.Instance.StartCoroutine(PushGoogleCoroutine());
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					SyncAmazon();
				}
			}
			return null;
		}

		public Coroutine Sync()
		{
			if (!Ready)
			{
				return null;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return CoroutineRunner.Instance.StartCoroutine(SyncGoogleCoroutine(false));
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					SyncAmazon();
				}
			}
			return null;
		}

		private void SyncAmazon()
		{
			string callee = string.Format(CultureInfo.InvariantCulture, "{0}.SyncAmazon()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
			try
			{
				AGSWhispersyncClient.Synchronize();
				using (AGSGameDataMap aGSGameDataMap = AGSWhispersyncClient.GetGameData())
				{
					if (aGSGameDataMap == null)
					{
						Debug.LogWarning("dataMap == null");
						return;
					}
					using (AGSGameDataMap aGSGameDataMap2 = aGSGameDataMap.GetMap("trophiesMap"))
					{
						if (aGSGameDataMap2 == null)
						{
							Debug.LogWarning("trophiesMap == null");
							return;
						}
						AGSSyncableNumber highestNumber = aGSGameDataMap2.GetHighestNumber("trophiesNegative");
						AGSSyncableNumber highestNumber2 = aGSGameDataMap2.GetHighestNumber("trophiesPositive");
						AGSSyncableNumber highestNumber3 = aGSGameDataMap2.GetHighestNumber("season");
						int num = ((highestNumber != null) ? highestNumber.AsInt() : 0);
						int num2 = ((highestNumber2 != null) ? highestNumber2.AsInt() : 0);
						int num3 = ((highestNumber3 != null) ? highestNumber3.AsInt() : 0);
						int negativeRating = RatingSystem.instance.negativeRating;
						int positiveRating = RatingSystem.instance.positiveRating;
						int num4 = positiveRating - negativeRating;
						int currentCompetition = FriendsController.sharedController.currentCompetition;
						bool flag = false;
						if (num3 == 0)
						{
							if (RatingSystem.instance.currentLeague != RatingSystem.RatingLeague.Adamant)
							{
								int num5 = negativeRating;
								if (num > negativeRating)
								{
									num5 = num;
									RatingSystem.instance.negativeRating = num5;
									flag = true;
								}
								int num6 = positiveRating;
								if (num2 > positiveRating)
								{
									num6 = num2;
									RatingSystem.instance.positiveRating = num6;
									flag = true;
								}
								int num7 = num6 - num5;
								int trophiesSeasonThreshold = RatingSystem.instance.TrophiesSeasonThreshold;
								if (num7 > trophiesSeasonThreshold)
								{
									int num8 = num7 - trophiesSeasonThreshold;
									num5 += num8;
									RatingSystem.instance.negativeRating = num5;
									flag = true;
									TournamentAvailableBannerWindow.CanShow = true;
								}
							}
						}
						else if (num3 > currentCompetition)
						{
							FriendsController.sharedController.currentCompetition = num3;
							RatingSystem.instance.negativeRating = num;
							RatingSystem.instance.positiveRating = num2;
							flag = true;
						}
						else if (num3 == currentCompetition)
						{
							int num9 = negativeRating;
							if (num > negativeRating)
							{
								num9 = num;
								RatingSystem.instance.negativeRating = num9;
								flag = true;
							}
							int num10 = positiveRating;
							if (num2 > positiveRating)
							{
								num10 = num2;
								RatingSystem.instance.positiveRating = num10;
								flag = true;
							}
						}
						EventHandler updated = this.Updated;
						if (flag && updated != null)
						{
							updated(this, EventArgs.Empty);
						}
						if (true)
						{
							highestNumber.Set(RatingSystem.instance.negativeRating);
							highestNumber2.Set(RatingSystem.instance.positiveRating);
							highestNumber3.Set(FriendsController.sharedController.currentCompetition);
						}
					}
					AGSWhispersyncClient.Synchronize();
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private IEnumerator SyncGoogleCoroutine(bool pullOnly)
		{
			if (!Ready)
			{
				yield break;
			}
			string thisName = string.Format(CultureInfo.InvariantCulture, "TrophiesSynchronizer.SyncGoogleCoroutine('{0}')", (!pullOnly) ? "sync" : "pull");
			ScopeLogger scopeLogger = new ScopeLogger(thisName, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				if (!Application.isEditor && GpgFacade.Instance.SavedGame == null)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("{0}: Waiting while `SavedGame == null`...", GetType().Name);
					}
					while (GpgFacade.Instance.SavedGame == null)
					{
						yield return null;
					}
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("{0}: `SavedGame != null`...", GetType().Name);
					}
				}
				TrophiesSynchronizerGoogleSavedGameFacade googleSavedGamesFacade = default(TrophiesSynchronizerGoogleSavedGameFacade);
				WaitForSeconds delay = new WaitForSeconds(30f);
				int i = 0;
				while (true)
				{
					string callee = string.Format(CultureInfo.InvariantCulture, "Pull and wait ({0})", i);
					using (ScopeLogger logger = new ScopeLogger(thisName, callee, Defs.IsDeveloperBuild && !Application.isEditor))
					{
						Task<GoogleSavedGameRequestResult<TrophiesMemento>> future = googleSavedGamesFacade.Pull();
						while (!future.IsCompleted)
						{
							yield return null;
						}
						logger.Dispose();
						if (future.IsFaulted)
						{
							Exception ex = future.Exception.InnerExceptions.FirstOrDefault() ?? future.Exception;
							Debug.LogWarning("Failed to pull trophies with exception: " + ex.Message);
							yield return delay;
						}
						else
						{
							SavedGameRequestStatus requestStatus = future.Result.RequestStatus;
							if (requestStatus == SavedGameRequestStatus.Success)
							{
								TrophiesMemento cloudTrophies = future.Result.Value;
								int localTrophiesNegative = RatingSystem.instance.negativeRating;
								int localTrophiesPositive = RatingSystem.instance.positiveRating;
								int localTrophies = localTrophiesPositive - localTrophiesNegative;
								bool cloudDirty = true;
								int localSeason = FriendsController.sharedController.currentCompetition;
								bool localDirty = false;
								if (cloudTrophies.Season == 0)
								{
									if (RatingSystem.instance.currentLeague != RatingSystem.RatingLeague.Adamant)
									{
										int newLocalTrophiesNegative = localTrophiesNegative;
										if (cloudTrophies.TrophiesNegative > localTrophiesNegative)
										{
											newLocalTrophiesNegative = cloudTrophies.TrophiesNegative;
											RatingSystem.instance.negativeRating = newLocalTrophiesNegative;
											localDirty = true;
										}
										int newLocalTrophiesPositive3 = localTrophiesPositive;
										if (cloudTrophies.TrophiesPositive > localTrophiesPositive)
										{
											newLocalTrophiesPositive3 = cloudTrophies.TrophiesPositive;
											RatingSystem.instance.positiveRating = newLocalTrophiesPositive3;
											localDirty = true;
										}
										int newLocalTrophies = newLocalTrophiesPositive3 - newLocalTrophiesNegative;
										int threshold = RatingSystem.instance.TrophiesSeasonThreshold;
										if (newLocalTrophies > threshold)
										{
											int compensate = newLocalTrophies - threshold;
											newLocalTrophiesNegative += compensate;
											RatingSystem.instance.negativeRating = newLocalTrophiesNegative;
											localDirty = true;
											TournamentAvailableBannerWindow.CanShow = true;
										}
									}
								}
								else if (cloudTrophies.Season > localSeason)
								{
									FriendsController.sharedController.currentCompetition = cloudTrophies.Season;
									RatingSystem.instance.negativeRating = cloudTrophies.TrophiesNegative;
									RatingSystem.instance.positiveRating = cloudTrophies.TrophiesPositive;
									localDirty = true;
								}
								else if (cloudTrophies.Season == localSeason)
								{
									int newLocalTrophiesNegative3 = localTrophiesNegative;
									if (cloudTrophies.TrophiesNegative > localTrophiesNegative)
									{
										newLocalTrophiesNegative3 = cloudTrophies.TrophiesNegative;
										RatingSystem.instance.negativeRating = newLocalTrophiesNegative3;
										localDirty = true;
									}
									int newLocalTrophiesPositive2 = localTrophiesPositive;
									if (cloudTrophies.TrophiesPositive > localTrophiesPositive)
									{
										newLocalTrophiesPositive2 = cloudTrophies.TrophiesPositive;
										RatingSystem.instance.positiveRating = newLocalTrophiesPositive2;
										localDirty = true;
									}
								}
								EventHandler handler = this.Updated;
								if (localDirty && handler != null)
								{
									handler(this, EventArgs.Empty);
								}
								if (pullOnly || (!cloudTrophies.Conflicted && !cloudDirty))
								{
									break;
								}
								ScopeLogger scopeLogger2 = new ScopeLogger("TrophiesSynchronizer.PullGoogleCoroutine()", "PushGoogleCoroutine(conflict)", Defs.IsDeveloperBuild && !Application.isEditor);
								try
								{
									IEnumerator enumerator = PushGoogleCoroutine();
									while (enumerator.MoveNext())
									{
										yield return null;
									}
									break;
								}
								finally
								{
									scopeLogger2.Dispose();
								}
							}
							Debug.LogWarning("Failed to push trophies with status: " + requestStatus);
							yield return delay;
						}
					}
					i++;
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private IEnumerator PushGoogleCoroutine()
		{
			if (!Ready)
			{
				yield break;
			}
			ScopeLogger scopeLogger = new ScopeLogger("TrophiesSynchronizer.PushGoogleCoroutine()", Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				if (!Application.isEditor && GpgFacade.Instance.SavedGame == null)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("{0}: Waiting while `SavedGame == null`...", GetType().Name);
					}
					while (GpgFacade.Instance.SavedGame == null)
					{
						yield return null;
					}
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("{0}: `SavedGame != null`...", GetType().Name);
					}
				}
				TrophiesSynchronizerGoogleSavedGameFacade googleSavedGamesFacade = default(TrophiesSynchronizerGoogleSavedGameFacade);
				WaitForSeconds delay = new WaitForSeconds(30f);
				int i = 0;
				while (true)
				{
					int trophiesNegative = RatingSystem.instance.negativeRating;
					int trophiesPositive = RatingSystem.instance.positiveRating;
					int localSeason = FriendsController.sharedController.currentCompetition;
					TrophiesMemento localTrophies = new TrophiesMemento(trophiesNegative, trophiesPositive, localSeason);
					string callee = string.Format(CultureInfo.InvariantCulture, "Push and wait {0} ({1})", localTrophies, i);
					using (ScopeLogger logger = new ScopeLogger("TrophiesSynchronizer.PushGoogleCoroutine()", callee, Defs.IsDeveloperBuild && !Application.isEditor))
					{
						Task<GoogleSavedGameRequestResult<ISavedGameMetadata>> future = googleSavedGamesFacade.Push(localTrophies);
						while (!future.IsCompleted)
						{
							yield return null;
						}
						logger.Dispose();
						if (future.IsFaulted)
						{
							Exception ex = future.Exception.InnerExceptions.FirstOrDefault() ?? future.Exception;
							Debug.LogWarning("Failed to push trophies with exception: " + ex.Message);
							yield return delay;
						}
						else
						{
							SavedGameRequestStatus requestStatus = future.Result.RequestStatus;
							if (requestStatus == SavedGameRequestStatus.Success)
							{
								if (Defs.IsDeveloperBuild)
								{
									ISavedGameMetadata metadata = future.Result.Value;
									string description = ((metadata == null) ? string.Empty : metadata.Description);
									Debug.LogFormat("[Trophies] Succeeded to push trophies with status: '{0}'", description);
								}
								break;
							}
							Debug.LogWarning("Failed to push trophies with status: " + requestStatus);
							yield return delay;
						}
					}
					i++;
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}
	}
}
