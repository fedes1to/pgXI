using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Linq;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class WeaponSkin
	{
		public string Id = string.Empty;

		public string Lkey = string.Empty;

		public int Price;

		public VirtualCurrencyBonusType Currency;

		public RatingSystem.RatingLeague ForLeague;

		public string ShaderName = "Mobile/Diffuse";

		public WeaponSkinTexture[] Textures = new WeaponSkinTexture[0];

		public string[] ToWeapons = new string[0];

		private static Regex _regex = new Regex("Weapon([0-9])+");

		public bool IsForWeapon(string weaponName)
		{
			if (weaponName == null)
			{
				Debug.LogError("WeaponSkin IsForWeapon weaponName == null");
				return false;
			}
			ItemRecord byPrefabName = ItemDb.GetByPrefabName(weaponName);
			if (byPrefabName == null)
			{
				Debug.LogError("WeaponSkin IsForWeapon rec == null, weaponName: " + weaponName);
				return false;
			}
			List<string> list = WeaponUpgrades.ChainForTag(byPrefabName.Tag);
			if (list == null)
			{
				return weaponName == ToWeapons[0];
			}
			ItemRecord byTag = ItemDb.GetByTag(list[0]);
			if (byTag == null)
			{
				Debug.LogError("WeaponSkin IsForWeapon recOfFirstWeapon == null, weaponName: " + weaponName);
				return false;
			}
			return byTag.PrefabName == ToWeapons[0];
		}

		public bool SetTo(GameObject go)
		{
			//Discarded unreachable code: IL_0199, IL_01cc
			try
			{
				if (go == null)
				{
					return false;
				}
				go = go.GetChildGameObject("Arms_Mesh", true);
				if (go == null)
				{
					return false;
				}
				bool flag = false;
				WeaponSkinTexture[] textures = Textures;
				foreach (WeaponSkinTexture weaponSkinTexture in textures)
				{
					if (weaponSkinTexture.ToObjects.IsNullOrEmpty())
					{
						continue;
					}
					string[] toObjects = weaponSkinTexture.ToObjects;
					foreach (string name in toObjects)
					{
						GameObject childGameObject = go.GetChildGameObject(name, true);
						if (childGameObject != null)
						{
							Renderer component = childGameObject.GetComponent<Renderer>();
							if (component == null)
							{
								flag = true;
							}
							Material material = component.material;
							if (material.shader.name != ShaderName)
							{
								Shader shader = Shader.Find(ShaderName);
								if (shader == null)
								{
									flag = true;
								}
								material = (component.material = new Material(shader));
							}
							if (weaponSkinTexture.ShaderPropertyName.IsNullOrEmpty())
							{
								weaponSkinTexture.ShaderPropertyName = "_MainTex";
							}
							material.SetTexture(weaponSkinTexture.ShaderPropertyName, weaponSkinTexture.Texture);
							if (childGameObject.GetComponent<UvScroller>() == null)
							{
								childGameObject.AddComponent<UvScroller>();
							}
						}
						else
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					Debug.LogErrorFormat("[WEAPON SKIN] set error: skin:'{0}', go:'{1}'", Id, go.name);
				}
				return !flag;
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("{0} {1}", ex.Message, ex.StackTrace);
				return false;
			}
		}

		public static WeaponSkin CreateFromWeapon(GameObject go, string id, int price, VirtualCurrencyBonusType currency, string weaponId)
		{
			go = go.GetChildGameObject("Arms_Mesh", true);
			if (go == null)
			{
				return null;
			}
			List<WeaponSkinTexture> list = new List<WeaponSkinTexture>();
			foreach (GameObject item in go.Descendants())
			{
				Renderer component = item.GetComponent<Renderer>();
				if (!(component == null))
				{
					Material material = component.material;
					Texture2D texture2D = material.mainTexture as Texture2D;
					if (!(texture2D == null))
					{
						string raw = SkinsController.StringFromTexture(texture2D);
						list.Add(new WeaponSkinTexture(raw, texture2D.width, texture2D.height, new string[1] { item.name }));
					}
				}
			}
			WeaponSkin weaponSkin = new WeaponSkin();
			weaponSkin.Id = id;
			weaponSkin.Price = price;
			weaponSkin.Currency = currency;
			weaponSkin.Textures = list.ToArray();
			ItemRecord byPrefabName = ItemDb.GetByPrefabName(weaponId);
			if (byPrefabName != null)
			{
				List<string> list2 = WeaponUpgrades.ChainForTag(byPrefabName.Tag);
				if (list2 == null)
				{
					weaponSkin.ToWeapons = new string[1] { weaponId };
				}
				else
				{
					ItemRecord byTag = ItemDb.GetByTag(list2[0]);
					weaponSkin.ToWeapons = new string[1] { byTag.PrefabName };
				}
			}
			return weaponSkin;
		}

		public static string GetWeaponId(string containsString)
		{
			Match match = _regex.Match(containsString);
			return (!match.Success) ? string.Empty : match.Value;
		}
	}
}
