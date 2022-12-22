using System;

namespace Rilisoft
{
	public sealed class QuestCompletedEventArgs : EventArgs
	{
		public QuestBase Quest { get; set; }
	}
}
