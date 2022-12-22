using Rilisoft;
using UnityEngine;

public class FacebookInNetworkTableBtn : MonoBehaviour
{
	private void Start()
	{
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnClick()
	{
		ButtonClickSound.Instance.PlayClick();
		if (WeaponManager.sharedManager.myTable != null)
		{
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().PostFacebookBtnClick();
		}
	}
}
