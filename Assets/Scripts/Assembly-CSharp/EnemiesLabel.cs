using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
internal sealed class EnemiesLabel : MonoBehaviour
{
	private UILabel _label;

	private ZombieCreator _zombieCreator;

	private KeyValuePair<int, string> _enemiesCountMemo = new KeyValuePair<int, string>(0, "0");

	private string _sceneName = string.Empty;

	private void Start()
	{
		bool flag = !Defs.isMulti;
		base.gameObject.SetActive(flag);
		if (flag)
		{
			_label = GetComponent<UILabel>();
			_zombieCreator = GameObject.FindGameObjectWithTag("GameController").GetComponent<ZombieCreator>();
			_sceneName = SceneManager.GetActiveScene().name ?? string.Empty;
		}
	}

	private void Update()
	{
		_label.text = GetEnemiesCountString();
	}

	private void OnDestroy()
	{
		_enemiesCountMemo = new KeyValuePair<int, string>(0, string.Empty);
	}

	private string GetEnemiesCountString()
	{
		int num = ZombieCreator.GetEnemiesToKill(_sceneName) - _zombieCreator.NumOfDeadZombies;
		if (num != _enemiesCountMemo.Key)
		{
			string value = num.ToString(CultureInfo.InvariantCulture);
			_enemiesCountMemo = new KeyValuePair<int, string>(num, value);
		}
		return _enemiesCountMemo.Value;
	}
}
