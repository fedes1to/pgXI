using System;
using Rilisoft;

internal sealed class TrafficForwardingInfo : EventArgs
{
	private static readonly Lazy<TrafficForwardingInfo> _disabledInstance = new Lazy<TrafficForwardingInfo>(() => new TrafficForwardingInfo(null, 0, 31));

	private readonly int _minLevel;

	private readonly int _maxLevel;

	private readonly string _url;

	public static TrafficForwardingInfo DisabledInstance
	{
		get
		{
			return _disabledInstance.Value;
		}
	}

	public bool Enabled
	{
		get
		{
			return !string.IsNullOrEmpty(_url);
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

	public string Url
	{
		get
		{
			return _url;
		}
	}

	public TrafficForwardingInfo(string url, int minLevel, int maxLevel)
	{
		_url = url;
		_minLevel = minLevel;
		_maxLevel = maxLevel;
	}
}
