using UnityEngine;

namespace Rilisoft
{
	public class NestClickHandler : MonoBehaviour
	{
		private bool hintShowed;

		private void OnClick()
		{
			if (Nest.Instance != null)
			{
				Nest.Instance.Click();
				HideAndSaveHint();
			}
		}

		private void Start()
		{
			if (HintController.instance != null && ExperienceController.sharedController.currentLevel >= 2 && Nest.Instance.NestCanShow() && PlayerPrefs.GetInt("NestHintShowed", 0) == 0)
			{
				hintShowed = true;
				HintController.instance.ShowHintByName("incubator", 0f);
				HintController.instance.ShowHintByName("incubator_2", 0f);
			}
		}

		private void OnDisable()
		{
			HideAndSaveHint();
		}

		private void HideAndSaveHint()
		{
			if (hintShowed && HintController.instance != null)
			{
				hintShowed = false;
				HintController.instance.HideHintByName("incubator");
				HintController.instance.HideHintByName("incubator_2");
				PlayerPrefs.SetInt("NestHintShowed", 1);
			}
		}
	}
}
