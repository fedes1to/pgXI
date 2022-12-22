using System.Collections;
using System.Threading;
using UnityEngine;

namespace Rilisoft
{
	public class AreaFog : AreaBase
	{
		[SerializeField]
		private float animationTime = 1f;

		[SerializeField]
		private FogSettings _settings;

		[SerializeField]
		[ReadOnly]
		private FogSettings _prevSettings;

		private CancellationTokenSource _tokenSource = new CancellationTokenSource();

		private new void Awake()
		{
			_prevSettings = new FogSettings().FromCurrent();
		}

		public override void CheckIn(GameObject to)
		{
			base.CheckIn(to);
			_tokenSource.Cancel();
			_tokenSource = new CancellationTokenSource();
			StartCoroutine(Change(_settings, animationTime, _tokenSource.Token));
		}

		public override void CheckOut(GameObject from)
		{
			base.CheckOut(from);
			_tokenSource.Cancel();
			_tokenSource = new CancellationTokenSource();
			StartCoroutine(Change(_prevSettings, animationTime, _tokenSource.Token));
		}

		private IEnumerator Change(FogSettings to, float time, CancellationToken token)
		{
			RenderSettings.fog = to.Active;
			if (RenderSettings.fog)
			{
				FogSettings fr = new FogSettings().FromCurrent();
				RenderSettings.fogMode = to.Mode;
				float elapsed = 0f;
				while (elapsed < time && !token.IsCancellationRequested)
				{
					elapsed += Time.deltaTime;
					float rate = elapsed / time;
					RenderSettings.fogStartDistance = Mathf.Lerp(fr.Start, to.Start, rate);
					RenderSettings.fogEndDistance = Mathf.Lerp(fr.End, to.End, rate);
					RenderSettings.fogColor = Color.Lerp(fr.Color, to.Color, rate);
					yield return null;
				}
			}
		}
	}
}
