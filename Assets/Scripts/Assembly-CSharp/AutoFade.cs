using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

public class AutoFade : MonoBehaviour
{
	private static AutoFade m_Instance;

	private Material m_Material;

	private string m_LevelName = string.Empty;

	private bool m_Fading;

	private bool isLoadScene = true;

	private float killedTime;

	private static AutoFade Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new GameObject("AutoFade").AddComponent<AutoFade>();
			}
			return m_Instance;
		}
	}

	public static bool Fading
	{
		get
		{
			return Instance.m_Fading;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		m_Instance = this;
		Shader shader = Shader.Find("Mobile/Particles/Alpha Blended");
		m_Material = new Material(shader);
	}

	private void DrawQuad(Color aColor, float aAlpha)
	{
		if (!ShopNGUIController.GuiActive && !BankController.Instance.uiRoot.gameObject.activeInHierarchy)
		{
			aColor.a = aAlpha;
			if (m_Material.SetPass(0))
			{
				GL.PushMatrix();
				GL.LoadOrtho();
				GL.Begin(7);
				GL.Color(aColor);
				GL.Vertex3(0f, 0f, -1f);
				GL.Vertex3(0f, 1f, -1f);
				GL.Vertex3(1f, 1f, -1f);
				GL.Vertex3(1f, 0f, -1f);
				GL.End();
				GL.PopMatrix();
			}
			else
			{
				Debug.LogWarning("Couldnot set pass for material.");
			}
		}
	}

	private IEnumerator Fade(float aFadeOutTime, float aFadeInTime, Color aColor, bool collectGrabage)
	{
		float t = 0f;
		while (t < 1f)
		{
			yield return new WaitForEndOfFrame();
			t = Mathf.Clamp01(t + Time.deltaTime / aFadeOutTime);
			DrawQuad(aColor, t);
		}
		if (collectGrabage)
		{
			GC.Collect();
		}
		if (isLoadScene)
		{
			if (m_LevelName != string.Empty)
			{
				Singleton<SceneLoader>.Instance.LoadScene(m_LevelName);
			}
		}
		else
		{
			while (killedTime > 0f)
			{
				killedTime -= Time.deltaTime;
				DrawQuad(aColor, t);
				yield return new WaitForEndOfFrame();
			}
		}
		while (t > 0f && !(Mathf.Abs(aFadeInTime) < 1E-06f))
		{
			t = Mathf.Clamp01(t - Time.deltaTime / aFadeInTime);
			DrawQuad(aColor, t);
			yield return new WaitForEndOfFrame();
		}
		m_Fading = false;
	}

	private void StartFade(float aFadeOutTime, float aFadeInTime, Color aColor, bool collectGarbage = false)
	{
		m_Fading = true;
		StartCoroutine(Fade(aFadeOutTime, aFadeInTime, aColor, collectGarbage));
	}

	public static void LoadLevel(string aLevelName, float aFadeOutTime, float aFadeInTime, Color aColor)
	{
		if (!Fading)
		{
			Instance.isLoadScene = true;
			Instance.m_LevelName = aLevelName;
			Instance.StartFade(aFadeOutTime, aFadeInTime, aColor);
		}
	}

	public static void fadeKilled(float aFadeOutTime, float aFadeKilledTime, float aFadeInTime, Color aColor)
	{
		if (!Fading)
		{
			Instance.isLoadScene = false;
			Instance.killedTime = aFadeKilledTime;
			Instance.StartFade(aFadeOutTime, aFadeInTime, aColor, true);
		}
	}
}
