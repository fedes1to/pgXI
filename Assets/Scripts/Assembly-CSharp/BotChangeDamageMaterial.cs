using UnityEngine;

public sealed class BotChangeDamageMaterial : MonoBehaviour
{
	private Texture _mainTexture;

	private void Start()
	{
		string text = base.transform.root.GetChild(0).name;
		Texture texture = null;
		if (text.Contains("Enemy"))
		{
			string text2 = text + "_Level" + CurrentCampaignGame.currentLevel;
			if (!(texture = SkinsManagerPixlGun.sharedManager.skins[text2] as Texture))
			{
				Debug.Log("No skin: " + text2);
			}
		}
		if (texture != null)
		{
			_mainTexture = texture;
			ResetMainMaterial();
		}
		else
		{
			_mainTexture = GetComponent<Renderer>().material.mainTexture;
		}
	}

	public void ShowDamageEffect(bool poison = false)
	{
		GetComponent<Renderer>().material.mainTexture = ((!poison) ? SkinsController.damageHitTexture : SkinsController.poisonHitTexture);
	}

	public void ResetMainMaterial()
	{
		GetComponent<Renderer>().material.mainTexture = _mainTexture;
	}
}
