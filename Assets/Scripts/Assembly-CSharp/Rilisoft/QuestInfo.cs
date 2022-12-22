using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public sealed class QuestInfo
	{
		private readonly bool _forcedSkip;

		private readonly IList<QuestBase> _quests;

		private readonly Func<IList<QuestBase>> _skipMethod;

		public QuestBase Quest
		{
			get
			{
				return _quests.FirstOrDefault();
			}
		}

		public bool CanSkip
		{
			get
			{
				if (_skipMethod == null)
				{
					return false;
				}
				if (_quests.Count == 0)
				{
					return _forcedSkip;
				}
				if (_quests[0].Rewarded)
				{
					return false;
				}
				if (_quests[0].CalculateProgress() >= 1m)
				{
					return false;
				}
				if (_quests.Count < 2)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("_quests.Count < 2: {0}", _quests.Count);
					}
					return _forcedSkip;
				}
				return true;
			}
		}

		internal QuestInfo(IEnumerable<QuestBase> quests, Func<IList<QuestBase>> skipMethod, bool forcedSkip = false)
		{
			if (quests == null)
			{
				throw new ArgumentNullException("quests");
			}
			_forcedSkip = forcedSkip;
			_quests = quests.ToList();
			_skipMethod = skipMethod;
		}

		public void Skip()
		{
			if (!CanSkip)
			{
				return;
			}
			IList<QuestBase> list = _skipMethod();
			_quests.Clear();
			if (list == null)
			{
				return;
			}
			foreach (QuestBase item in list)
			{
				_quests.Add(item);
			}
		}
	}
}
