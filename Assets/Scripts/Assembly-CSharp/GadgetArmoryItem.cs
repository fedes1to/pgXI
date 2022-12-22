using System.Collections.Generic;
using UnityEngine;

public class GadgetArmoryItem : MonoBehaviour
{
	public bool isReplaceOnlyHands = true;

	public GameObject gadgetPoint;

	public List<GameObject> noFillPersSkinObjects;

	[Header("For ReplaceOnlyHands")]
	public Transform LeftArmorHand;

	public Transform RightArmorHand;
}
