using I2.Loc;
using UnityEngine;

[ExecuteInEditMode]
public class LocalizeRili : MonoBehaviour
{
	public GameObject[] labels;

	public string term;

	public bool execute;

	[Header("delete after execute?")]
	public bool selfDestroy;

	private void Start()
	{
		if (base.gameObject.GetComponent<UILabel>() != null)
		{
			labels = new GameObject[1] { base.gameObject };
		}
	}

	private void Update()
	{
		if (!execute || labels == null)
		{
			return;
		}
		GameObject[] array = labels;
		foreach (GameObject gameObject in array)
		{
			while (gameObject.GetComponent<Localize>() != null)
			{
				Localize component = gameObject.GetComponent<Localize>();
				Object.DestroyImmediate(component);
			}
			Localize localize = gameObject.gameObject.AddComponent<Localize>();
			localize.SetTerm("Key_04B_03", "Key_04B_03");
			if (term != string.Empty)
			{
				Localize localize2 = gameObject.gameObject.AddComponent<Localize>();
				localize2.SetTerm(term, term);
			}
		}
		execute = false;
		if (selfDestroy)
		{
			Object.DestroyImmediate(this);
		}
	}
}
