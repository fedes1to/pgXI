using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class meminfo
{
	public struct meminf
	{
		public int memtotal;

		public int memfree;

		public int active;

		public int inactive;

		public int cached;

		public int swapcached;

		public int swaptotal;

		public int swapfree;
	}

	public static meminf minf = default(meminf);

	private static Regex re = new Regex("\\d+");

	public static bool getMemInfo()
	{
		if (!File.Exists("/proc/meminfo"))
		{
			return false;
		}
		FileStream fileStream = new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read, FileShare.Read);
		StreamReader streamReader = new StreamReader(fileStream);
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			text = text.ToLower().Replace(" ", string.Empty);
			if (text.Contains("memtotal"))
			{
				minf.memtotal = mVal(text);
			}
			if (text.Contains("memfree"))
			{
				minf.memfree = mVal(text);
			}
			if (text.Contains("active"))
			{
				minf.active = mVal(text);
			}
			if (text.Contains("inactive"))
			{
				minf.inactive = mVal(text);
			}
			if (text.Contains("cached") && !text.Contains("swapcached"))
			{
				minf.cached = mVal(text);
			}
			if (text.Contains("swapcached"))
			{
				minf.swapcached = mVal(text);
			}
			if (text.Contains("swaptotal"))
			{
				minf.swaptotal = mVal(text);
			}
			if (text.Contains("swapfree"))
			{
				minf.swapfree = mVal(text);
			}
		}
		streamReader.Close();
		fileStream.Close();
		fileStream.Dispose();
		return true;
	}

	private static int mVal(string s)
	{
		Match match = re.Match(s);
		return int.Parse(match.Value);
	}

	public static void gc_Collect()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.lang.System");
		androidJavaClass.CallStatic("gc");
		androidJavaClass.Dispose();
	}
}
