using UnityEngine;

public class BulletStackController : MonoBehaviour
{
	public static BulletStackController sharedController;

	public Transform standartBulletStack;

	public Transform redBulletStack;

	public Transform for252BulletStack;

	public Transform turquoiseBulletStack;

	public Transform greenBulletStack;

	public Transform violetBulletStack;

	public GameObject[][] bullets;

	private int[] currentIndexBullet = new int[6];

	private void Start()
	{
		sharedController = this;
		base.transform.position = Vector3.zero;
		for (int i = 0; i < 6; i++)
		{
			currentIndexBullet[i] = 0;
		}
		bullets = new GameObject[6][];
		bullets[0] = new GameObject[standartBulletStack.childCount];
		for (int j = 0; j < bullets[0].Length; j++)
		{
			bullets[0][j] = standartBulletStack.GetChild(j).gameObject;
		}
		bullets[1] = new GameObject[redBulletStack.childCount];
		for (int k = 0; k < bullets[1].Length; k++)
		{
			bullets[1][k] = redBulletStack.GetChild(k).gameObject;
		}
		bullets[2] = new GameObject[for252BulletStack.childCount];
		for (int l = 0; l < bullets[2].Length; l++)
		{
			bullets[2][l] = for252BulletStack.GetChild(l).gameObject;
		}
		bullets[3] = new GameObject[turquoiseBulletStack.childCount];
		for (int m = 0; m < bullets[3].Length; m++)
		{
			bullets[3][m] = turquoiseBulletStack.GetChild(m).gameObject;
		}
		bullets[4] = new GameObject[greenBulletStack.childCount];
		for (int n = 0; n < bullets[4].Length; n++)
		{
			bullets[4][n] = greenBulletStack.GetChild(n).gameObject;
		}
		bullets[5] = new GameObject[violetBulletStack.childCount];
		for (int num = 0; num < bullets[5].Length; num++)
		{
			bullets[5][num] = violetBulletStack.GetChild(num).gameObject;
		}
	}

	public GameObject GetCurrentBullet(int type = 0)
	{
		if (type < 0)
		{
			return null;
		}
		currentIndexBullet[type]++;
		if (currentIndexBullet[type] >= bullets[type].Length)
		{
			currentIndexBullet[type] = 0;
		}
		return bullets[type][currentIndexBullet[type]];
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}
