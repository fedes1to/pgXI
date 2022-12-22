using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	public sealed class AdvertisementController : MonoBehaviour
	{
		public enum State
		{
			Idle,
			Checking,
			Disabled,
			Downloading,
			Error,
			Complete,
			Closed
		}

		public bool updateBanner;

		public bool updateFromMultiBanner;

		private Texture2D _advertisementTexture;

		private State _currentState;

		private WWW _checkingRequest;

		private float _disabledTimeStamp;

		private WWW _imageRequest;

		private readonly HashSet<State> _permittedStatesForRun = new HashSet<State>
		{
			State.Idle,
			State.Disabled,
			State.Closed
		};

		public Texture2D AdvertisementTexture
		{
			get
			{
				return _advertisementTexture;
			}
		}

		public State CurrentState
		{
			get
			{
				return _currentState;
			}
		}

		public void Close()
		{
			if (_currentState != State.Complete)
			{
				Debug.LogError(string.Concat("AdvertisementController cannot be started in ", _currentState, " state."));
				return;
			}
			_advertisementTexture = null;
			if (_imageRequest != null)
			{
				_imageRequest.Dispose();
				_imageRequest = null;
			}
			_currentState = State.Closed;
		}

		public void Run()
		{
			if (!_permittedStatesForRun.Contains(_currentState))
			{
				Debug.LogError(string.Concat("AdvertisementController cannot be started in ", _currentState, " state."));
				return;
			}
			if (Debug.isDebugBuild)
			{
				Debug.Log("Start checking advertisement.");
			}
			if (_imageRequest != null)
			{
				_imageRequest.Dispose();
				_imageRequest = null;
			}
			if (_checkingRequest != null)
			{
				_checkingRequest.Dispose();
			}
			if (AdsConfigManager.Instance.LastLoadedConfig == null)
			{
				return;
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
			PerelivSettings perelivSettings = PerelivConfigManager.Instance.GetPerelivSettings(playerCategory);
			if (!string.IsNullOrEmpty(perelivSettings.Error))
			{
				Debug.LogWarning(perelivSettings.Error);
			}
			else
			{
				if (!perelivSettings.Enabled)
				{
					return;
				}
				if (string.IsNullOrEmpty(perelivSettings.ImageUrl))
				{
					Debug.LogWarning("Pereliv image url is empty.");
					return;
				}
				if (updateBanner || updateFromMultiBanner)
				{
					_advertisementTexture = Resources.Load<Texture2D>("update_available");
					_currentState = State.Complete;
					return;
				}
				if (Application.isEditor && FriendsController.isDebugLogWWW)
				{
					string message = string.Format("<color=yellow><size=14>PerelivSettings.ImageUrl: {0}</size></color>", perelivSettings.ImageUrl);
					Debug.Log(message);
				}
				_checkingRequest = Tools.CreateWwwIfNotConnected(perelivSettings.ImageUrl);
				_currentState = State.Checking;
			}
		}

		private void OnDestroy()
		{
			if (_checkingRequest != null)
			{
				_checkingRequest.Dispose();
			}
			if (_imageRequest != null)
			{
				_imageRequest.Dispose();
			}
		}

		private void Update()
		{
			switch (_currentState)
			{
			case State.Idle:
				break;
			case State.Checking:
				if (_checkingRequest == null)
				{
					Debug.LogError("Checking request is null.");
					_currentState = State.Idle;
				}
				else if (!string.IsNullOrEmpty(_checkingRequest.error))
				{
					Debug.LogWarning(_checkingRequest.error);
					_checkingRequest.Dispose();
					_checkingRequest = null;
					_disabledTimeStamp = Time.time;
					_currentState = State.Disabled;
				}
				else if (_checkingRequest.isDone)
				{
					if (Debug.isDebugBuild)
					{
						Debug.Log("Complete checking advertisement.");
					}
					_advertisementTexture = null;
					_imageRequest = _checkingRequest;
					_checkingRequest = null;
					_currentState = State.Downloading;
				}
				break;
			case State.Disabled:
				if (Time.time - _disabledTimeStamp > 300f)
				{
					_disabledTimeStamp = 0f;
					Run();
				}
				break;
			case State.Downloading:
				if (_imageRequest == null)
				{
					Debug.LogError("Image request is null.");
					_currentState = State.Idle;
				}
				else if (!string.IsNullOrEmpty(_imageRequest.error))
				{
					Debug.LogWarning(_imageRequest.error);
					_currentState = State.Error;
				}
				else if (_imageRequest.isDone)
				{
					if (Debug.isDebugBuild)
					{
						Debug.Log("Complete downloading advertisement.");
					}
					_advertisementTexture = _imageRequest.texture;
					_currentState = State.Complete;
				}
				break;
			case State.Error:
				break;
			case State.Complete:
				break;
			case State.Closed:
				break;
			default:
				Debug.LogError("Unknown state.");
				break;
			}
		}
	}
}
