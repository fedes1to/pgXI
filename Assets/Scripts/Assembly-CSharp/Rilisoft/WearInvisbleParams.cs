using UnityEngine;

namespace Rilisoft
{
	[RequireComponent(typeof(Renderer))]
	public class WearInvisbleParams : MonoBehaviour
	{
		public bool SkipSetInvisible;

		public bool HideIsInvisible;

		public string InvisibleShader = "Mobile/Transparent-Shop";

		[ReadOnly]
		public string BaseShader;

		private Renderer _rend;

		private void Awake()
		{
			_rend = GetComponent<Renderer>();
			if (!(_rend == null))
			{
				BaseShader = _rend.sharedMaterial.shader.name;
			}
		}
	}
}
