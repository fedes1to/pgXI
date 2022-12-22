using UnityEngine;

public class GoMapInEndGame : MonoBehaviour
{
	public int mapIndex;

	public UITexture mapTexture;

	public UILabel mapLabel;

	private float enableTime;

	private bool _isLeavingRoom;

	public bool IsLeavingRoom
	{
		get
		{
			return _isLeavingRoom;
		}
		protected set
		{
			_isLeavingRoom = value;
		}
	}

	private void OnEnable()
	{
		enableTime = Time.time;
	}

	private void Start()
	{
		if (!Defs.isInet || Defs.isDaterRegim)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SetMap(SceneInfo scInfo)
	{
		if (scInfo == null)
		{
			mapIndex = -1;
			mapTexture.mainTexture = Resources.Load<Texture>("LevelLoadingsSmall/Random_Map");
			mapLabel.text = LocalizationStore.Get("Key_2463");
			return;
		}
		mapIndex = scInfo.indexMap;
		mapTexture.mainTexture = Resources.Load<Texture>("LevelLoadingsSmall/Loading_" + scInfo.NameScene);
		if (scInfo != null)
		{
			mapLabel.text = scInfo.TranslateName;
		}
	}

	public void OnClick()
	{
		if (Time.time - enableTime < 2f || (BankController.Instance != null && BankController.Instance.InterfaceEnabled) || (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown))
		{
			return;
		}
		Defs.typeDisconnectGame = Defs.DisconectGameType.SelectNewMap;
		if (mapIndex != -1)
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(mapIndex);
			Initializer.Instance.goMapName = infoScene.NameScene;
		}
		else
		{
			int randomMapIndex = ConnectSceneNGUIController.GetRandomMapIndex();
			if (randomMapIndex != -1)
			{
				SceneInfo infoScene2 = SceneInfoController.instance.GetInfoScene(randomMapIndex);
				Initializer.Instance.goMapName = infoScene2.NameScene;
			}
			else
			{
				Initializer.Instance.goMapName = string.Empty;
			}
		}
		GlobalGameController.countKillsRed = 0;
		GlobalGameController.countKillsBlue = 0;
		PhotonNetwork.LeaveRoom();
		IsLeavingRoom = true;
	}
}
