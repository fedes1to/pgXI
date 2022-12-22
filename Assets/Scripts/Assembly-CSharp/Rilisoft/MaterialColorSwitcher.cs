using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[RequireComponent(typeof(Renderer))]
	public class MaterialColorSwitcher : MonoBehaviour
	{
		public List<Color> Colors = new List<Color>();

		public float ToColorTime = 1f;

		private Material _mat;

		private int _colorIdx = -1;

		private bool _changed = true;

		private void Awake()
		{
			_mat = GetComponent<Renderer>().material;
		}

		private void OnEnable()
		{
			StopAllCoroutines();
			_changed = true;
		}

		private void Update()
		{
			if (_changed)
			{
				_changed = false;
				_colorIdx = ((Colors.Count - 1 > _colorIdx) ? (_colorIdx + 1) : 0);
				StartCoroutine(ChangeColor(_colorIdx, ToColorTime));
			}
		}

		private IEnumerator ChangeColor(int toIdx, float time)
		{
			Color startColor = _mat.color;
			Color color = Colors[toIdx];
			float elapsed = 0f;
			while (elapsed < time)
			{
				elapsed += Time.deltaTime;
				_mat.color = Color.Lerp(startColor, color, elapsed / time);
				yield return null;
			}
			_changed = true;
		}
	}
}
