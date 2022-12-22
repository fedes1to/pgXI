using UnityEngine;
using ZeichenKraftwerk;

public class RocketSettings : MonoBehaviour
{
	public enum TypeFlyRocket
	{
		Rocket,
		Grenade,
		Bullet,
		MegaBullet,
		Autoaim,
		Bomb,
		AutoaimBullet,
		Ball,
		GravityRocket,
		Lightning,
		AutoTarget,
		StickyBomb,
		Ghost,
		ChargeRocket,
		ToxicBomb,
		GrenadeBouncing,
		SingularityGrenade,
		NuclearGrenade,
		StickyMine,
		Molotov,
		Drone,
		FakeBonus,
		BlackMark,
		Firework,
		HomingGrenade,
		SlowdownGrenade
	}

	[Header("General settings")]
	public TypeFlyRocket typeFly;

	public WeaponSounds.TypeDead typeDead = WeaponSounds.TypeDead.explosion;

	public Player_move_c.TypeKills typeKilsIconChat = Player_move_c.TypeKills.explosion;

	public float lifeTime = 7f;

	public float startForce = 190f;

	[Header("Particles")]
	public GameObject flyParticle;

	public TrailRenderer trail;

	public Rotator droneRotator;

	public GameObject droneParticle;

	[Header("Size detect collider")]
	public Vector3 sizeBoxCollider = new Vector3(0.15f, 0.15f, 0.75f);

	public Vector3 centerBoxCollider = new Vector3(0f, 0f, 0f);

	[Header("For AutoTarget, Autoaim")]
	public float autoRocketForce = 15f;

	[Header("For AutoTarget, StickyBomb, ToxicBomb")]
	public float raduisDetectTarget = 5f;

	[Header("For AutoTarget, StickyBomb, ToxicBomb")]
	public float toxicHitTime = 1f;

	public float toxicDamageMultiplier = 0.2f;

	[Header("For StickyBomb, ToxicBomb")]
	public GameObject stickedParticle;

	[Header("For Lightning")]
	public int countJumpLightning = 2;

	public float raduisDetectTargetLightning = 5f;

	[Header("For charge weapon")]
	public float chargeScaleMin = 0.7f;

	public float chargeScaleMax = 1.2f;
}
