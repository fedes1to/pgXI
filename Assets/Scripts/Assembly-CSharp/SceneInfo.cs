using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public sealed class SceneInfo : MonoBehaviour
{
	[Header("Parametr map")]
	public int indexMap;

	public ModeWeapon AvaliableWeapon;

	public List<TypeModeGame> avaliableInModes = new List<TypeModeGame>();

	public string minAvaliableVersion = "0.0.0.0";

	public string maxAvaliableVersion = "0.0.0.0";

	public bool isPremium;

	public bool isPreloading;

	private bool _isLoaded;

	public InfoSizeMap sizeMap;

	[Header("Number key for translate")]
	public string keyTranslateName = string.Empty;

	private string transName = string.Empty;

	public string keyTranslateShortName = string.Empty;

	private string transShortName = string.Empty;

	private string transEngShortName = string.Empty;

	private string transSizeMap = string.Empty;

	[Header("Camera on start")]
	public Vector3 positionCam;

	public Vector3 rotationCam;

	public bool IsLoaded
	{
		get
		{
			if (_isLoaded)
			{
				return true;
			}
			UpdateKeyLoaded();
			return _isLoaded;
		}
	}

	public string NameScene
	{
		get
		{
			return base.gameObject.name;
		}
	}

	public bool IsAvaliableVersion
	{
		get
		{
			return true;
		}
	}

	public Sounds GetBackgroundSound
	{
		get
		{
			return null;
		}
	}

	public string TranslateName
	{
		get
		{
			return transName;
		}
	}

	public string TranslatePreviewName
	{
		get
		{
			return transShortName;
		}
	}

	public string TranslateEngShortName
	{
		get
		{
			return transShortName;
		}
	}

	public string KeyTranslateSizeMap
	{
		get
		{
			switch (sizeMap)
			{
			case InfoSizeMap.big:
				return "Key_0538";
			case InfoSizeMap.normal:
				return "Key_0539";
			case InfoSizeMap.veryBig:
				return "Key_0540";
			case InfoSizeMap.small:
				return "Key_0541";
			default:
				return string.Empty;
			}
		}
	}

	public string TranslateSizeMap
	{
		get
		{
			return transSizeMap;
		}
	}

	public void UpdateKeyLoaded()
	{
		if (isPreloading)
		{
			_isLoaded = false;
		}
		else
		{
			_isLoaded = true;
		}
	}

	public bool IsAvaliableForMode(TypeModeGame curMode)
	{
		if (IsAvaliableVersion && avaliableInModes != null && avaliableInModes.Count > 0)
		{
			for (int i = 0; i < avaliableInModes.Count; i++)
			{
				if (curMode == avaliableInModes[i])
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddMode(TypeModeGame curMode)
	{
		for (int i = 0; i < avaliableInModes.Count; i++)
		{
			if (curMode == avaliableInModes[i])
			{
				return;
			}
		}
		avaliableInModes.Add(curMode);
	}

	public void UpdateLocalize()
	{
		transName = LocalizationStore.Get(keyTranslateName);
		transShortName = LocalizationStore.Get(keyTranslateShortName);
		transSizeMap = LocalizationStore.Get(KeyTranslateSizeMap);
		transEngShortName = LocalizationStore.GetByDefault(keyTranslateShortName);
	}

	public void SetStartPositionCamera(GameObject curCamObj)
	{
		if (curCamObj != null)
		{
			curCamObj.transform.position = positionCam;
			curCamObj.transform.eulerAngles = rotationCam;
		}
	}

	[ContextMenu("Set Next Index")]
	private void DoSomething()
	{
		int num = 0;
		Object[] array = Resources.LoadAll("SceneInfo");
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			SceneInfo component = (@object as GameObject).GetComponent<SceneInfo>();
			if (component.indexMap > num)
			{
				num = component.indexMap;
			}
		}
		indexMap = num + 1;
	}
}
