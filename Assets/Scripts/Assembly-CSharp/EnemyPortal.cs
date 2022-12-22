using System;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{
	public event Action OnHided = delegate
	{
	};

	public void OnAnimationOff()
	{
		ChangeVisibleState(false, this.OnHided);
	}

	public void Show(Vector3 position)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(position, Vector3.down, out hitInfo))
		{
			Debug.DrawLine(position, hitInfo.point, Color.blue);
			base.transform.position = hitInfo.point;
		}
		ChangeVisibleState(true);
	}

	private void ChangeVisibleState(bool state, Action onComplete = null)
	{
		base.gameObject.SetActive(state);
		if (onComplete != null)
		{
			onComplete();
		}
	}
}
