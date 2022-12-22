using System.Linq;
using UnityEngine;

public class EnemyPortalStackController : MonoBehaviour
{
	public static EnemyPortalStackController sharedController;

	[SerializeField]
	[ReadOnly]
	private EnemyPortal[] _portals;

	private int currentIndex;

	private void Awake()
	{
		sharedController = this;
	}

	private void Start()
	{
		if (Device.isPixelGunLow)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public EnemyPortal GetPortal()
	{
		if (_portals == null || !_portals.Any())
		{
			SetPortals();
		}
		currentIndex++;
		if (currentIndex >= _portals.Length)
		{
			currentIndex = 0;
		}
		return _portals[currentIndex];
	}

	private void SetPortals()
	{
		_portals = GetComponentsInChildren<EnemyPortal>(true);
		EnemyPortal[] portals = _portals;
		foreach (EnemyPortal enemyPortal in portals)
		{
			if (enemyPortal.gameObject.activeInHierarchy)
			{
				enemyPortal.gameObject.SetActive(false);
			}
		}
	}
}
