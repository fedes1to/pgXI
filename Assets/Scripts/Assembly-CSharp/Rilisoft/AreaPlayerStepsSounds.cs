using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class AreaPlayerStepsSounds : AreaBase
	{
		[SerializeField]
		private PlayerStepsSoundsData _sounds;

		[SerializeField]
		[ReadOnly]
		private PlayerStepsSoundsData _soundsOriginal;

		private static readonly Dictionary<int, SkinName> _soundsComponents = new Dictionary<int, SkinName>();

		public override void CheckIn(GameObject to)
		{
			base.CheckIn(to);
		}

		public override void CheckOut(GameObject from)
		{
			base.CheckOut(from);
		}

		private SkinName GetSoundsComponent(GameObject go)
		{
			int hashCode = go.GetHashCode();
			if (_soundsComponents.ContainsKey(hashCode))
			{
				return _soundsComponents[hashCode];
			}
			SkinName componentInChildren = go.Ancestors().First((GameObject a) => a.Parent() == null).GetComponentInChildren<SkinName>();
			_soundsComponents.Add(hashCode, componentInChildren);
			return componentInChildren;
		}
	}
}
