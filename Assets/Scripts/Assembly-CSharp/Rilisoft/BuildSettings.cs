using UnityEngine;

namespace Rilisoft
{
	public static class BuildSettings
	{
		public static RuntimePlatform BuildTargetPlatform
		{
			get
			{
				switch (Application.platform)
				{
				case RuntimePlatform.MetroPlayerX86:
				case RuntimePlatform.MetroPlayerX64:
				case RuntimePlatform.MetroPlayerARM:
					return RuntimePlatform.MetroPlayerX64;
				default:
					return Application.platform;
				}
			}
		}
	}
}
