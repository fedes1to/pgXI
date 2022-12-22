using UnityEngine;

public class ExpHUD : MonoBehaviour
{
	public UILabel lbCurLev;

	public UILabel lbExp;

	public UITexture txExp;

	private void OnEnable()
	{
		ExpController.Instance.experienceView.VisibleHUD = false;
		UpdateHUD();
	}

	private void OnDisable()
	{
		if (ExpController.Instance == null)
		{
			Debug.LogWarning("ExpController.Instance == null");
		}
		else if (ExpController.Instance.experienceView == null)
		{
			Debug.LogWarning("experienceView == null");
		}
		else
		{
			ExpController.Instance.experienceView.VisibleHUD = true;
		}
	}

	public void UpdateHUD()
	{
		lbCurLev.text = ExperienceController.sharedController.currentLevel.ToString();
		lbExp.text = ExpController.ExpToString();
		if (ExperienceController.sharedController.currentLevel == 31)
		{
			txExp.fillAmount = 1f;
		}
		else
		{
			txExp.fillAmount = ExpController.progressExpInPer();
		}
	}
}
