using UnityEngine;

public class PlayerDeadStackController : MonoBehaviour
{
	public static PlayerDeadStackController sharedController;

	public PlayerDeadController[] playerDeads;

	public GameObject playerDeadObject;

	public GameObject playerDeadObjectLow;

	private int currentIndexHole;

	private void Start()
	{
		sharedController = this;
		Transform transform = base.transform;
		transform.position = Vector3.zero;
		playerDeads = new PlayerDeadController[10];
		for (int i = 0; i < playerDeads.Length; i++)
		{
			GameObject gameObject = ((!Device.isPixelGunLow) ? Object.Instantiate(playerDeadObject) : Object.Instantiate(playerDeadObjectLow));
			gameObject.transform.parent = base.transform;
			playerDeads[i] = gameObject.GetComponent<PlayerDeadController>();
		}
		Object.Destroy(playerDeadObjectLow);
		Object.Destroy(playerDeadObject);
	}

	public PlayerDeadController GetCurrentParticle(bool _isUseMine)
	{
		bool flag = true;
		do
		{
			currentIndexHole++;
			if (currentIndexHole >= playerDeads.Length)
			{
				if (!flag)
				{
					return null;
				}
				currentIndexHole = 0;
				flag = false;
			}
		}
		while (playerDeads[currentIndexHole].isUseMine && !_isUseMine);
		return playerDeads[currentIndexHole];
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}
