using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
	public static class GameObjectExtensions
	{
		public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.Ancestors())
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source, string name)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.Ancestors(name))
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.AncestorsAndSelf())
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source, string name)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.AncestorsAndSelf(name))
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.Descendants())
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source, string name)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.Descendants(name))
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.DescendantsAndSelf())
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source, string name)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.DescendantsAndSelf(name))
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.Children())
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source, string name)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.Children(name))
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.ChildrenAndSelf())
				{
					yield return item2;
				}
			}
		}

		public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source, string name)
		{
			foreach (GameObject item in source)
			{
				foreach (GameObject item2 in item.ChildrenAndSelf(name))
				{
					yield return item2;
				}
			}
		}

		public static void Destroy(this IEnumerable<GameObject> source, bool useDestroyImmediate = false)
		{
			foreach (GameObject item in new List<GameObject>(source))
			{
				item.Destroy(useDestroyImmediate);
			}
		}

		public static IEnumerable<T> OfComponent<T>(this IEnumerable<GameObject> source) where T : Component
		{
			foreach (GameObject item in source)
			{
				T component = item.GetComponent<T>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					yield return component;
				}
			}
		}

		public static GameObject Add(this GameObject parent, GameObject childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (childOriginal == null)
			{
				throw new ArgumentNullException("childOriginal");
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(childOriginal);
			Transform transform = gameObject.transform;
			RectTransform rectTransform = transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(parent.transform, false);
			}
			else
			{
				Transform transform3 = (transform.parent = parent.transform);
				switch (cloneType)
				{
				case TransformCloneType.FollowParent:
					transform.localPosition = transform3.localPosition;
					transform.localScale = transform3.localScale;
					transform.localRotation = transform3.localRotation;
					break;
				case TransformCloneType.Origin:
					transform.localPosition = Vector3.zero;
					transform.localScale = Vector3.one;
					transform.localRotation = Quaternion.identity;
					break;
				case TransformCloneType.KeepOriginal:
				{
					Transform transform4 = childOriginal.transform;
					transform.localPosition = transform4.localPosition;
					transform.localScale = transform4.localScale;
					transform.localRotation = transform4.localRotation;
					break;
				}
				}
			}
			gameObject.layer = parent.layer;
			if (setActive.HasValue)
			{
				gameObject.SetActive(setActive.Value);
			}
			if (specifiedName != null)
			{
				gameObject.name = specifiedName;
			}
			return gameObject;
		}

		public static List<GameObject> Add(this GameObject parent, IEnumerable<GameObject> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (childOriginals == null)
			{
				throw new ArgumentNullException("childOriginals");
			}
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject childOriginal in childOriginals)
			{
				GameObject item = parent.Add(childOriginal, cloneType, setActive, specifiedName);
				list.Add(item);
			}
			return list;
		}

		public static GameObject AddFirst(this GameObject parent, GameObject childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
		{
			GameObject gameObject = parent.Add(childOriginal, cloneType, setActive, specifiedName);
			gameObject.transform.SetAsFirstSibling();
			return gameObject;
		}

		public static List<GameObject> AddFirst(this GameObject parent, IEnumerable<GameObject> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
		{
			List<GameObject> list = parent.Add(childOriginals, cloneType, setActive, specifiedName);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num].transform.SetAsFirstSibling();
			}
			return list;
		}

		public static GameObject AddBeforeSelf(this GameObject parent, GameObject childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex();
			GameObject gameObject2 = gameObject.Add(childOriginal, cloneType, setActive, specifiedName);
			gameObject2.transform.SetSiblingIndex(siblingIndex);
			return gameObject2;
		}

		public static List<GameObject> AddBeforeSelf(this GameObject parent, IEnumerable<GameObject> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex();
			List<GameObject> list = gameObject.Add(childOriginals, cloneType, setActive, specifiedName);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num].transform.SetSiblingIndex(siblingIndex);
			}
			return list;
		}

		public static GameObject AddAfterSelf(this GameObject parent, GameObject childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex() + 1;
			GameObject gameObject2 = gameObject.Add(childOriginal, cloneType, setActive, specifiedName);
			gameObject2.transform.SetSiblingIndex(siblingIndex);
			return gameObject2;
		}

		public static List<GameObject> AddAfterSelf(this GameObject parent, IEnumerable<GameObject> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex() + 1;
			List<GameObject> list = gameObject.Add(childOriginals, cloneType, setActive, specifiedName);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num].transform.SetSiblingIndex(siblingIndex);
			}
			return list;
		}

		public static GameObject MoveToLast(this GameObject parent, GameObject child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (child == null)
			{
				throw new ArgumentNullException("child");
			}
			Transform transform = child.transform;
			RectTransform rectTransform = transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(parent.transform, false);
			}
			else
			{
				Transform transform3 = (transform.parent = parent.transform);
				switch (moveType)
				{
				case TransformMoveType.FollowParent:
					transform.localPosition = transform3.localPosition;
					transform.localScale = transform3.localScale;
					transform.localRotation = transform3.localRotation;
					break;
				case TransformMoveType.Origin:
					transform.localPosition = Vector3.zero;
					transform.localScale = Vector3.one;
					transform.localRotation = Quaternion.identity;
					break;
				}
			}
			child.layer = parent.layer;
			if (setActive.HasValue)
			{
				child.SetActive(setActive.Value);
			}
			return child;
		}

		public static List<GameObject> MoveToLast(this GameObject parent, IEnumerable<GameObject> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (childs == null)
			{
				throw new ArgumentNullException("childs");
			}
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject child in childs)
			{
				GameObject item = parent.MoveToLast(child, moveType);
				list.Add(item);
			}
			return list;
		}

		public static GameObject MoveToFirst(this GameObject parent, GameObject child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
		{
			parent.MoveToLast(child, moveType, setActive);
			child.transform.SetAsFirstSibling();
			return child;
		}

		public static List<GameObject> MoveToFirst(this GameObject parent, IEnumerable<GameObject> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
		{
			List<GameObject> list = parent.MoveToLast(childs, moveType, setActive);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num].transform.SetAsFirstSibling();
			}
			return list;
		}

		public static GameObject MoveToBeforeSelf(this GameObject parent, GameObject child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex();
			gameObject.MoveToLast(child, moveType, setActive);
			child.transform.SetSiblingIndex(siblingIndex);
			return child;
		}

		public static List<GameObject> MoveToBeforeSelf(this GameObject parent, IEnumerable<GameObject> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex();
			List<GameObject> list = gameObject.MoveToLast(childs, moveType, setActive);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num].transform.SetSiblingIndex(siblingIndex);
			}
			return list;
		}

		public static GameObject MoveToAfterSelf(this GameObject parent, GameObject child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex() + 1;
			gameObject.MoveToLast(child, moveType, setActive);
			child.transform.SetSiblingIndex(siblingIndex);
			return child;
		}

		public static List<GameObject> MoveToAfterSelf(this GameObject parent, IEnumerable<GameObject> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
		{
			GameObject gameObject = parent.Parent();
			if (gameObject == null)
			{
				throw new InvalidOperationException("The parent root is null");
			}
			int siblingIndex = parent.transform.GetSiblingIndex() + 1;
			List<GameObject> list = gameObject.MoveToLast(childs, moveType, setActive);
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num].transform.SetSiblingIndex(siblingIndex);
			}
			return list;
		}

		public static void Destroy(this GameObject self, bool useDestroyImmediate = false)
		{
			if (!(self == null))
			{
				self.SetActive(false);
				self.transform.parent = null;
				self.transform.SetParent(null);
				if (useDestroyImmediate)
				{
					UnityEngine.Object.DestroyImmediate(self);
				}
				else
				{
					UnityEngine.Object.Destroy(self);
				}
			}
		}

		public static GameObject Parent(this GameObject origin)
		{
			if (origin == null)
			{
				return null;
			}
			Transform parent = origin.transform.parent;
			if (parent == null)
			{
				return null;
			}
			return parent.gameObject;
		}

		public static GameObject Child(this GameObject origin, string name)
		{
			if (origin == null)
			{
				return null;
			}
			Transform transform = origin.transform.FindChild(name);
			if (transform == null)
			{
				return null;
			}
			return transform.gameObject;
		}

		public static IEnumerable<GameObject> Children(this GameObject origin)
		{
			return origin.ChildrenCore(null, false);
		}

		public static IEnumerable<GameObject> Children(this GameObject origin, string name)
		{
			return origin.ChildrenCore(name, false);
		}

		public static IEnumerable<GameObject> ChildrenAndSelf(this GameObject origin)
		{
			return origin.ChildrenCore(null, true);
		}

		public static IEnumerable<GameObject> ChildrenAndSelf(this GameObject origin, string name)
		{
			return origin.ChildrenCore(name, true);
		}

		private static IEnumerable<GameObject> ChildrenCore(this GameObject origin, string nameFilter, bool withSelf)
		{
			if (origin == null)
			{
				yield break;
			}
			if (withSelf && (nameFilter == null || origin.name == nameFilter))
			{
				yield return origin;
			}
			foreach (Transform child in origin.transform)
			{
				if (nameFilter == null || child.name == nameFilter)
				{
					yield return child.gameObject;
				}
			}
		}

		public static IEnumerable<GameObject> Ancestors(this GameObject origin)
		{
			return AncestorsCore(origin, null, false);
		}

		public static IEnumerable<GameObject> Ancestors(this GameObject origin, string name)
		{
			return AncestorsCore(origin, null, false);
		}

		public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin)
		{
			return AncestorsCore(origin, null, true);
		}

		public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin, string name)
		{
			return AncestorsCore(origin, name, true);
		}

		private static IEnumerable<GameObject> AncestorsCore(GameObject origin, string nameFilter, bool withSelf)
		{
			if (origin == null)
			{
				yield break;
			}
			if (withSelf && (nameFilter == null || origin.name == nameFilter))
			{
				yield return origin;
			}
			Transform parentTransform = origin.transform.parent;
			while (parentTransform != null)
			{
				if (nameFilter == null || parentTransform.name == nameFilter)
				{
					yield return parentTransform.gameObject;
				}
				parentTransform = parentTransform.parent;
			}
		}

		public static IEnumerable<GameObject> Descendants(this GameObject origin)
		{
			return origin.DescendantsCore(null, false);
		}

		public static IEnumerable<GameObject> Descendants(this GameObject origin, string name)
		{
			return origin.DescendantsCore(name, false);
		}

		public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin)
		{
			return origin.DescendantsCore(null, true);
		}

		public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin, string name)
		{
			return origin.DescendantsCore(name, true);
		}

		private static IEnumerable<GameObject> DescendantsCore(this GameObject origin, string nameFilter, bool withSelf)
		{
			if (origin == null)
			{
				yield break;
			}
			if (withSelf && (nameFilter == null || origin.name == nameFilter))
			{
				yield return origin;
			}
			foreach (Transform item in origin.transform)
			{
				foreach (GameObject child in item.gameObject.DescendantsCore(nameFilter, true))
				{
					if (nameFilter == null || child.name == nameFilter)
					{
						yield return child.gameObject;
					}
				}
			}
		}

		public static IEnumerable<GameObject> BeforeSelf(this GameObject origin)
		{
			return origin.BeforeSelfCore(null, false);
		}

		public static IEnumerable<GameObject> BeforeSelf(this GameObject origin, string name)
		{
			return origin.BeforeSelfCore(name, false);
		}

		public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin)
		{
			return origin.BeforeSelfCore(null, true);
		}

		public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin, string name)
		{
			return origin.BeforeSelfCore(name, true);
		}

		private static IEnumerable<GameObject> BeforeSelfCore(this GameObject origin, string nameFilter, bool withSelf)
		{
			if (origin == null)
			{
				yield break;
			}
			Transform parent = origin.transform.parent;
			if (!(parent == null))
			{
				foreach (Transform item in parent.transform)
				{
					GameObject go = item.gameObject;
					if (go == origin)
					{
						break;
					}
					if (nameFilter == null || item.name == nameFilter)
					{
						yield return go;
					}
				}
			}
			if (withSelf && (nameFilter == null || origin.name == nameFilter))
			{
				yield return origin;
			}
		}

		public static IEnumerable<GameObject> AfterSelf(this GameObject origin)
		{
			return origin.AfterSelfCore(null, false);
		}

		public static IEnumerable<GameObject> AfterSelf(this GameObject origin, string name)
		{
			return origin.AfterSelfCore(name, false);
		}

		public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin)
		{
			return origin.AfterSelfCore(null, true);
		}

		public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin, string name)
		{
			return origin.AfterSelfCore(name, true);
		}

		private static IEnumerable<GameObject> AfterSelfCore(this GameObject origin, string nameFilter, bool withSelf)
		{
			if (origin == null)
			{
				yield break;
			}
			if (withSelf && (nameFilter == null || origin.name == nameFilter))
			{
				yield return origin;
			}
			Transform parent = origin.transform.parent;
			if (parent == null)
			{
				yield break;
			}
			int index = origin.transform.GetSiblingIndex() + 1;
			Transform parentTransform = parent.transform;
			for (int count = parentTransform.childCount; index < count; index++)
			{
				GameObject target = parentTransform.GetChild(index).gameObject;
				if (nameFilter == null || target.name == nameFilter)
				{
					yield return target;
				}
			}
		}
	}
}
