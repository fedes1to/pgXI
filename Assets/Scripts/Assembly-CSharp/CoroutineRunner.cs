using System;
using System.Collections;
using System.Threading.Tasks;
using Rilisoft;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
	private static CoroutineRunner _instance;

	public static CoroutineRunner Instance
	{
		get
		{
			if (_instance == null)
			{
				try
				{
					GameObject gameObject = new GameObject("CoroutineRunner");
					_instance = gameObject.AddComponent<CoroutineRunner>();
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
				catch (Exception ex)
				{
					Debug.LogError("[Rilisoft] CoroutineRunner: Instance exception: " + ex);
				}
			}
			return _instance;
		}
	}

	internal IEnumerator WrapCoroutine(IEnumerator routine, TaskCompletionSource<bool> promise)
	{
		if (routine == null)
		{
			throw new ArgumentNullException("routine");
		}
		if (promise == null)
		{
			throw new ArgumentNullException("promise");
		}
		return WrapCoroutineCore(routine, promise);
	}

	private IEnumerator WrapCoroutineCore(IEnumerator routine, TaskCompletionSource<bool> promise)
	{
		while (routine.MoveNext())
		{
			yield return routine.Current;
		}
		promise.SetResult(true);
	}

	public static IEnumerator WaitForSeconds(float tm)
	{
		float startTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}

	public static IEnumerator WaitForSecondsActionEveryNFrames(float tm, Action action, int everyNFrames)
	{
		float startTime = Time.realtimeSinceStartup;
		int i = 0;
		do
		{
			yield return null;
			i++;
			if (i % everyNFrames == 0 && action != null)
			{
				action();
			}
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}

	public static void WaitUntil(Func<bool> func, Action onActiveAction)
	{
		Instance.StartCoroutine(Instance.WaitUntilCoroutine(func, onActiveAction));
	}

	private IEnumerator WaitUntilCoroutine(Func<bool> func, Action onActiveAction)
	{
		yield return new WaitUntil(func);
		if (onActiveAction != null)
		{
			onActiveAction();
		}
	}

	public static void DeferredAction(float runAfterSecs, Action act)
	{
		Instance.StartCoroutine(Instance.DeferredActionCoroutine(runAfterSecs, act));
	}

	private IEnumerator DeferredActionCoroutine(float runAfterSecs, Action act)
	{
		yield return new WaitForRealSeconds(runAfterSecs);
		if (act != null)
		{
			act();
		}
	}
}
