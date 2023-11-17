using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class LightmapUtility : Editor
{
	public static string[] DiffuseShaders = new string[]
	{
		"Mobile/Diffuse",
		"Mobile/Diffuse Detail",
		"Legacy Shaders/Diffuse",
		"Legacy Shaders/Self-Illumin/Diffuse",
		"Legacy Shaders/Better Lightmapped/Diffuse",
		"Legacy Shaders/Lightmapped/Diffuse"
	};

	//make a transparent thing or whatever
	public static string[] TransparentShaders = new string[]
	{
		"Sprites/Default",
		"Sprites/Diffuse",
		"Unlit/Transparent",
		"Legacy Shaders/Transparent/Diffuse"
	};

	public static List<Material> DiffuseShadersInThisScene
	{
		get
		{
			List<Material> finalMats = new List<Material>();
			List<string> shaders = DiffuseShaders.ToList();
			foreach (MeshRenderer renderer in Resources.FindObjectsOfTypeAll<MeshRenderer>())
			{
				if (renderer.gameObject.isStatic && renderer.gameObject.activeInHierarchy)
				{
					foreach (Material mat in renderer.sharedMaterials)
					{
						try
						{
							if (shaders.Contains(mat.shader.name))
							{
								finalMats.Add(mat);
							}
						} catch {}
					}
				}
			}
			return finalMats;
		}
	}

	public static List<Material> TransparentShadersInThisScene
	{
		get
		{
			List<Material> finalMats = new List<Material>();
			List<string> shaders = TransparentShaders.ToList();
			foreach (MeshRenderer renderer in Resources.FindObjectsOfTypeAll<MeshRenderer>())
			{
				if (renderer.gameObject.isStatic && renderer.gameObject.activeInHierarchy)
				{
					foreach (Material mat in renderer.sharedMaterials)
					{
						try
						{
							if (shaders.Contains(mat.shader.name))
							{
								finalMats.Add(mat);
							}
						} catch {}
					}
				}
			}
			return finalMats;
		}
	}

	public static Texture2D CurrentLightmap
	{
		get
		{
			return Resources.Load<Texture2D>("lightmap/high/" + Application.loadedLevelName.ToLower() + "/Lightmap-0_comp_light");
		}
	}

	public static void SetDiffuseShaders(List<Material> diffuseMats)
	{
		foreach (Material mat in diffuseMats)
		{
			mat.shader = Shader.Find("Legacy Shaders/Better Lightmapped/Diffuse");
			mat.SetColor("_Color", new Color(1, 1, 1, 1));
		}
	}

	public static void SetTransparentShaders(List<Material> transparentMats)
	{
		foreach (Material mat in transparentMats)
		{
			mat.shader = Shader.Find("Legacy Shaders/Better Lightmapped/Transparent Diffuse");
			mat.SetColor("_Color", new Color(1, 1, 1, 1));
		}
	}

	public static void ChangeLightmaps(List<Material> mats, Texture2D lightmap)
	{
		foreach (Material mat in mats)
		{
			mat.SetTexture("_LightMap", lightmap);
		}
	}

	public static void ChangeLightmapsMult(List<Material> mats, float mult)
	{
		foreach (Material mat in mats)
		{
			mat.SetFloat("_LightMapMult", mult);
		}
	}
}
