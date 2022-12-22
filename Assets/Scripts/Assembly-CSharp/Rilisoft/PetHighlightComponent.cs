using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	public class PetHighlightComponent : MonoBehaviour
	{
		[SerializeField]
		private Texture2D _damageTexture;

		[SerializeField]
		private Renderer _rend;

		[SerializeField]
		private Color _immortalColor;

		[SerializeField]
		[Range(0f, 2f)]
		private float _splashTime = 0.3f;

		[SerializeField]
		[ReadOnly]
		private Texture _baseTexture;

		private Color _baseColor;

		[SerializeField]
		private string _shaderColorProp = "_ColorRili";

		private bool _damageCoroutineIsRunnig;

		private bool _immortalBlinkStarted;

		private void Awake()
		{
			_baseTexture = _rend.material.mainTexture;
			if (_rend.material.HasProperty(_shaderColorProp))
			{
				_baseColor = _rend.material.GetColor(_shaderColorProp);
			}
			else
			{
				Debug.LogError(string.Format("shader property '{0}' not found", _shaderColorProp));
			}
		}

		private void OnDisable()
		{
			ResetHit();
			ImmortalBlinkStop();
		}

		public void Hit()
		{
			StopCoroutine(DamageCoroutine());
			StartCoroutine(DamageCoroutine());
		}

		private void ResetHit()
		{
			_damageCoroutineIsRunnig = false;
			_rend.material.mainTexture = _baseTexture;
		}

		private IEnumerator DamageCoroutine()
		{
			if (_damageCoroutineIsRunnig)
			{
				ResetHit();
				yield return null;
			}
			_damageCoroutineIsRunnig = true;
			_rend.material.mainTexture = _damageTexture;
			yield return new WaitForSeconds(_splashTime);
			ResetHit();
		}

		public void ImmortalBlinkStart(float time)
		{
			if (!_immortalBlinkStarted)
			{
				StartCoroutine("ImmortalBlinkCoroutine");
				_immortalBlinkStarted = true;
			}
		}

		public void ImmortalBlinkStop()
		{
			StopCoroutine("ImmortalBlinkCoroutine");
			_immortalBlinkStarted = false;
			SetColor(_baseColor);
		}

		private IEnumerator ImmortalBlinkCoroutine()
		{
			float loopTime = 0.4f;
			int loopsCount = 1;
			float elapsedTime = 0f;
			while (true)
			{
				elapsedTime += Time.deltaTime;
				SetColor(Color.Lerp(t: (loopsCount % 2 == 0) ? (elapsedTime / loopTime * -1f) : (elapsedTime / loopTime), a: _baseColor, b: _immortalColor));
				if (elapsedTime > loopTime)
				{
					elapsedTime = 0f;
					loopsCount++;
				}
				yield return null;
			}
		}

		private void SetColor(Color color)
		{
			if (_rend.material.HasProperty(_shaderColorProp))
			{
				_rend.material.SetColor(_shaderColorProp, color);
			}
			else
			{
				Debug.LogError(string.Format("shader property '{0}' not found", _shaderColorProp));
			}
		}
	}
}
