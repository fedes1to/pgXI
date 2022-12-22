using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class VisibleAtScene : MonoBehaviour
	{
		[SerializeField]
		private VisibleState _visible;

		[SerializeField]
		private bool _includeMenuScene;

		[SerializeField]
		private List<string> _scenes = new List<string>();

		private bool _baseVisible;

		private void Awake()
		{
			if (_includeMenuScene)
			{
				_scenes.Add(Defs.MainMenuScene);
			}
			_baseVisible = base.gameObject.activeSelf;
			_scenes = _scenes.Select((string s) => s.ToLower()).ToList();
			SetVisible(SceneLoader.ActiveSceneName);
			Singleton<SceneLoader>.Instance.OnSceneLoaded += OnSceneLoaded;
		}

		private void OnDestroy()
		{
			Singleton<SceneLoader>.Instance.OnSceneLoaded -= OnSceneLoaded;
		}

		private void OnSceneLoaded(SceneLoadInfo inf)
		{
			SetVisible(inf.SceneName);
		}

		private void SetVisible(string currentSceneName)
		{
			currentSceneName = currentSceneName.ToLower();
			if (_scenes.Contains(currentSceneName))
			{
				base.gameObject.SetActiveSafe(_visible == VisibleState.On);
			}
			else
			{
				base.gameObject.SetActiveSafe(_baseVisible);
			}
		}
	}
}
