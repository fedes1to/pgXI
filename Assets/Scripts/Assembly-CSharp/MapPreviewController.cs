using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public sealed class MapPreviewController : MonoBehaviour
{
	private readonly string[] _ratingLabelsKeys = new string[6] { "Key_0545", "Key_0546", "Key_0547", "Key_0548", "Key_0549", "Key_2183" };

	public UILabel NameMapLbl;

	public GameObject[] SizeMapNameLbl;

	public UILabel popularityLabel;

	public UISprite popularitySprite;

	public GameObject premium;

	public GameObject milee;

	public GameObject dater;

	public int mapID;

	public string sceneMapName;

	public UITexture mapPreviewTexture;

	public GameObject bottomPanel;

	private MyCenterOnChild centerChild;

	[ReadOnly]
	[SerializeField]
	private int _ratingVal;

	private int _rating
	{
		get
		{
			return _ratingVal;
		}
		set
		{
			_ratingVal = value;
			if (_ratingVal >= 0)
			{
				popularitySprite.spriteName = string.Format("Nb_Players_{0}", _ratingVal);
			}
		}
	}

	private void Start()
	{
	}

	public void UpdatePopularity()
	{
		StopCoroutine(SetPopularity());
		StartCoroutine(SetPopularity());
	}

	private IEnumerator SetPopularity()
	{
		Lazy<HashSet<ConnectSceneNGUIController.RegimGame>> loggedFailedModes = new Lazy<HashSet<ConnectSceneNGUIController.RegimGame>>(() => new HashSet<ConnectSceneNGUIController.RegimGame>());
		Dictionary<string, string> _mapsPoplarityInCurrentRegim;
		while (true)
		{
			if (FriendsController.mapPopularityDictionary.Count > 0)
			{
				ConnectSceneNGUIController.RegimGame mode = ConnectSceneNGUIController.regim;
				Dictionary<string, Dictionary<string, string>> mapPopularityDictionary = FriendsController.mapPopularityDictionary;
				int num = (int)mode;
				if (!mapPopularityDictionary.TryGetValue(num.ToString(), out _mapsPoplarityInCurrentRegim) && !loggedFailedModes.Value.Contains(mode))
				{
					Debug.LogWarningFormat("Cannot find given key in map popularity dictionary: {0} ({1})", (int)mode, mode);
					loggedFailedModes.Value.Add(mode);
				}
				if (_mapsPoplarityInCurrentRegim != null)
				{
					break;
				}
				_rating = 0;
				yield return StartCoroutine(MyWaitForSeconds(2f));
			}
			else
			{
				yield return StartCoroutine(MyWaitForSeconds(2f));
			}
		}
		int _countPlayersOnMap = (_mapsPoplarityInCurrentRegim.ContainsKey(mapID.ToString()) ? int.Parse(_mapsPoplarityInCurrentRegim[mapID.ToString()]) : 0);
		if (_countPlayersOnMap < 1)
		{
			_rating = 0;
		}
		else if (_countPlayersOnMap >= 1 && _countPlayersOnMap < 8)
		{
			_rating = 1;
		}
		else if (_countPlayersOnMap >= 8 && _countPlayersOnMap < 15)
		{
			_rating = 2;
		}
		else if (_countPlayersOnMap >= 15 && _countPlayersOnMap < 35)
		{
			_rating = 3;
		}
		else if (_countPlayersOnMap >= 35 && _countPlayersOnMap < 50)
		{
			_rating = 4;
		}
		else if (_countPlayersOnMap >= 50)
		{
			_rating = 5;
		}
	}

	public IEnumerator MyWaitForSeconds(float tm)
	{
		float startTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}

	private void OnClick()
	{
		if (ConnectSceneNGUIController.sharedController.selectMap.Equals(this))
		{
			if (!ConnectSceneNGUIController.sharedController.createPanel.activeSelf)
			{
				ConnectSceneNGUIController.sharedController.HandleGoBtnClicked(null, EventArgs.Empty);
			}
		}
		else
		{
			ConnectSceneNGUIController.sharedController.selectMap = this;
			ConnectSceneNGUIController.sharedController.StopFingerAnimation();
		}
	}
}
