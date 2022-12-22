using Rilisoft;
using UnityEngine;

internal sealed class CoinConfigurator : MonoBehaviour
{
	[SerializeField]
	private VirtualCurrencyBonusType bonusType;

	public bool CoinIsPresent = true;

	public Vector3 pos = default(Vector3);

	public Transform coinCreatePoint;

	public VirtualCurrencyBonusType BonusType
	{
		get
		{
			return bonusType;
		}
	}
}
