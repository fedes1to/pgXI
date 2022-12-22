using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class PrefabHandler
	{
		[SerializeField]
		public string FullPath;

		public GameObject _prefab;

		public string ResourcePath
		{
			get
			{
				return ToResourcePath(FullPath);
			}
		}

		public GameObject Prefab
		{
			get
			{
				if (_prefab == null)
				{
					_prefab = Resources.Load<GameObject>(ResourcePath);
				}
				return _prefab;
			}
		}

		public static string ToResourcePath(string fullPath)
		{
			if (fullPath.IsNullOrEmpty())
			{
				return string.Empty;
			}
			List<string> list = fullPath.Split((!fullPath.Contains("/")) ? '\\' : '/').ToList();
			if (list.Count > 0 && list[0].Contains("Assets"))
			{
				list.RemoveAt(0);
			}
			if (list.Count > 0 && list[0].Contains("Resources"))
			{
				list.RemoveAt(0);
			}
			string separator = Path.DirectorySeparatorChar.ToString();
			fullPath = string.Join(separator, list.ToArray());
			fullPath = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath));
			return fullPath;
		}
	}
}
