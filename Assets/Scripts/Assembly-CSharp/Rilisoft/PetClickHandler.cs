using System.Linq;
using Unity.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class PetClickHandler : MonoBehaviour
	{
		public const string TAP_ANIMATION_NAME = "Tap";

		[SerializeField]
		private float _threshold = 10f;

		private Vector3? _mouseDownPos;

		private PetEngine _myPetEngine;

		private Camera _clickStartCamera;

		private bool _clickAnimIsPlaying;

		private static bool CanClickToPet
		{
			get
			{
				return (FeedbackMenuController.Instance == null || !FeedbackMenuController.Instance.gameObject.activeSelf) && (PauseGUIController.Instance == null || !PauseGUIController.Instance.IsPaused) && (BankController.Instance == null || BankController.Instance.uiRoot == null || !BankController.Instance.uiRoot.gameObject.activeInHierarchy) && (NewsLobbyController.sharedController == null || !NewsLobbyController.sharedController.isActiveAndEnabled) && (LeaderboardScript.Instance == null || !LeaderboardScript.Instance.UIEnabled) && (FriendsWindowGUI.Instance == null || !FriendsWindowGUI.Instance.cameraObject.activeInHierarchy) && !Nest.Instance.BannerIsVisible && (FriendsController.sharedController == null || !FriendsController.sharedController.ProfileInterfaceActive) && (BannerWindowController.SharedController == null || !BannerWindowController.SharedController.IsAnyBannerShown) && (FreeAwardController.Instance == null || FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>()) && (MainMenuController.sharedController == null || !MainMenuController.sharedController.InAdventureScreen) && (MainMenuController.sharedController == null || (!MainMenuController.sharedController.SettingsJoysticksPanel.activeInHierarchy && !MainMenuController.sharedController.settingsPanel.activeInHierarchy));
			}
		}

		private void OnDisable()
		{
			_mouseDownPos = null;
			_clickAnimIsPlaying = false;
		}

		private void Start()
		{
			_myPetEngine = GetPetEngine(base.gameObject);
			if (_myPetEngine == null)
			{
				Debug.LogError("PetEngine not found");
			}
		}

		private void Update()
		{
			if (_myPetEngine == null)
			{
				return;
			}
			if (Input.GetMouseButtonDown(0))
			{
				_mouseDownPos = Input.mousePosition;
				_clickStartCamera = GetCurrentCamera();
			}
			else if (Input.GetMouseButtonUp(0) && _mouseDownPos.HasValue && !(_clickStartCamera != GetCurrentCamera()) && Vector3.Distance(Input.mousePosition, _mouseDownPos.Value) <= _threshold && CanClickToPet && (!ShopNGUIController.GuiActive || !(_myPetEngine != ShopNGUIController.sharedShop.ShopCharacterInterface.myPetEngine)))
			{
				GameObject touchedGo = null;
				if (CheckTouchToPet(out touchedGo) && touchedGo.Equals(_myPetEngine.gameObject))
				{
					PlayClickAnimation();
				}
			}
		}

		private bool CheckTouchToPet(out GameObject touchedGo)
		{
			touchedGo = null;
			Camera currentCamera = GetCurrentCamera();
			if (currentCamera == null)
			{
				return false;
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(currentCamera.ScreenPointToRay(Input.mousePosition), out hitInfo) && (hitInfo.transform.gameObject.CompareTag("Pet") || hitInfo.transform.gameObject.IsSubobjectOf(_myPetEngine.gameObject)))
			{
				touchedGo = _myPetEngine.gameObject;
				return true;
			}
			return false;
		}

		private Camera GetCurrentCamera()
		{
			Camera result = null;
			if (ShopNGUIController.GuiActive)
			{
				result = ShopNGUIController.sharedShop.Camera3D;
			}
			else if (ProfileController.Instance != null && ProfileController.Instance.InterfaceEnabled)
			{
				result = ProfileController.Instance.Camera3D;
			}
			else if (MainMenuController.sharedController != null)
			{
				result = MainMenuController.sharedController.Camera3D;
			}
			return result;
		}

		private void PlayClickAnimation()
		{
			if (_clickAnimIsPlaying || !_myPetEngine.AnimationHandler.AnimationIsExists("Tap"))
			{
				return;
			}
			_clickAnimIsPlaying = true;
			_myPetEngine.AnimationHandler.SubscribeTo("Tap", AnimationHandler.AnimState.Finished, true, delegate
			{
				_clickAnimIsPlaying = false;
				PetAnimation animation = _myPetEngine.GetAnimation(PetAnimationType.Profile);
				if (animation != null)
				{
					_myPetEngine.Animator.Play(animation.AnimationName);
				}
			});
			_myPetEngine.Animator.Play("Tap");
			if (Defs.isSoundFX && _myPetEngine.ClipTap != null)
			{
				_myPetEngine.AudioSourceOne.spatialBlend = 0f;
				_myPetEngine.AudioSourceOne.clip = _myPetEngine.ClipTap;
				_myPetEngine.AudioSourceOne.Play();
			}
		}

		private PetEngine GetPetEngine(GameObject petGo)
		{
			if (petGo == null)
			{
				return null;
			}
			PetEngine component = petGo.GetComponent<PetEngine>();
			if (component != null)
			{
				return component;
			}
			component = petGo.GetComponentInParents<PetEngine>();
			if (component != null)
			{
				return component;
			}
			GameObject gameObject = petGo.Ancestors().LastOrDefault();
			if (gameObject != null)
			{
				GameObject[] array = gameObject.Descendants().ToArray();
				GameObject[] array2 = array;
				foreach (GameObject gameObject2 in array2)
				{
					if (gameObject2.GetComponent<PetEngine>() != null)
					{
						return gameObject2.GetComponent<PetEngine>();
					}
				}
			}
			return null;
		}
	}
}
