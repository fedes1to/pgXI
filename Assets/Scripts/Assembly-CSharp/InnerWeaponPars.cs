using UnityEngine;

public class InnerWeaponPars : MonoBehaviour
{
	public GameObject particlePoint;

	public Transform LeftArmorHand;

	public Transform RightArmorHand;

	public Transform grenatePoint;

	public AudioClip shoot;

	public AudioClip reload;

	public AudioClip empty;

	public AudioClip idle;

	public AudioClip zoomIn;

	public AudioClip zoomOut;

	public AudioClip charge;

	public GameObject bonusPrefab;

	public GameObject shockerEffect;

	public GameObject fakeGrenade;

	public GameObject animationObject;

	public Texture preview;

	public Texture2D aimTextureV;

	public Texture2D aimTextureH;

	private SkinnedMeshRenderer renderArms;

	private void Awake()
	{
		FindArms();
	}

	private void FindArms()
	{
		if (renderArms != null)
		{
			return;
		}
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>(true);
		if (componentsInChildren == null)
		{
			return;
		}
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			if (skinnedMeshRenderer != null && skinnedMeshRenderer.gameObject != bonusPrefab)
			{
				renderArms = skinnedMeshRenderer;
				break;
			}
		}
	}

	public void SetMaterialForArms(Material shMat)
	{
		if (renderArms == null)
		{
			FindArms();
		}
		if (renderArms != null)
		{
			renderArms.sharedMaterial = shMat;
		}
	}
}
