using System.Collections;
using UnityEngine;

namespace RilisoftBot
{
	[RequireComponent(typeof(BaseBot))]
	public class PortalEnemyEffectsManager : MonoBehaviour, IEnemyEffectsManager
	{
		private const string SpawnShaderParamName = "_Burn";

		private const float SpawnPlayTime = 1f;

		private const float SpawnBurnAmountStart = 0.25f;

		private const float SpawnBurnAmountEnd = 1.25f;

		private BaseBot _bot;

		private Material _portalMaterialPref;

		private void Awake()
		{
			_bot = GetComponent<BaseBot>();
			_portalMaterialPref = Resources.Load<Material>("Enemy_Portal");
			if (_portalMaterialPref == null)
			{
				Debug.LogError("material not found");
			}
		}

		public void ShowSpawnEffect()
		{
			ShowSpawnMaterials();
			ShowSpawnPortal();
		}

		private void ShowSpawnMaterials()
		{
			StartCoroutine(ShowSpawnMaterialsCoroutine());
		}

		private IEnumerator ShowSpawnMaterialsCoroutine()
		{
			yield return null;
			Renderer[] rends = GetComponentsInChildren<Renderer>();
			Renderer[] array = rends;
			foreach (Renderer rend in array)
			{
				StartCoroutine(AnimateMaterial(rend));
			}
		}

		private IEnumerator AnimateMaterial(Renderer rend)
		{
			Material baseMaterial = rend.material;
			if (rend.gameObject.GetComponent<BotChangeDamageMaterial>() != null)
			{
				string skinKey = _bot.name + "_Level" + CurrentCampaignGame.currentLevel;
				Texture tx = SkinsManagerPixlGun.sharedManager.skins[skinKey] as Texture;
				if (tx != null)
				{
					baseMaterial.mainTexture = tx;
				}
			}
			rend.material = new Material(_portalMaterialPref);
			rend.material.mainTexture = baseMaterial.mainTexture;
			rend.material.SetFloat("_Burn", 0.25f);
			float timeElapsed = 0f;
			while (timeElapsed < 1f)
			{
				timeElapsed += Time.deltaTime;
				float curVal = timeElapsed * 1.25f / 1f;
				rend.material.SetFloat("_Burn", curVal);
				yield return null;
			}
			rend.material = baseMaterial;
			yield return null;
		}

		private void ShowSpawnPortal()
		{
			EnemyPortal portal = EnemyPortalStackController.sharedController.GetPortal();
			if (!(portal == null))
			{
				portal.Show(base.gameObject.transform.position);
			}
		}
	}
}
