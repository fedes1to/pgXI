using UnityEngine;

public class HideInLocal : MonoBehaviour
{
	private void Start()
	{
		if (!Defs.isInet || Defs.isDaterRegim)
		{
			base.gameObject.SetActive(false);
		}
	}
}
