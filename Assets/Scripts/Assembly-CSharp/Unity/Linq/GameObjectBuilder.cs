using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
	public class GameObjectBuilder
	{
		private readonly GameObject original;

		private readonly IEnumerable<GameObjectBuilder> children;

		public GameObjectBuilder(GameObject original, params GameObjectBuilder[] children)
			: this(original, (IEnumerable<GameObjectBuilder>)children)
		{
		}

		public GameObjectBuilder(GameObject original, IEnumerable<GameObjectBuilder> children)
		{
			this.original = original;
			this.children = children;
		}

		public GameObject Instantiate()
		{
			return Instantiate(TransformCloneType.KeepOriginal);
		}

		public GameObject Instantiate(bool setActive)
		{
			return Instantiate(TransformCloneType.KeepOriginal);
		}

		public GameObject Instantiate(TransformCloneType cloneType)
		{
			GameObject gameObject = Object.Instantiate(original);
			InstantiateChildren(gameObject, cloneType, null);
			return gameObject;
		}

		public GameObject Instantiate(TransformCloneType cloneType, bool setActive)
		{
			GameObject gameObject = Object.Instantiate(original);
			InstantiateChildren(gameObject, cloneType, setActive);
			return gameObject;
		}

		private void InstantiateChildren(GameObject root, TransformCloneType cloneType, bool? setActive)
		{
			foreach (GameObjectBuilder child in children)
			{
				GameObject root2 = root.Add(child.original, cloneType, setActive);
				child.InstantiateChildren(root2, cloneType, setActive);
			}
		}
	}
}
