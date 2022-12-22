using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer : IComparable<PhotonPlayer>, IComparable<int>, IEquatable<PhotonPlayer>, IEquatable<int>
{
	private int actorID = -1;

	private string nameField = string.Empty;

	public readonly bool isLocal;

	public object TagObject;

	public int ID
	{
		get
		{
			return actorID;
		}
	}

	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			if (!isLocal)
			{
				Debug.LogError("Error: Cannot change the name of a remote player!");
			}
			else if (!string.IsNullOrEmpty(value) && !value.Equals(nameField))
			{
				nameField = value;
				PhotonNetwork.playerName = value;
			}
		}
	}

	public string userId { get; internal set; }

	public bool isMasterClient
	{
		get
		{
			return PhotonNetwork.networkingPeer.mMasterClientId == ID;
		}
	}

	public bool isInactive { get; set; }

	public Hashtable customProperties { get; internal set; }

	public Hashtable allProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(customProperties);
			hashtable[byte.MaxValue] = name;
			return hashtable;
		}
	}

	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		nameField = name;
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		InternalCacheProperties(properties);
	}

	public override bool Equals(object p)
	{
		PhotonPlayer photonPlayer = p as PhotonPlayer;
		return photonPlayer != null && GetHashCode() == photonPlayer.GetHashCode();
	}

	public override int GetHashCode()
	{
		return ID;
	}

	internal void InternalChangeLocalID(int newID)
	{
		if (!isLocal)
		{
			Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
		}
		else
		{
			actorID = newID;
		}
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties != null && properties.Count != 0 && !customProperties.Equals(properties))
		{
			if (properties.ContainsKey(byte.MaxValue))
			{
				nameField = (string)properties[byte.MaxValue];
			}
			if (properties.ContainsKey((byte)253))
			{
				userId = (string)properties[(byte)253];
			}
			if (properties.ContainsKey((byte)254))
			{
				isInactive = (bool)properties[(byte)254];
			}
			customProperties.MergeStringKeys(properties);
			customProperties.StripKeysWithNullValues();
		}
	}

	public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
	{
		if (propertiesToSet != null)
		{
			Hashtable hashtable = propertiesToSet.StripToStringKeys();
			Hashtable hashtable2 = expectedValues.StripToStringKeys();
			bool flag = hashtable2 == null || hashtable2.Count == 0;
			bool flag2 = actorID > 0 && !PhotonNetwork.offlineMode;
			if (flag2)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfActor(actorID, hashtable, hashtable2, webForward);
			}
			if (!flag2 || flag)
			{
				InternalCacheProperties(hashtable);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, this, hashtable);
			}
		}
	}

	public static PhotonPlayer Find(int ID)
	{
		if (PhotonNetwork.networkingPeer != null)
		{
			return PhotonNetwork.networkingPeer.GetPlayerWithId(ID);
		}
		return null;
	}

	public PhotonPlayer Get(int id)
	{
		return Find(id);
	}

	public PhotonPlayer GetNext()
	{
		return GetNextFor(ID);
	}

	public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		return GetNextFor(currentPlayer.ID);
	}

	public PhotonPlayer GetNextFor(int currentPlayerId)
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
		{
			return null;
		}
		Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
		int num = int.MaxValue;
		int num2 = currentPlayerId;
		foreach (int key in mActors.Keys)
		{
			if (key < num2)
			{
				num2 = key;
			}
			else if (key > currentPlayerId && key < num)
			{
				num = key;
			}
		}
		return (num == int.MaxValue) ? mActors[num2] : mActors[num];
	}

	public int CompareTo(PhotonPlayer other)
	{
		if (other == null)
		{
			return 0;
		}
		return GetHashCode().CompareTo(other.GetHashCode());
	}

	public int CompareTo(int other)
	{
		return GetHashCode().CompareTo(other);
	}

	public bool Equals(PhotonPlayer other)
	{
		if (other == null)
		{
			return false;
		}
		return GetHashCode().Equals(other.GetHashCode());
	}

	public bool Equals(int other)
	{
		return GetHashCode().Equals(other);
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(name))
		{
			return string.Format("#{0:00}{1}{2}", ID, (!isInactive) ? " " : " (inactive)", (!isMasterClient) ? string.Empty : "(master)");
		}
		return string.Format("'{0}'{1}{2}", name, (!isInactive) ? " " : " (inactive)", (!isMasterClient) ? string.Empty : "(master)");
	}

	public string ToStringFull()
	{
		return string.Format("#{0:00} '{1}'{2} {3}", ID, name, (!isInactive) ? string.Empty : " (inactive)", customProperties.ToStringFull());
	}
}
