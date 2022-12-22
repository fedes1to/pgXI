using System.Globalization;
using Rilisoft;
using UnityEngine;

public class RenderAllInSceneObj : MonoBehaviour
{
	private void Awake()
	{
		string callee = string.Format(CultureInfo.InvariantCulture, "{0}.Awake()", GetType().Name);
		ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
		try
		{
			if (Device.IsLoweMemoryDevice)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			Transform transform = Object.Instantiate(Resources.Load<GameObject>("RenderAllInSceneObjInner")).transform;
			transform.parent = base.transform;
			transform.localPosition = Vector3.zero;
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}
}
