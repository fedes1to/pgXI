using UnityEngine;

public class Recenter : MonoBehaviour
{
	public UICenterOnChild centerOnChildScript;

	[ReadOnly]
	public int nextChild = 1;

	[ReadOnly]
	public int prevChild = -1;

	public void CenterOn(int child)
	{
		int num = centerOnChildScript.centeredObject.transform.GetSiblingIndex() + child;
		if (num >= 0 && num < centerOnChildScript.transform.childCount)
		{
			Transform child2 = centerOnChildScript.transform.GetChild(num);
			centerOnChildScript.CenterOn(child2);
		}
	}
}
