using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class ScenesList : ScriptableObject
	{
		[ReadOnly]
		[SerializeField]
		public List<ExistsSceneInfo> Infos = new List<ExistsSceneInfo>();
	}
}
