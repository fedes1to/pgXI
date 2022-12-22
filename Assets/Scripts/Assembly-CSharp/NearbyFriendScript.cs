using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class NearbyFriendScript : MonoBehaviour
{
	public UIRect nearbyFriendHeader;

	public UIRect nearbyFriendGrid;

	public UIRect otherFriendHeader;

	public UIRect otherFriendGrid;

	public bool NearbyFriendSupported
	{
		get
		{
			return BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64;
		}
	}

	private void Start()
	{
		if (!NearbyFriendSupported)
		{
			if (nearbyFriendGrid != null && otherFriendGrid != null)
			{
				otherFriendGrid.topAnchor.Set(nearbyFriendGrid.topAnchor.relative, nearbyFriendGrid.topAnchor.absolute);
			}
			if (nearbyFriendHeader != null && otherFriendHeader != null)
			{
				otherFriendHeader.topAnchor.Set(nearbyFriendHeader.topAnchor.relative, nearbyFriendHeader.topAnchor.absolute);
				otherFriendHeader.bottomAnchor.Set(nearbyFriendHeader.bottomAnchor.relative, nearbyFriendHeader.bottomAnchor.absolute);
			}
			nearbyFriendHeader.Do(delegate(UIRect h)
			{
				h.gameObject.SetActive(false);
			});
			nearbyFriendGrid.Do(delegate(UIRect g)
			{
				g.gameObject.SetActive(false);
			});
		}
	}
}
