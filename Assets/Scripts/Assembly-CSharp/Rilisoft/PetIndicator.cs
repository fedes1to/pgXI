using UnityEngine;

namespace Rilisoft
{
	public class PetIndicator : MonoBehaviour
	{
		[SerializeField]
		private GameObject _rootObject;

		[SerializeField]
		private GameObject _hpBarObject;

		[SerializeField]
		private UI2DSprite _iconSprite;

		[SerializeField]
		private Transform _HPIndicator;

		[SerializeField]
		private UITexture _RewiveIndicator;

		private PetEngine _myPetEngine;

		private string _prevPetId;

		private float _prevHp;

		private Shader _shaderEnable;

		private Shader _shaderDisable;

		private void Awake()
		{
			_shaderEnable = Shader.Find("Unlit/Transparent Colored");
			_shaderDisable = Shader.Find("Unlit/TransparentGrayscale");
		}

		private void Start()
		{
			if (!GlobalGameController.LeftHanded)
			{
				base.transform.localPosition += base.transform.right * ((float)Screen.width * (768f / (float)Screen.height) - base.transform.localPosition.x * 2f);
			}
		}

		private void Update()
		{
			if (WeaponManager.sharedManager == null || WeaponManager.sharedManager.myPlayerMoveC == null || WeaponManager.sharedManager.myPlayerMoveC.myPetEngine == null)
			{
				_rootObject.SetActiveSafe(false);
				return;
			}
			_rootObject.SetActiveSafe(true);
			_myPetEngine = WeaponManager.sharedManager.myPlayerMoveC.myPetEngine;
			if (_prevPetId != _myPetEngine.Info.IdWithoutUp)
			{
				_iconSprite.sprite2D = Resources.Load<Sprite>(string.Format("Pets/Icons/{0}_icon", _myPetEngine.Info.IdWithoutUp));
				_prevPetId = _myPetEngine.Info.IdWithoutUp;
			}
			if (_prevHp != _myPetEngine.CurrentHealth)
			{
				if (_myPetEngine.CurrentHealth > 0f)
				{
					_hpBarObject.SetActiveSafe(true);
					Vector3 localScale = new Vector3(_myPetEngine.CurrentHealth / _myPetEngine.Info.HP, _HPIndicator.localScale.y, _HPIndicator.localScale.z);
					_HPIndicator.localScale = localScale;
					_iconSprite.shader = _shaderEnable;
				}
				else
				{
					_hpBarObject.SetActiveSafe(false);
					_iconSprite.shader = _shaderDisable;
				}
				_prevHp = _myPetEngine.CurrentHealth;
			}
			if (WeaponManager.sharedManager.myPlayerMoveC.myPetEngine.RespawnTimeLeft > 0f)
			{
				_RewiveIndicator.gameObject.SetActive(true);
				_RewiveIndicator.fillAmount = (WeaponManager.sharedManager.myPlayerMoveC.myPetEngine.RespawnTime - WeaponManager.sharedManager.myPlayerMoveC.myPetEngine.RespawnTimeLeft) / WeaponManager.sharedManager.myPlayerMoveC.myPetEngine.RespawnTime;
			}
			else
			{
				_RewiveIndicator.gameObject.SetActive(false);
			}
		}
	}
}
