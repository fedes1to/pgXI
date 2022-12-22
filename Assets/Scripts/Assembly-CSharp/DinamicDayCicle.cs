using System.Collections;
using UnityEngine;

public class DinamicDayCicle : MonoBehaviour
{
	public MaterialToChange[] matToChange;

	public float lerpFactor;

	private int nextCicle;

	private float cicleTime;

	public int currentCicle;

	private float matchTime;

	private float timeDelta;

	private void Start()
	{
		ResetColors();
		StartCoroutine(MatColorChange());
	}

	private void Update()
	{
		if (TimeGameController.sharedController != null && PhotonNetwork.room != null && !string.IsNullOrEmpty(ConnectSceneNGUIController.maxKillProperty))
		{
			if (!PhotonNetwork.room.customProperties.ContainsKey(ConnectSceneNGUIController.maxKillProperty))
			{
				return;
			}
			int result = -1;
			int.TryParse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty].ToString(), out result);
			if (result < 0)
			{
				ResetColors();
				return;
			}
			matchTime = (float)result * 60f;
			if (!((float)TimeGameController.sharedController.timerToEndMatch < matchTime))
			{
				return;
			}
			timeDelta = matchTime - (float)TimeGameController.sharedController.timerToEndMatch;
			if (matchTime == timeDelta)
			{
				return;
			}
			MaterialToChange[] array = matToChange;
			foreach (MaterialToChange materialToChange in array)
			{
				cicleTime = matchTime / (float)materialToChange.cicleColors.Length;
				currentCicle = Mathf.FloorToInt(timeDelta / matchTime * (float)materialToChange.cicleColors.Length);
				nextCicle = Mathf.Min(currentCicle + 1, materialToChange.cicleColors.Length - 1);
				lerpFactor = (timeDelta - cicleTime * (float)currentCicle) / cicleTime;
				if (materialToChange.changecolor && currentCicle < materialToChange.cicleColors.Length)
				{
					materialToChange.currentColor = Color.Lerp(materialToChange.cicleColors[currentCicle], materialToChange.cicleColors[nextCicle], lerpFactor);
				}
				if (materialToChange.cicleLerp != null && materialToChange.cicleLerp.Length == materialToChange.cicleColors.Length && currentCicle < materialToChange.cicleColors.Length)
				{
					materialToChange.currentLerp = Mathf.Lerp(materialToChange.cicleLerp[currentCicle], materialToChange.cicleLerp[nextCicle], lerpFactor);
				}
			}
		}
		else
		{
			ResetColors();
		}
	}

	private void ResetColors()
	{
		MaterialToChange[] array = matToChange;
		foreach (MaterialToChange materialToChange in array)
		{
			if (materialToChange.changecolor)
			{
				materialToChange.currentColor = materialToChange.cicleColors[0];
			}
			if (materialToChange.cicleLerp != null && materialToChange.cicleLerp.Length == materialToChange.cicleColors.Length)
			{
				materialToChange.currentLerp = materialToChange.cicleLerp[0];
			}
		}
	}

	private IEnumerator MatColorChange()
	{
		while (true)
		{
			if (matchTime != timeDelta)
			{
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
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}
