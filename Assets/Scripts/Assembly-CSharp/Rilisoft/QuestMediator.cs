using System;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class QuestMediator
	{
		private sealed class QuestEventSource : QuestEvents
		{
			internal new void RaiseWin(WinEventArgs e)
			{
				base.RaiseWin(e);
			}

			internal new void RaiseKillOtherPlayer(KillOtherPlayerEventArgs e)
			{
				base.RaiseKillOtherPlayer(e);
			}

			internal new void RaiseKillOtherPlayerWithFlag(EventArgs e)
			{
				base.RaiseKillOtherPlayerWithFlag(e);
			}

			internal new void RaiseCapture(CaptureEventArgs e)
			{
				base.RaiseCapture(e);
			}

			internal new void RaiseKillMonster(KillMonsterEventArgs e)
			{
				base.RaiseKillMonster(e);
			}

			internal new void RaiseBreakSeries(EventArgs e)
			{
				base.RaiseBreakSeries(e);
			}

			internal new void RaiseMakeSeries(EventArgs e)
			{
				base.RaiseMakeSeries(e);
			}

			internal new void RaiseSurviveWaveInArena(SurviveWaveInArenaEventArgs e)
			{
				base.RaiseSurviveWaveInArena(e);
			}

			internal new void RaiseGetGotcha(EventArgs e)
			{
				base.RaiseGetGotcha(e);
			}

			internal new void RaiseSocialInteraction(SocialInteractionEventArgs e)
			{
				base.RaiseSocialInteraction(e);
			}

			internal new void RaiseJump(EventArgs e)
			{
				base.RaiseJump(e);
			}

			internal new void RaiseTurretKill(EventArgs e)
			{
				base.RaiseTurretKill(e);
			}

			internal new void RaiseKillOtherPlayerOnFly(KillOtherPlayerOnFlyEventArgs e)
			{
				base.RaiseKillOtherPlayerOnFly(e);
			}
		}

		private static readonly QuestEventSource _eventSource = new QuestEventSource();

		public static QuestEvents Events
		{
			get
			{
				return _eventSource;
			}
		}

		public static void NotifyWin(ConnectSceneNGUIController.RegimGame mode, string map)
		{
			WinEventArgs winEventArgs = new WinEventArgs();
			winEventArgs.Mode = mode;
			winEventArgs.Map = map ?? string.Empty;
			WinEventArgs winEventArgs2 = winEventArgs;
			try
			{
				_eventSource.RaiseWin(winEventArgs2);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Caught exception in NotifyWin: {0}", winEventArgs2);
				Debug.LogException(exception);
			}
		}

		public static void NotifyKillOtherPlayer(ConnectSceneNGUIController.RegimGame mode, ShopNGUIController.CategoryNames weaponSlot, bool headshot = false, bool grenade = false, bool revenge = false, bool isInvisible = false, bool turretKill = false)
		{
			KillOtherPlayerEventArgs killOtherPlayerEventArgs = new KillOtherPlayerEventArgs();
			killOtherPlayerEventArgs.Mode = mode;
			killOtherPlayerEventArgs.WeaponSlot = weaponSlot;
			killOtherPlayerEventArgs.Headshot = headshot;
			killOtherPlayerEventArgs.Grenade = grenade;
			killOtherPlayerEventArgs.Revenge = revenge;
			killOtherPlayerEventArgs.IsInvisible = isInvisible;
			KillOtherPlayerEventArgs killOtherPlayerEventArgs2 = killOtherPlayerEventArgs;
			try
			{
				_eventSource.RaiseKillOtherPlayer(killOtherPlayerEventArgs2);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Caught exception in NotifyKillOtherPlayer: {0}", killOtherPlayerEventArgs2);
				Debug.LogException(exception);
			}
		}

		public static void NotifyKillOtherPlayerWithFlag()
		{
			try
			{
				_eventSource.RaiseKillOtherPlayerWithFlag(EventArgs.Empty);
			}
			catch (Exception exception)
			{
				Debug.LogError("Caught exception in NotifyKillOtherPlayerWithFlag.");
				Debug.LogException(exception);
			}
		}

		public static void NotifyCapture(ConnectSceneNGUIController.RegimGame mode)
		{
			CaptureEventArgs captureEventArgs = new CaptureEventArgs();
			captureEventArgs.Mode = mode;
			CaptureEventArgs captureEventArgs2 = captureEventArgs;
			try
			{
				_eventSource.RaiseCapture(captureEventArgs2);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Caught exception in NotifyCapture: {0}", captureEventArgs2);
				Debug.LogException(exception);
			}
		}

		public static void NotifyKillMonster(ShopNGUIController.CategoryNames weaponSlot, bool campaign = false)
		{
			KillMonsterEventArgs killMonsterEventArgs = new KillMonsterEventArgs();
			killMonsterEventArgs.WeaponSlot = weaponSlot;
			killMonsterEventArgs.Campaign = campaign;
			KillMonsterEventArgs killMonsterEventArgs2 = killMonsterEventArgs;
			try
			{
				_eventSource.RaiseKillMonster(killMonsterEventArgs2);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Caught exception in NotifyKillMonster: {0}", killMonsterEventArgs2);
				Debug.LogException(exception);
			}
		}

		public static void NotifyBreakSeries()
		{
			try
			{
				_eventSource.RaiseBreakSeries(EventArgs.Empty);
			}
			catch (Exception exception)
			{
				Debug.LogError("Caught exception in NotifyBreakSeries.");
				Debug.LogException(exception);
			}
		}

		public static void NotifyMakeSeries()
		{
			try
			{
				_eventSource.RaiseMakeSeries(EventArgs.Empty);
			}
			catch (Exception exception)
			{
				Debug.LogError("Caught exception in NotifyMakeSeries.");
				Debug.LogException(exception);
			}
		}

		public static void NotifySurviveWaveInArena(int currentWave)
		{
			SurviveWaveInArenaEventArgs surviveWaveInArenaEventArgs = new SurviveWaveInArenaEventArgs();
			surviveWaveInArenaEventArgs.WaveNumber = currentWave;
			SurviveWaveInArenaEventArgs surviveWaveInArenaEventArgs2 = surviveWaveInArenaEventArgs;
			try
			{
				_eventSource.RaiseSurviveWaveInArena(surviveWaveInArenaEventArgs2);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Caught exception in NotifySurviveWaveInArena: {0}", surviveWaveInArenaEventArgs2);
				Debug.LogException(exception);
			}
		}

		public static void NotifyGetGotcha()
		{
			try
			{
				_eventSource.RaiseGetGotcha(EventArgs.Empty);
			}
			catch (Exception exception)
			{
				Debug.LogError("Caught exception in NotifyGetGotcha.");
				Debug.LogException(exception);
			}
		}

		public static void NotifySocialInteraction(string kind)
		{
			SocialInteractionEventArgs socialInteractionEventArgs = new SocialInteractionEventArgs();
			socialInteractionEventArgs.Kind = kind ?? string.Empty;
			SocialInteractionEventArgs socialInteractionEventArgs2 = socialInteractionEventArgs;
			try
			{
				_eventSource.RaiseSocialInteraction(socialInteractionEventArgs2);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Caught exception in NotifySocialInteraction: {0}", socialInteractionEventArgs2);
				Debug.LogException(exception);
			}
		}

		public static void NotifyJump()
		{
			try
			{
				_eventSource.RaiseJump(EventArgs.Empty);
			}
			catch (Exception exception)
			{
				Debug.LogError("Caught exception in NotifyJump");
				Debug.LogException(exception);
			}
		}

		public static void NotifyTurretKill()
		{
			try
			{
				_eventSource.RaiseTurretKill(EventArgs.Empty);
			}
			catch (Exception exception)
			{
				Debug.LogError("Caught exception in NotifyTurretKill");
				Debug.LogException(exception);
			}
		}

		public static void NotifyKillOtherPlayerOnFly(bool iamFly, bool killedFly)
		{
			KillOtherPlayerOnFlyEventArgs killOtherPlayerOnFlyEventArgs = new KillOtherPlayerOnFlyEventArgs();
			killOtherPlayerOnFlyEventArgs.IamFly = iamFly;
			killOtherPlayerOnFlyEventArgs.KilledPlayerFly = killedFly;
			KillOtherPlayerOnFlyEventArgs killOtherPlayerOnFlyEventArgs2 = killOtherPlayerOnFlyEventArgs;
			try
			{
				_eventSource.RaiseKillOtherPlayerOnFly(killOtherPlayerOnFlyEventArgs2);
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Caught exception in NotifyKillOtherPlayerOnFly: {0}", killOtherPlayerOnFlyEventArgs2);
				Debug.LogException(exception);
			}
		}
	}
}
