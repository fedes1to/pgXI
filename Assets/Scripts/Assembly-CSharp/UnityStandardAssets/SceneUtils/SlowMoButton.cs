using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.SceneUtils
{
	public class SlowMoButton : MonoBehaviour
	{
		public Sprite FullSpeedTex;

		public Sprite SlowSpeedTex;

		public float fullSpeed = 1f;

		public float slowSpeed = 0.3f;

		public Button button;

		private bool m_SlowMo;

		private void Start()
		{
			m_SlowMo = false;
		}

		private void OnDestroy()
		{
			Time.timeScale = 1f;
		}

		public void ChangeSpeed()
		{
			m_SlowMo = !m_SlowMo;
			Image image = button.targetGraphic as Image;
			if (image != null)
			{
				image.sprite = ((!m_SlowMo) ? FullSpeedTex : SlowSpeedTex);
			}
			button.targetGraphic = image;
			Time.timeScale = ((!m_SlowMo) ? fullSpeed : slowSpeed);
		}
	}
}
