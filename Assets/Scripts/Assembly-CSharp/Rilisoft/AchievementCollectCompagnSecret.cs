using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class AchievementCollectCompagnSecret : Achievement
	{
		internal sealed class SecretGetttedInfo
		{
			public string Map;

			public bool CoinsGetted;

			public bool GemsGetted;

			public SecretGetttedInfo(string map)
			{
				Map = map;
			}

			public override string ToString()
			{
				return string.Format("'{0}'=> c:{1} g:{2}", Map, CoinsGetted, GemsGetted);
			}
		}

		private readonly List<SecretGetttedInfo> _collectedList = new List<SecretGetttedInfo>();

		public AchievementCollectCompagnSecret(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (base.IsCompleted)
			{
				return;
			}
			int count = LevelBox.campaignBoxes.Count;
			for (int i = 0; i != count; i++)
			{
				LevelBox levelBox = LevelBox.campaignBoxes[i];
				int count2 = levelBox.levels.Count;
				for (int j = 0; j != count2; j++)
				{
					CampaignLevel campaignLevel = levelBox.levels[j];
					_collectedList.Add(new SecretGetttedInfo(campaignLevel.sceneName));
				}
			}
			Refresh();
			Storager.SubscribeToChanged(Defs.LevelsWhereGetCoinS, Update);
			Storager.SubscribeToChanged(Defs.LevelsWhereGotGems, Update);
		}

		private void Refresh()
		{
			HashSet<string> hashSet = new HashSet<string>(CoinBonus.GetLevelsWhereGotCoins());
			HashSet<string> hashSet2 = new HashSet<string>(CoinBonus.GetLevelsWhereGotGems());
			if (Debug.isDebugBuild)
			{
			}
			foreach (SecretGetttedInfo collected in _collectedList)
			{
				collected.CoinsGetted = hashSet.Contains(collected.Map);
				collected.GemsGetted = hashSet2.Contains(collected.Map);
			}
			if (Debug.isDebugBuild)
			{
			}
			if (_collectedList.All((SecretGetttedInfo si) => si.CoinsGetted && si.GemsGetted))
			{
				Gain(1);
			}
		}

		private void Update()
		{
			string[] levelsWhereGotCoins = CoinBonus.GetLevelsWhereGotCoins();
			IEnumerable<string> levelsWhereGotGems = CoinBonus.GetLevelsWhereGotGems();
			foreach (SecretGetttedInfo collected in _collectedList)
			{
				collected.CoinsGetted = levelsWhereGotCoins.Contains(collected.Map);
				collected.GemsGetted = levelsWhereGotGems.Contains(collected.Map);
			}
			if (_collectedList.All((SecretGetttedInfo si) => si.CoinsGetted && si.GemsGetted))
			{
				Gain(1);
			}
		}

		public override void Dispose()
		{
			Storager.UnSubscribeToChanged(Defs.LevelsWhereGotGems, Update);
			Storager.UnSubscribeToChanged(Defs.LevelsWhereGetCoinS, Update);
		}
	}
}
