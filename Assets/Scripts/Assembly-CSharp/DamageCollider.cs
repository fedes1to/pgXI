using UnityEngine;

public class DamageCollider : MonoBehaviour
{
	public float damage;

	public float frequency;

	private bool _playerRegistered;

	private float _remainsTimeToHit;

	private Transform cachedTransform;

	public void RegisterPlayer()
	{
		_playerRegistered = true;
		_remainsTimeToHit = frequency;
		CauseDamage();
	}

	public void UnregisterPlayer()
	{
		_playerRegistered = false;
	}

	private void Start()
	{
		cachedTransform = base.transform;
	}

	private void CauseDamage()
	{
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.GetDamage(damage, Player_move_c.TypeKills.himself, WeaponSounds.TypeDead.angel, default(Vector3), string.Empty);
		}
	}

	private void Update()
	{
		if (!_playerRegistered)
		{
			return;
		}
		if (WeaponManager.sharedManager.myPlayerMoveC == null)
		{
			_playerRegistered = false;
			return;
		}
		_remainsTimeToHit -= Time.deltaTime;
		if (_remainsTimeToHit <= 0f)
		{
			_remainsTimeToHit = frequency;
			CauseDamage();
		}
	}
}
