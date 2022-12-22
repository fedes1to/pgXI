using UnityEngine;

public class SetDaterIconInFastShop : MonoBehaviour
{
	public string daterIconName;

	private void Awake()
	{
		if (Defs.isDaterRegim)
		{
			GetComponent<UISprite>().spriteName = daterIconName;
		}
	}
}
