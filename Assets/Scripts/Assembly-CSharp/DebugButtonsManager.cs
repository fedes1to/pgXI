using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugButtonsManager : MonoBehaviour
{
	private class TopBarButton
	{
		public bool NeedShow = true;

		public string Text { get; private set; }

		public int Width { get; private set; }

		public Action Act { get; private set; }

		public TopBarButton(string text, int width, Action act)
		{
			Text = text;
			Width = width;
			Act = act;
		}
	}

	private static DebugButtonsManager _instance;

	private static bool _topPanelOpened = true;

	private static readonly List<TopBarButton> _tbButtons = new List<TopBarButton>();

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public static void ShowTopBarButton(string text, int width, Action onClickAction)
	{
		if (_instance == null)
		{
			GameObject gameObject = new GameObject("DebugButtonsManager");
			_instance = gameObject.AddComponent<DebugButtonsManager>();
		}
		TopBarButton topBarButton = _tbButtons.FirstOrDefault((TopBarButton b) => b.Text == text);
		if (topBarButton != null)
		{
			topBarButton.NeedShow = true;
			return;
		}
		TopBarButton item = new TopBarButton(text, width, onClickAction);
		_tbButtons.Add(item);
	}
}
