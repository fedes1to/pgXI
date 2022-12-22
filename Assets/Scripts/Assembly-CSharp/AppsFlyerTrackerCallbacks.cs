using UnityEngine;

public class AppsFlyerTrackerCallbacks : MonoBehaviour
{
	private void Start()
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks on Start");
	}

	private void Update()
	{
	}

	public void didReceiveConversionData(string conversionData)
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks:: got conversion data = " + conversionData);
	}

	public void didReceiveConversionDataWithError(string error)
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks:: got conversion data error = " + error);
	}

	public void didFinishValidateReceipt(string validateResult)
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks:: got didFinishValidateReceipt  = " + validateResult);
	}

	public void didFinishValidateReceiptWithError(string error)
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks:: got idFinishValidateReceiptWithError error = " + error);
	}

	public void onAppOpenAttribution(string validateResult)
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks:: got onAppOpenAttribution  = " + validateResult);
	}

	public void onAppOpenAttributionFailure(string error)
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks:: got onAppOpenAttributionFailure error = " + error);
	}

	public void onInAppBillingSuccess()
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks:: got onInAppBillingSuccess succcess");
	}

	public void onInAppBillingFailure(string error)
	{
		MonoBehaviour.print("AppsFlyerTrackerCallbacks:: got onInAppBillingFailure error = " + error);
	}
}
