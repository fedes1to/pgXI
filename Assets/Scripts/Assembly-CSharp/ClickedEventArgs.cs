using System;

public sealed class ClickedEventArgs : EventArgs
{
	private readonly string _id;

	public string Id
	{
		get
		{
			return _id;
		}
	}

	public ClickedEventArgs(string id)
	{
		_id = id ?? string.Empty;
	}
}
