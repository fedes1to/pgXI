using System;
using System.Reflection;
using UnityEngine;

public static class ConsoleProDebug
{
	private static bool _checkedConsoleClearMethod;

	private static MethodInfo _consoleClearMethod;

	private static bool _checkedConsoleWindowType;

	private static Type _consoleWindowType;

	private static MethodInfo ConsoleClearMethod
	{
		get
		{
			if (_consoleClearMethod == null || !_checkedConsoleClearMethod)
			{
				_checkedConsoleClearMethod = true;
				if (ConsoleWindowType == null)
				{
					return null;
				}
				_consoleClearMethod = ConsoleWindowType.GetMethod("ClearEntries", BindingFlags.Static | BindingFlags.Public);
			}
			return _consoleClearMethod;
		}
	}

	private static Type ConsoleWindowType
	{
		get
		{
			if (_consoleWindowType == null || !_checkedConsoleWindowType)
			{
				_checkedConsoleWindowType = true;
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					Type[] types = assemblies[i].GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						if (types[j].Name == "ConsolePro3Window")
						{
							_consoleWindowType = types[j];
						}
					}
				}
			}
			return _consoleWindowType;
		}
	}

	public static void Clear()
	{
		if (ConsoleClearMethod != null)
		{
			ConsoleClearMethod.Invoke(null, null);
		}
	}

	public static void LogToFilter(string inLog, string inFilterName)
	{
		Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"Filter\" \"name\":\"" + inFilterName + "\"}");
	}

	public static void Watch(string inName, string inValue)
	{
		Debug.Log(inName + " : " + inValue + "\nCPAPI:{\"cmd\":\"Watch\" \"name\":\"" + inName + "\"}");
	}
}
