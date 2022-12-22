using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
internal sealed class KillsLabel : MonoBehaviour
{
	private UILabel _label;

	private InGameGUI _inGameGUI;

	private KeyValuePair<int, string> _killCountMemo = new KeyValuePair<int, string>(0, "0");

	private void Start()
	{
		base.gameObject.SetActive(Defs.isMulti && (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.Duel || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.Deathmatch || Defs.isDaterRegim));
		_label = GetComponent<UILabel>();
		_inGameGUI = InGameGUI.sharedInGameGUI;
	}

	private void Update()
	{
		if ((bool)_inGameGUI && (bool)_label)
		{
			if (Defs.isDaterRegim || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.Duel)
			{
				_label.text = GetKillCountString(GlobalGameController.CountKills);
			}
			else if (_inGameGUI != null)
			{
				_label.text = _inGameGUI.killsToMaxKills();
			}
		}
	}

	private string GetKillCountString(int killCount)
	{
		if (killCount != _killCountMemo.Key)
		{
			string value = killCount.ToString();
			_killCountMemo = new KeyValuePair<int, string>(killCount, value);
		}
		return _killCountMemo.Value;
	}
}
