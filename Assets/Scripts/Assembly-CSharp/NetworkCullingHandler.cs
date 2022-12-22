using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkCullingHandler : MonoBehaviour
{
	private int orderIndex;

	private CullArea cullArea;

	private List<int> previousActiveCells;

	private List<int> activeCells;

	private PhotonView pView;

	private Vector3 lastPosition;

	private Vector3 currentPosition;

	private void OnEnable()
	{
		if (pView == null)
		{
			pView = GetComponent<PhotonView>();
			if (!pView.isMine)
			{
				return;
			}
		}
		if (cullArea == null)
		{
			cullArea = Object.FindObjectOfType<CullArea>();
		}
		previousActiveCells = new List<int>(0);
		activeCells = new List<int>(0);
		currentPosition = (lastPosition = base.transform.position);
	}

	private void Start()
	{
		if (pView.isMine && PhotonNetwork.inRoom)
		{
			if (cullArea.NumberOfSubdivisions == 0)
			{
				pView.group = cullArea.FIRST_GROUP_ID;
				PhotonNetwork.SetReceivingEnabled(cullArea.FIRST_GROUP_ID, true);
				PhotonNetwork.SetSendingEnabled(cullArea.FIRST_GROUP_ID, true);
			}
			else
			{
				CheckGroupsChanged();
				InvokeRepeating("UpdateActiveGroup", 0f, 1f / (float)PhotonNetwork.sendRateOnSerialize);
			}
		}
	}

	private void Update()
	{
		if (pView.isMine)
		{
			lastPosition = currentPosition;
			currentPosition = base.transform.position;
			if (currentPosition != lastPosition)
			{
				CheckGroupsChanged();
			}
		}
	}

	private void OnDisable()
	{
		CancelInvoke();
	}

	private void CheckGroupsChanged()
	{
		if (cullArea.NumberOfSubdivisions == 0)
		{
			return;
		}
		previousActiveCells = new List<int>(activeCells);
		activeCells = cullArea.GetActiveCells(base.transform.position);
		if (activeCells.Count != previousActiveCells.Count)
		{
			UpdateInterestGroups();
			return;
		}
		foreach (int activeCell in activeCells)
		{
			if (!previousActiveCells.Contains(activeCell))
			{
				UpdateInterestGroups();
				break;
			}
		}
	}

	private void UpdateInterestGroups()
	{
		foreach (int previousActiveCell in previousActiveCells)
		{
			PhotonNetwork.SetReceivingEnabled(previousActiveCell, false);
			PhotonNetwork.SetSendingEnabled(previousActiveCell, false);
		}
		foreach (int activeCell in activeCells)
		{
			PhotonNetwork.SetReceivingEnabled(activeCell, true);
			PhotonNetwork.SetSendingEnabled(activeCell, true);
		}
	}

	private void UpdateActiveGroup()
	{
		while (activeCells.Count <= cullArea.NumberOfSubdivisions)
		{
			activeCells.Add(cullArea.FIRST_GROUP_ID);
		}
		if (cullArea.NumberOfSubdivisions == 1)
		{
			orderIndex = ++orderIndex % cullArea.SUBDIVISION_FIRST_LEVEL_ORDER.Length;
			pView.group = activeCells[cullArea.SUBDIVISION_FIRST_LEVEL_ORDER[orderIndex]];
		}
		else if (cullArea.NumberOfSubdivisions == 2)
		{
			orderIndex = ++orderIndex % cullArea.SUBDIVISION_SECOND_LEVEL_ORDER.Length;
			pView.group = activeCells[cullArea.SUBDIVISION_SECOND_LEVEL_ORDER[orderIndex]];
		}
		else if (cullArea.NumberOfSubdivisions == 3)
		{
			orderIndex = ++orderIndex % cullArea.SUBDIVISION_THIRD_LEVEL_ORDER.Length;
			pView.group = activeCells[cullArea.SUBDIVISION_THIRD_LEVEL_ORDER[orderIndex]];
		}
	}

	private void OnGUI()
	{
		if (!pView.isMine)
		{
			return;
		}
		string text = "Inside cells:\n";
		string text2 = "Subscribed cells:\n";
		for (int i = 0; i < activeCells.Count; i++)
		{
			if (i <= cullArea.NumberOfSubdivisions)
			{
				text = text + activeCells[i] + "  ";
			}
			text2 = text2 + activeCells[i] + "  ";
		}
		GUI.Label(new Rect(20f, (float)Screen.height - 100f, 200f, 40f), "<color=white>" + text + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
		GUI.Label(new Rect(20f, (float)Screen.height - 60f, 200f, 40f), "<color=white>" + text2 + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
	}
}
