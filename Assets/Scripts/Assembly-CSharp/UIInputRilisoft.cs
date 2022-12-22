public class UIInputRilisoft : UIInput
{
	public delegate void OnFocus();

	public delegate void OnFocusLost();

	public OnFocus onFocus;

	public OnFocusLost onFocusLost;

	protected override void OnSelect(bool isSelected)
	{
		base.OnSelect(isSelected);
		if (isSelected && onFocus != null)
		{
			onFocus();
		}
		else if (!isSelected && onFocusLost != null)
		{
			onFocusLost();
		}
	}
}
