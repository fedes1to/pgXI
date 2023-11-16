using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightmapFinder : EditorWindow
{
    [MenuItem("Lightmaps/Open Lightmap Fixer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LightmapFinder));
    }

	private static List<Material> lightmapDiffuse, lightmapTransparent;

	private static Texture2D lightmap;
    
    void OnGUI()
    {
		GUILayout.Label("Lightmap Fixage", EditorStyles.largeLabel);

		if (GUILayout.Button("Refresh lightmaps", GUILayout.Height(45), GUILayout.Width(125)))
		{
			lightmapDiffuse = LightmapUtility.DiffuseShadersInThisScene;
			lightmapTransparent = LightmapUtility.TransparentShadersInThisScene;
			lightmap = LightmapUtility.CurrentLightmap;
			LightmapUtility.SetDiffuseShaders(lightmapDiffuse);
			LightmapUtility.SetTransparentShaders(lightmapTransparent);
			LightmapUtility.ChangeLightmaps(lightmapDiffuse, lightmap);
			LightmapUtility.ChangeLightmaps(lightmapTransparent, lightmap);
		}
    }
}