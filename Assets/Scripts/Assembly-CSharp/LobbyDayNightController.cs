using System;
using System.Collections;
using UnityEngine;

public class LobbyDayNightController : MonoBehaviour
{
	[Serializable]
	public class MaterialToChange
	{
		public string description = "description";

		public Color[] cicleColors = new Color[6];

		public Material[] materials;

		public float[] cicleLerp = new float[6];

		public bool changecolor;

		public bool changeLM;

		[HideInInspector]
		public Color currentColor;

		[HideInInspector]
		public float currentLerp;

		public GameObject[] objectsToOnAtNight;

		[HideInInspector]
		public bool objectsIsActive;
	}

	public MaterialToChange[] matToChange;

	private float cicleTime;

	private float timeDelta;

	private float dayLength = 86400f;

	private void Start()
	{
		DateTime now = DateTime.Now;
		timeDelta = dayLength - (float)(now.Hour * 60 * 60) + (float)(now.Minute * 60) + (float)now.Second;
		cicleTime = dayLength / 6f;
		StartCoroutine(MatColorChange());
	}

	private void Update()
	{
		timeDelta -= Time.deltaTime;
		if (timeDelta < 0f)
		{
			timeDelta = dayLength;
		}
		MaterialToChange[] array = matToChange;
		foreach (MaterialToChange materialToChange in array)
		{
			int num = Mathf.FloorToInt(timeDelta / dayLength * 6f);
			if (num == 6)
			{
				num = 0;
			}
			int num2 = num + 1;
			if (num2 > 5)
			{
				num2 = 0;
			}
			float t = (timeDelta - cicleTime * (float)num) / cicleTime;
			if (materialToChange.changecolor)
			{
				materialToChange.currentColor = Color.Lerp(materialToChange.cicleColors[num], materialToChange.cicleColors[num2], t);
			}
			if (materialToChange.changeLM)
			{
				materialToChange.currentLerp = Mathf.Lerp(materialToChange.cicleLerp[num], materialToChange.cicleLerp[num2], t);
			}
			if (materialToChange.objectsToOnAtNight != null)
			{
				if ((num == 5 || num == 0) && !materialToChange.objectsIsActive)
				{
					ActiveGo(materialToChange.objectsToOnAtNight, true);
					materialToChange.objectsIsActive = true;
				}
				if (num > 0 && num < 5 && materialToChange.objectsIsActive)
				{
					ActiveGo(materialToChange.objectsToOnAtNight, false);
					materialToChange.objectsIsActive = false;
				}
			}
		}
	}

	private void ActiveGo(GameObject[] go, bool active)
	{
		foreach (GameObject gameObject in go)
		{
			gameObject.SetActive(active);
		}
	}

	private IEnumerator MatColorChange()
	{
		while (true)
		{
			yield return null;
			MaterialToChange[] array = matToChange;
			foreach (MaterialToChange mTCh in array)
			{
				Material[] materials = mTCh.materials;
				foreach (Material mat in materials)
				{
					if (mTCh.changecolor)
					{
						mat.color = mTCh.currentColor;
					}
					if (mat.HasProperty("_Lerp"))
					{
						mat.SetFloat("_Lerp", mTCh.currentLerp);
					}
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}
