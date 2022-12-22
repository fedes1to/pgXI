using System;
using System.Collections;
using UnityEngine;

public class CleanUpAndDoAction : MonoBehaviour
{
	public Texture riliFon;

	public Texture blackPixel;

	public static Action action;

	private IEnumerator Start()
	{
		Action handler = action;
		if (ShopNGUIController.GuiActive)
		{
			ShopNGUIController.GuiActive = false;
		}
		PhotonNetwork.isMessageQueueRunning = true;
		PhotonNetwork.Disconnect();
		yield return null;
		yield return null;
		WeaponManager.sharedManager.UnloadAll();
		yield return null;
		yield return null;
		if (handler != null)
		{
			handler();
		}
		action = null;
		while (FacebookController.LoggingIn)
		{
			yield return null;
		}
		int i = 0;
		while (i < 60)
		{
			i++;
			yield return null;
		}
		while (FacebookController.LoggingIn)
		{
			yield return null;
		}
		Application.LoadLevel(Defs.MainMenuScene);
	}

	private void OnGUI()
	{
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), blackPixel, ScaleMode.StretchToFill);
		GUI.DrawTexture(AppsMenu.RiliFonRect(), riliFon, ScaleMode.StretchToFill);
	}
}
