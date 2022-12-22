using UnityEngine;

public class DevMemoryController : MonoBehaviour
{
	public static string keyActiveMemoryInfo = "keyActiveMemoryInfo";

	public static DevMemoryController instance;

	private void Awake()
	{
		Object.Destroy(this);
	}
}
