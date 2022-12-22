using System;
using System.Text;

namespace Rilisoft
{
	public class MailUrlBuilder
	{
		public string to;

		public string subject;

		public string body;

		public string GetUrl()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("mailto:{0}", to);
			string arg = Uri.EscapeUriString(subject);
			stringBuilder.AppendFormat("?subject={0}", arg);
			string arg2 = Uri.EscapeUriString(body);
			stringBuilder.AppendFormat("&body={0}", arg2);
			return stringBuilder.ToString();
		}
	}
}
