using UnityEngine;

public sealed class CampaignLevel
{
	public float timeToComplete = 300f;

	private Vector3 _localPosition = Vector3.forward;

	private string _sceneName;

	private string _localizeKeyForLevelMap;

	public string sceneName
	{
		get
		{
			return _sceneName;
		}
		set
		{
			_sceneName = value;
		}
	}

	public string localizeKeyForLevelMap
	{
		get
		{
			return _localizeKeyForLevelMap;
		}
		set
		{
			_localizeKeyForLevelMap = value;
		}
	}

	public string predlog { get; set; }

	public Vector3 LocalPosition
	{
		get
		{
			return _localPosition;
		}
		set
		{
			_localPosition = value;
		}
	}

	public CampaignLevel(string sceneName, string keyForLevelMap, string pr = "in")
	{
		_sceneName = sceneName;
		_localizeKeyForLevelMap = keyForLevelMap;
		predlog = pr;
	}

	public CampaignLevel()
	{
		_sceneName = string.Empty;
	}
}
