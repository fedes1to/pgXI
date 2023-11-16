using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class RenderTheRenderTexture : Editor
{
	[MenuItem("Assets/RenderTexture/Render To PNG", false, 0)]
	public static void RenderPNG()
	{
		if (!Directory.Exists(Application.dataPath + "/Rendered PNGs"))
		{
			Directory.CreateDirectory(Application.dataPath + "/Rendered PNGs");
		}

		foreach (object selected in Selection.objects)
		{
			if (selected is RenderTexture)
			{
				RenderTexture renderTexture = (RenderTexture)selected;
				string path = EditorUtility.SaveFilePanel("Save PNG", Application.dataPath, "Rendered PNG", "png");

				if (!File.Exists(path))
				{
					File.Create(path).Dispose();
				}
				Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        		RenderTexture.active = renderTexture;

        		tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        		tex.Apply();

        		File.WriteAllBytes(path, tex.EncodeToPNG());
				
			}
		}

		AssetDatabase.Refresh(ImportAssetOptions.Default);
	}
}
