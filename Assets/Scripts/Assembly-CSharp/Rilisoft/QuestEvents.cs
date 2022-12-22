using System;
using Rilisoft.NullExtensions;

namespace Rilisoft
{
	public class QuestEvents
	{
		public event EventHandler<WinEventArgs> Win;

		public event EventHandler<KillOtherPlayerEventArgs> KillOtherPlayer;

		public event EventHandler KillOtherPlayerWithFlag;

		public event EventHandler<CaptureEventArgs> Capture;

		public event EventHandler<KillMonsterEventArgs> KillMonster;

		public event EventHandler BreakSeries;

		public event EventHandler MakeSeries;

		public event EventHandler<SurviveWaveInArenaEventArgs> SurviveWaveInArena;

		public event EventHandler GetGotcha;

		public event EventHandler<SocialInteractionEventArgs> SocialInteraction;

		public event EventHandler Jump;

		public event EventHandler TurretKill;

		public event EventHandler<KillOtherPlayerOnFlyEventArgs> KillOtherPlayerOnFly;

		protected void RaiseWin(WinEventArgs e)
		{
			this.Win.Do(delegate(EventHandler<WinEventArgs> handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseKillOtherPlayer(KillOtherPlayerEventArgs e)
		{
			this.KillOtherPlayer.Do(delegate(EventHandler<KillOtherPlayerEventArgs> handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseKillOtherPlayerWithFlag(EventArgs e)
		{
			this.KillOtherPlayerWithFlag.Do(delegate(EventHandler handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseCapture(CaptureEventArgs e)
		{
			this.Capture.Do(delegate(EventHandler<CaptureEventArgs> handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseKillMonster(KillMonsterEventArgs e)
		{
			this.KillMonster.Do(delegate(EventHandler<KillMonsterEventArgs> handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseBreakSeries(EventArgs e)
		{
			this.BreakSeries.Do(delegate(EventHandler handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseMakeSeries(EventArgs e)
		{
			this.MakeSeries.Do(delegate(EventHandler handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseSurviveWaveInArena(SurviveWaveInArenaEventArgs e)
		{
			this.SurviveWaveInArena.Do(delegate(EventHandler<SurviveWaveInArenaEventArgs> handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseGetGotcha(EventArgs e)
		{
			this.GetGotcha.Do(delegate(EventHandler handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseSocialInteraction(SocialInteractionEventArgs e)
		{
			this.SocialInteraction.Do(delegate(EventHandler<SocialInteractionEventArgs> handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseJump(EventArgs e)
		{
			this.Jump.Do(delegate(EventHandler handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseTurretKill(EventArgs e)
		{
			this.TurretKill.Do(delegate(EventHandler handler)
			{
				handler(this, e);
			});
		}

		protected void RaiseKillOtherPlayerOnFly(KillOtherPlayerOnFlyEventArgs e)
		{
			this.KillOtherPlayerOnFly.Do(delegate(EventHandler<KillOtherPlayerOnFlyEventArgs> handler)
			{
				handler(this, e);
			});
		}
	}
}
