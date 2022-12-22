using UnityEngine;

public class BlueRedButtonController : MonoBehaviour
{
	public UIButton blueButton;

	public UIButton redButton;

	public bool isBlueAvalible = true;

	public bool isRedAvalible = true;

	public int countBlue;

	public int countRed;

	private void Start()
	{
		if (!Defs.isFlag && !Defs.isCompany && !Defs.isCapturePoints)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		countBlue = 0;
		countRed = 0;
		for (int i = 0; i < Initializer.networkTables.Count; i++)
		{
			if (Initializer.networkTables[i].myCommand == 1)
			{
				countBlue++;
			}
			if (Initializer.networkTables[i].myCommand == 2)
			{
				countRed++;
			}
		}
		isBlueAvalible = true;
		isRedAvalible = true;
		if (PhotonNetwork.room != null && (countBlue >= PhotonNetwork.room.maxPlayers / 2 || countBlue - countRed > 1))
		{
			isBlueAvalible = false;
		}
		if (PhotonNetwork.room != null && (countRed >= PhotonNetwork.room.maxPlayers / 2 || countRed - countBlue > 1))
		{
			isRedAvalible = false;
		}
		if (isBlueAvalible != blueButton.isEnabled)
		{
			blueButton.isEnabled = isBlueAvalible;
		}
		if (isRedAvalible != redButton.isEnabled)
		{
			redButton.isEnabled = isRedAvalible;
		}
	}
}
