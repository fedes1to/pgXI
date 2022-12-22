using UnityEngine;

internal sealed class ABTestInfoInConsol : MonoBehaviour
{
	private void Update()
	{
		UILabel component = GetComponent<UILabel>();
		string text = "Текущие кагорты: ";
		if (Defs.abTestBalansCohort != 0 && Defs.abTestBalansCohort != Defs.ABTestCohortsType.SKIP)
		{
			text = text + Defs.abTestBalansCohortName + " ";
		}
		if (Defs.cohortABTestAdvert != 0 && Defs.cohortABTestAdvert != Defs.ABTestCohortsType.SKIP)
		{
			text = text + FriendsController.configNameABTestAdvert + " ";
		}
		foreach (ABTestBase currentABTest in ABTestController.currentABTests)
		{
			if (currentABTest.cohort != 0 && currentABTest.cohort != ABTestController.ABTestCohortsType.SKIP)
			{
				text = text + currentABTest.cohortName + " ";
			}
		}
		component.text = text;
	}
}
