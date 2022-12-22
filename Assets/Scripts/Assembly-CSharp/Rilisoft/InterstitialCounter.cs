using System.Globalization;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class InterstitialCounter
	{
		private int _realInterstitialCount;

		private int _fakeInterstitialCount;

		private static readonly InterstitialCounter s_instance = new InterstitialCounter();

		public static InterstitialCounter Instance
		{
			get
			{
				return s_instance;
			}
		}

		public int RealInterstitialCount
		{
			get
			{
				return _realInterstitialCount;
			}
		}

		public int FakeInterstitialCount
		{
			get
			{
				return _fakeInterstitialCount;
			}
		}

		public int TotalInterstitialCount
		{
			get
			{
				return _realInterstitialCount + _fakeInterstitialCount;
			}
		}

		private InterstitialCounter()
		{
		}

		public void Reset()
		{
			_realInterstitialCount = 0;
			_fakeInterstitialCount = 0;
		}

		public void IncrementRealInterstitialCount()
		{
			_realInterstitialCount++;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log(ToString());
			}
		}

		public void IncrementFakeInterstitialCount()
		{
			_fakeInterstitialCount++;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log(ToString());
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "realInterstitialCount: {0}, fakeInterstitialCount: {1}", RealInterstitialCount, FakeInterstitialCount);
		}
	}
}
