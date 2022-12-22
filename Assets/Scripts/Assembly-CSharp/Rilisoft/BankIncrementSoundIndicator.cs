using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	public class BankIncrementSoundIndicator : MonoBehaviour
	{
		public float PlayDelay = 0.1f;

		public AudioClip ClipCoinAdded;

		public AudioClip ClipCoinsAdded;

		public AudioClip ClipGemAdded;

		public AudioClip ClipGemsAdded;

		private void OnEnable()
		{
			CoinsMessage.CoinsLabelDisappeared += OnCurrencyGetted;
		}

		private void OnDisable()
		{
			CoinsMessage.CoinsLabelDisappeared -= OnCurrencyGetted;
		}

		private void OnCurrencyGetted(bool isGems, int count)
		{
			float delay = ((Defs.isMulti || Defs.IsSurvival || !TrainingController.TrainingCompleted) ? PlayDelay : 0f);
			StartCoroutine(PlaySounds(isGems, count < 2, delay));
		}

		private IEnumerator PlaySounds(bool isGems, bool oneCoin, float delay)
		{
			if (!SceneLoader.ActiveSceneName.Equals("LevelComplete") && Defs.isSoundFX)
			{
				yield return new WaitForRealSeconds(delay);
				AudioClip clip2 = null;
				clip2 = ((!isGems) ? ((!oneCoin || !(ClipCoinAdded != null)) ? ClipCoinsAdded : ClipCoinAdded) : ((!oneCoin || !(ClipCoinAdded != null)) ? ClipGemsAdded : ClipGemAdded));
				NGUITools.PlaySound(clip2);
			}
		}
	}
}
