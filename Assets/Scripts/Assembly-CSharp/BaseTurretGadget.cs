public class BaseTurretGadget : TurretGadget
{
	public BaseTurretGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void Use()
	{
		base.Use();
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.ShowTurretInterface(GadgetsInfo.BaseName(Info.Id));
		}
	}
}
