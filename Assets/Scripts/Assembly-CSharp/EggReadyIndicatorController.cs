using Rilisoft;
using UnityEngine;

public class EggReadyIndicatorController : MonoBehaviour
{
	private UISprite _sprite;

	private float _lastUpdateTime = float.MinValue;

	private UISprite sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponent<UISprite>();
			}
			return _sprite;
		}
	}

	private void Update()
	{
		if (!(Time.realtimeSinceStartup - 0.5f < _lastUpdateTime))
		{
			bool flag = Singleton<EggsManager>.Instance.ReadyEggs().Count > 0;
			if (sprite != null && sprite.enabled != flag)
			{
				sprite.enabled = flag;
			}
			_lastUpdateTime = Time.realtimeSinceStartup;
		}
	}
}
