using System.Globalization;
using UnityEngine;

[DisallowMultipleComponent]
internal sealed class WavesSurvivedStat : MonoBehaviour
{
	internal static int SurvivedWaveCount { get; set; }

	private WavesSurvivedStat()
	{
	}

	private void Start()
	{
		UILabel component = GetComponent<UILabel>();
		if (component != null)
		{
			component.text = SurvivedWaveCount.ToString(CultureInfo.InvariantCulture);
		}
	}
}
