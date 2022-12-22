using System.Collections.Generic;
using UnityEngine;

public class PetsInfoDesigner : MonoBehaviour
{
	[Header("Common settings")]
	public PetInfo.BehaviourType Behaviour;

	public ItemDb.ItemRarity Rarity;

	public int Tier;

	public string Lkey;

	public int ToUpPoints;

	[Header("Shop settings")]
	public Vector3 positionInBanners;

	public Vector3 rotationInBanners;

	public List<GadgetInfo.Parameter> parameters;

	public List<WeaponSounds.Effects> effects;

	[Header("AI settings")]
	public float AttackDistance;

	public float AttackStopDistance;

	public float MinToOwnerDistance;

	public float MaxToOwnerDistance;

	public float TargetDetectRange;

	public float OffenderDetectRange = 10f;

	public float ToTargetTeleportDistance = 7f;

	[Header("Poison settings")]
	public bool poisonEnabled;

	public Player_move_c.PoisonType poisonType;

	public int poisonCount = 3;

	public float poisonTime = 2f;

	public float poisonDamagePercent = 0.033f;

	[Header("Critical hit settings")]
	public int criticalHitChance;

	public float criticalHitCoef = 2f;
}
