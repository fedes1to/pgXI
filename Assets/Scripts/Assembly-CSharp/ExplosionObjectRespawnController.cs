using System.Collections.Generic;
using UnityEngine;

public class ExplosionObjectRespawnController : MonoBehaviour
{
	[Header("Time settings")]
	public float timeToNextRespawn;

	[Header("Object settings")]
	public GameObject explosionObjectPrefab;

	private GameObject _currentExplosionObject;

	private bool _isMultiplayerMode;

	public static List<GameObject> respawnList = new List<GameObject>();

	private void CreateExplosionObject()
	{
		if (_isMultiplayerMode)
		{
			if (PhotonNetwork.isMasterClient)
			{
				string prefabName = string.Format("ExplosionObjects/{0}", explosionObjectPrefab.name);
				_currentExplosionObject = PhotonNetwork.InstantiateSceneObject(prefabName, base.transform.position, base.transform.rotation, 0, null);
			}
			else
			{
				_currentExplosionObject = null;
			}
		}
		else
		{
			_currentExplosionObject = Object.Instantiate(explosionObjectPrefab);
		}
		if (_currentExplosionObject != null)
		{
			_currentExplosionObject.transform.parent = base.transform;
			_currentExplosionObject.transform.localPosition = Vector3.zero;
			_currentExplosionObject.transform.localRotation = Quaternion.identity;
		}
	}

	private void Start()
	{
		_isMultiplayerMode = Defs.isMulti;
		CreateExplosionObject();
		respawnList.Add(base.gameObject);
	}

	private void OnDestroy()
	{
		respawnList.Remove(base.gameObject);
	}

	public void StartProcessNewRespawn()
	{
		_currentExplosionObject = null;
		Invoke("CreateExplosionObject", timeToNextRespawn);
	}
}
