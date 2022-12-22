using UnityEngine;

public class PlayerMechBody : MonoBehaviour
{
	public AudioClip activationSound;

	public AudioClip stepSound;

	public AudioClip shootSound;

	public Vector3 cameraPosition = new Vector3(0.12f, 0.7f, -0.3f);

	public Vector3 gunCameraPosition = new Vector3(-0.1f, 0f, 0f);

	public float gunCameraFieldOfView = 45f;

	public float bodyColliderHeight = 2.07f;

	public Vector3 bodyColliderCenter = new Vector3(0f, 0.19f, 0f);

	public Vector3 headColliderCenter = new Vector3(0f, 0.54f, 0f);

	public float nickLabelYPoision = 1.72f;

	public AudioSource explosionSound;

	public GameObject activationEffect;

	public GameObject explosionEffect;

	public GameObject jetpackObject;

	public GameObject body;

	public GameObject gun;

	public GameObject point;

	public Renderer bodyRenderer;

	public Renderer handsRenderer;

	public Renderer gunRenderer;

	public Animation bodyAnimation;

	public Animation gunAnimation;

	public WeaponSounds weapon;

	public string killChatIcon = "Chat_Mech";

	public PlayerEventScoreController.ScoreEvent scoreEventOnKill = PlayerEventScoreController.ScoreEvent.deadMech;

	public float dieTime = 0.46f;

	public float explosionEffectTime = 1f;

	public float activationEffectTime = 2f;

	public string[] healthSpriteName = new string[6] { "mech_armor1", "mech_armor2", "mech_armor3", "mech_armor4", "mech_armor5", "mech_armor6" };

	public bool dontShowHandsInThirdPerson;

	private float dieTimer = -1f;

	private float explosionEffectTimer = -1f;

	private Texture defaultBodyTexture;

	private Texture defaultHandsTexture;

	public void ShowActivationEffect()
	{
		activationEffect.SetActive(true);
		activationEffect.GetComponent<DisableObjectFromTimer>().timer = activationEffectTime;
	}

	public void ShowExplosionEffect()
	{
		dieTimer = Time.time + dieTime;
		explosionEffectTimer = Time.time + explosionEffectTime;
		explosionEffect.SetActive(true);
	}

	private void Awake()
	{
		defaultBodyTexture = bodyRenderer.sharedMaterial.mainTexture;
		defaultHandsTexture = handsRenderer.sharedMaterial.mainTexture;
	}

	public void ShowHitMaterial(bool hit, bool poison = false)
	{
		if (hit)
		{
			bodyRenderer.material.mainTexture = ((!poison) ? SkinsController.damageHitTexture : SkinsController.poisonHitTexture);
			handsRenderer.material.mainTexture = ((!poison) ? SkinsController.damageHitTexture : SkinsController.poisonHitTexture);
		}
		else
		{
			bodyRenderer.material.mainTexture = defaultBodyTexture;
			handsRenderer.material.mainTexture = defaultHandsTexture;
		}
	}

	private void Update()
	{
		if (dieTimer != -1f && dieTimer < Time.time)
		{
			point.SetActive(false);
			dieTimer = -1f;
		}
		if (explosionEffectTimer != -1f && explosionEffectTimer < Time.time)
		{
			explosionEffect.SetActive(false);
			explosionEffectTimer = -1f;
		}
	}
}
