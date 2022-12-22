using UnityEngine;

public class testMem : MonoBehaviour
{
	private void Start()
	{
		if (!meminfo.getMemInfo())
		{
			Debug.Log("Could not get Memory Info");
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, Screen.height - 50, 180f, 40f), "Get MemInfo"))
		{
			meminfo.getMemInfo();
		}
		if (GUI.Button(new Rect(200f, Screen.height - 50, 180f, 40f), "native Gc Collect"))
		{
			meminfo.gc_Collect();
		}
		GUI.Label(new Rect(50f, 10f, 250f, 40f), "memtotal: " + meminfo.minf.memtotal + " kb");
		GUI.Label(new Rect(50f, 50f, 250f, 40f), "memfree: " + meminfo.minf.memfree + " kb");
		GUI.Label(new Rect(50f, 90f, 250f, 40f), "active: " + meminfo.minf.active + " kb");
		GUI.Label(new Rect(50f, 130f, 250f, 40f), "inactive: " + meminfo.minf.inactive + " kb");
		GUI.Label(new Rect(50f, 170f, 250f, 40f), "cached: " + meminfo.minf.cached + " kb");
		GUI.Label(new Rect(50f, 210f, 250f, 40f), "swapcached: " + meminfo.minf.swapcached + " kb");
		GUI.Label(new Rect(50f, 250f, 250f, 40f), "swaptotal: " + meminfo.minf.swaptotal + " kb");
		GUI.Label(new Rect(50f, 290f, 250f, 40f), "swapfree: " + meminfo.minf.swapfree + " kb");
	}
}
