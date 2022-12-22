using UnityEngine;

[ExecuteInEditMode]
public class LightTuner : MonoBehaviour
{
	public Light[] lighters;

	public float value;

	public bool apply;

	private void Start()
	{
	}

	private void Update()
	{
		if (apply)
		{
			Light[] array = lighters;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].intensity *= value;
			}
			apply = false;
		}
	}
}
