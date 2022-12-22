using UnityEngine;

namespace FyberPlugin
{
	public class FyberSettings : ScriptableObject
	{
		private const string fyberSettingsAssetName = "FyberSettings";

		private const string fyberSettingsPath = "Fyber/Resources";

		private const string fyberSettingsAssetExtension = ".asset";

		private static FyberSettings instance;

		[SerializeField]
		[HideInInspector]
		private string bundlesJson;

		[HideInInspector]
		[SerializeField]
		private string configJson;

		[HideInInspector]
		[SerializeField]
		private int bundlesCount;

		public static FyberSettings Instance
		{
			get
			{
				return GetInstance();
			}
		}

		private void OnEnable()
		{
			GetInstance();
		}

		private static FyberSettings GetInstance()
		{
			if (instance == null)
			{
				PluginBridge.bridge = new PluginBridgeComponent();
				instance = Resources.Load("FyberSettings") as FyberSettings;
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<FyberSettings>();
				}
			}
			return instance;
		}

		internal string BundlesInfoJson()
		{
			return bundlesJson;
		}

		internal string BundlesConfigJson()
		{
			return configJson;
		}

		internal int BundlesCount()
		{
			return bundlesCount;
		}
	}
}
