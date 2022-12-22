using System.Collections.Generic;
using System.Text.RegularExpressions;
using Rilisoft.MiniJson;
using UnityEngine;

public class FilterBadWorld
{
	private const string CensoredText = "***";

	private const string PatternTemplate = "\\b({0})(s?)\\b";

	private const RegexOptions Options = RegexOptions.IgnoreCase;

	private static string[] badWordsConst = new string[222]
	{
		"drugs", "drugz", "alcohol", "penis", "vagina", "sexx", "sexxy", "boobs", "cumshot", "facial",
		"masturbate", "nipples", "orgasm", "slut", "porn", "porno", "pornography", "ass", "arse", "assbag",
		"assbandit", "assbanger", "assbite", "asscock", "assfuck", "asshead", "asshole", "asshopper", "asslicker", "assshole",
		"asswipe", "bampot", "bastard", "beaner", "bitch", "blow job", "blowjob", "boner", "brotherfucker", "bullshit",
		"butt plug", "butt-pirate", "buttfucker", "camel toe", "carpetmuncher", "chink", "choad", "chode", "clit", "cock",
		"cockbite", "cockface", "cockfucker", "cockmaster", "cockmongruel", "cockmuncher", "cocksmoker", "cocksucker", "coon", "cooter",
		"cracker", "cum", "cumtart", "cunnilingus", "cunt", "cunthole", "damn", "deggo", "dick", "dickbag",
		"dickhead", "dickhole", "dicks", "dickweed", "dickwod", "dildo", "dipshit", "dookie", "douche", "douchebag",
		"douchewaffle", "dumass", "dumb ass", "dumbass", "dumbfuck", "dumbshit", "dyke", "fag", "fagbag", "fagfucker",
		"faggit", "faggot", "fagtard", "fatass", "fellatio", "fuck", "fuckass", "fucked", "fucker", "fuckface",
		"fuckhead", "fuckhole", "fuckin", "fucking", "fucknut", "fucks", "fuckstick", "fucktard", "fuckup", "fuckwad",
		"fuckwit", "fudgepacker", "gay", "gaydo", "gaytard", "gaywad", "goddamn", "goddamnit", "gooch", "gook",
		"gringo", "guido", "hard on", "heeb", "hell", "ho", "homo", "homodumbshit", "honkey", "humping",
		"jackass", "jap", "jerk off", "jigaboo", "jizz", "jungle bunny", "kike", "kooch", "kootch", "kyke",
		"lesbian", "lesbo", "lezzie", "mcfagget", "mick", "minge", "mothafucka", "motherfucker", "motherfucking", "muff",
		"negro", "nigga", "nigger", "niglet", "nut sack", "nutsack", "paki", "panooch", "pecker", "peckerhead",
		"penis", "piss", "pissed", "pissed off", "pollock", "poon", "poonani", "poonany", "porch monkey", "porchmonkey",
		"prick", "punta", "pussy", "pussylicking", "puto", "queef", "queer", "queerbait", "renob", "rimjob",
		"sand nigger", "sandnigger", "schlong", "scrote", "shit", "shitcunt", "shitdick", "shitface", "shitfaced", "shithead",
		"shitter", "shittiest", "shitting", "shitty", "skank", "skeet", "slut", "slutbag", "snatch", "spic",
		"spick", "splooge", "tard", "testicle", "thundercunt", "tit", "titfuck", "tits", "twat", "twatlips",
		"twats", "twatwaffle", "va-j-j", "vag", "vjayjay", "wank", "wetback", "whore", "whorebag", "wop",
		"sex", "sexy"
	};

	private static string[] badSymbolsConst = new string[2] { "卐", "卍" };

	private static string[] badWords;

	private static string[] badSymbols;

	private static bool badArraysInit = false;

	public static void InitBadLists()
	{
		List<object> list = new List<object>();
		List<object> list2 = new List<object>();
		if (PlayerPrefs.HasKey("PixelFilterWordsKey"))
		{
			list = Json.Deserialize(PlayerPrefs.GetString("PixelFilterWordsKey")) as List<object>;
		}
		if (PlayerPrefs.HasKey("PixelFilterSymbolsKey"))
		{
			list2 = Json.Deserialize(PlayerPrefs.GetString("PixelFilterSymbolsKey")) as List<object>;
		}
		if (list != null)
		{
			badWords = new string[badWordsConst.Length + list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				badWords[badWordsConst.Length + i] = (string)list[i];
			}
		}
		else
		{
			badWords = new string[badWordsConst.Length];
		}
		badWordsConst.CopyTo(badWords, 0);
		if (list2 != null)
		{
			badSymbols = new string[badSymbolsConst.Length + list2.Count];
			for (int j = 0; j < list2.Count; j++)
			{
				badSymbols[badSymbolsConst.Length + j] = (string)list2[j];
			}
		}
		else
		{
			badSymbols = new string[badSymbolsConst.Length];
		}
		badSymbolsConst.CopyTo(badSymbols, 0);
		badArraysInit = true;
	}

	public static string FilterString(string inputStr)
	{
		if (!badArraysInit)
		{
			InitBadLists();
		}
		inputStr = NGUIText.StripSymbols(inputStr);
		string[] array = new string[19]
		{
			".", ",", "%", "!", "@", "#", "$", "*", "&", ";",
			":", "?", "/", "<", ">", "|", "-", "_", "\""
		};
		string text = inputStr;
		string text2 = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text = text.Replace(array[i], " ");
		}
		text = text.ToLower();
		int num = 0;
		for (int num2 = text.IndexOf(" ", num); num2 != -1; num2 = ((num > text.Length - 1) ? (-1) : text.IndexOf(" ", num)))
		{
			text2 = ((!scanMatInWold(text.Substring(num, num2 - num))) ? (text2 + inputStr.Substring(num, num2 - num + 1)) : (text2 + "***" + inputStr.Substring(num2, 1)));
			num = num2 + 1;
		}
		if (num < text.Length)
		{
			text2 = ((!scanMatInWold(text.Substring(num, text.Length - num))) ? (text2 + inputStr.Substring(num, text.Length - num)) : (text2 + "***"));
		}
		for (int j = 0; j < badSymbols.Length; j++)
		{
			text2 = text2.Replace(badSymbols[j], "*");
		}
		return text2;
	}

	private static bool scanMatInWold(string str)
	{
		if (str.Length < 3)
		{
			return false;
		}
		string[] array = badWords;
		foreach (string text in array)
		{
			if (text.Equals(str))
			{
				return true;
			}
		}
		return false;
	}
}
