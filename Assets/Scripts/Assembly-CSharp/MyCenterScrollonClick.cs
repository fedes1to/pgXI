public class MyCenterScrollonClick : UIDragScrollView
{
	private MyCenterOnChild center;

	private void Awake()
	{
		if (center == null)
		{
			center = NGUITools.FindInParents<MyCenterOnChild>(base.gameObject);
		}
	}

	private void OnClick()
	{
		center.CenterOn(base.transform);
	}
}
