using UnityEngine;

public class BootsMaterial : MonoBehaviour
{
	public MeshRenderer bootRenderer;

	public Material[] materialList;

	public void SetBootsMaterial(string materialName)
	{
		for (int i = 0; i < materialList.Length; i++)
		{
			if (materialList[i].name == materialName)
			{
				bootRenderer.sharedMaterial = materialList[i];
			}
		}
	}
}
