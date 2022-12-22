using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABTestController : MonoBehaviour
{
	public enum ABTestCohortsType
	{
		NONE,
		A,
		B,
		SKIP
	}

	public static List<ABTestBase> currentABTests = new List<ABTestBase>();

	public static bool useBuffSystem;

	public bool isRunABTest
	{
		get
		{
			return true;
		}
	}

	private void Start()
	{
		currentABTests.Add(new ABTestQuestSystem());
		if (TrainingController.TrainingCompleted)
		{
			SkipAllNotStartedTests();
		}
		InitAllABTests();
		StartCoroutine(UpdateConfigsAllABTests());
	}

	private void SkipAllNotStartedTests()
	{
		foreach (ABTestBase currentABTest in currentABTests)
		{
			if (currentABTest.cohort == ABTestCohortsType.NONE)
			{
				currentABTest.cohort = ABTestCohortsType.SKIP;
			}
		}
	}

	private void InitAllABTests()
	{
		foreach (ABTestBase currentABTest in currentABTests)
		{
			currentABTest.InitTest();
		}
	}

	private IEnumerator UpdateConfigsAllABTests()
	{
		foreach (ABTestBase abtest in currentABTests)
		{
			abtest.UpdateABTestConfig();
			yield return null;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		StartCoroutine(UpdateConfigsAllABTests());
	}
}
