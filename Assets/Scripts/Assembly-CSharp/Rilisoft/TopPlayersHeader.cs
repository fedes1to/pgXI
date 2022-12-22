using UnityEngine;

namespace Rilisoft
{
	public class TopPlayersHeader : MonoBehaviour
	{
		[SerializeField]
		private UILabel _label;

		private int _startLevel = -1;

		private int _endLevel = -1;

		private void OnEnable()
		{
			if (_label == null)
			{
				return;
			}
			int ourTier = ExpController.GetOurTier();
			if (ExpController.LevelsForTiers.Length - 1 >= ourTier)
			{
				_startLevel = ExpController.LevelsForTiers[ourTier];
				if (ExpController.LevelsForTiers.Length - 1 >= ourTier + 1)
				{
					_endLevel = ExpController.LevelsForTiers[ourTier + 1] - 1;
					_label.text = string.Format("{0}: {1} - {2}", LocalizationStore.Get("Key_2835"), _startLevel, _endLevel);
				}
				else
				{
					_label.text = string.Format("{0}: {1} + ", LocalizationStore.Get("Key_2835"), _startLevel);
				}
			}
		}
	}
}
