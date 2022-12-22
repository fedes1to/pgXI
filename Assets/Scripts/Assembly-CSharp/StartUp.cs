using UnityEngine;

public class StartUp : MonoBehaviour
{
	private void Start()
	{
		if (!Application.isEditor)
		{
			AppsFlyer.setAppsFlyerKey("Cja8TmYiYqwrDDFHJykmjD");
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				AppsFlyer.setAppID("com.pixel.gun3d");
			}
			else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				AppsFlyer.setAppID("com.PixelGun.a3D");
			}
			AppsFlyer.setIsDebug(Defs.IsDeveloperBuild);
			AppsFlyer.loadConversionData("AppsFlyerTrackerCallbacks", "didReceiveConversionData", "didReceiveConversionDataWithError");
			AppsFlyer.trackAppLaunch();
		}
	}
}
