public interface IDamageable
{
	bool isLivingTarget { get; }

	void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill);

	void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill, WeaponSounds.TypeDead typeDead, string weaponName, int killerViewId = 0);

	bool IsEnemyTo(Player_move_c player);

	bool IsDead();
}
