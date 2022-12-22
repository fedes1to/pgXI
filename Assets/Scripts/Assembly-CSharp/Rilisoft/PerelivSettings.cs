namespace Rilisoft
{
	internal sealed class PerelivSettings
	{
		private readonly bool _enabled;

		private readonly string _imageUrl = string.Empty;

		private readonly string _redirectUrl = string.Empty;

		private readonly string _text = string.Empty;

		private readonly bool _showAlways;

		private readonly int _minLevel;

		private readonly int _maxLevel;

		private readonly bool _buttonClose;

		private readonly string _error = string.Empty;

		private static readonly PerelivSettings s_default = new PerelivSettings();

		public static PerelivSettings Default
		{
			get
			{
				return s_default;
			}
		}

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
		}

		public string ImageUrl
		{
			get
			{
				return _imageUrl;
			}
		}

		public string RedirectUrl
		{
			get
			{
				return _redirectUrl;
			}
		}

		public string Text
		{
			get
			{
				return _text;
			}
		}

		public bool ShowAlways
		{
			get
			{
				return _showAlways;
			}
		}

		public int MinLevel
		{
			get
			{
				return _minLevel;
			}
		}

		public int MaxLevel
		{
			get
			{
				return _maxLevel;
			}
		}

		public bool ButtonClose
		{
			get
			{
				return _buttonClose;
			}
		}

		public string Error
		{
			get
			{
				return _error;
			}
		}

		public PerelivSettings(bool enabled, string imageUrl, string redirectUrl, string text, bool showAlways, int minLevel, int maxLevel, bool buttonClose)
		{
			_enabled = enabled;
			_imageUrl = imageUrl ?? string.Empty;
			_redirectUrl = redirectUrl ?? string.Empty;
			_text = text ?? string.Empty;
			_showAlways = showAlways;
			_minLevel = minLevel;
			_maxLevel = maxLevel;
			_buttonClose = buttonClose;
		}

		public PerelivSettings(string error)
		{
			_error = error ?? string.Empty;
		}

		private PerelivSettings()
		{
		}
	}
}
