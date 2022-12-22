using UnityEngine;

public class GameDebugConsole : MonoBehaviour
{
	public class TextStyle
	{
		public const string bold = "b";

		public const string italic = "i";

		public const string strike = "s";

		public const string underline = "u";

		public const string sup = "sup";

		public const string sub = "sub";
	}

	public UILabel logTextLabel;

	public static GameDebugConsole Instance { get; private set; }

	public static void AddLogLine(string log)
	{
	}

	public static void AddLogLine(string log, Color logColor)
	{
	}

	public static void AddLogLine(string log, string styleCode)
	{
	}

	public static void AddLogLine(string log, Color logColor, string styleCode)
	{
	}

	public void Clear()
	{
	}

	public void ChangeFontSize(int newSize)
	{
	}
}
