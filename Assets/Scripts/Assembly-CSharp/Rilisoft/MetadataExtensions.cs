using System.Globalization;
using GooglePlayGames.BasicApi.SavedGame;

namespace Rilisoft
{
	internal static class MetadataExtensions
	{
		public static string GetDescription(this ISavedGameMetadata metadata)
		{
			if (metadata == null)
			{
				return "<null>";
			}
			return string.Format(CultureInfo.InvariantCulture, "{0} ({1:s})", metadata.Description, metadata.LastModifiedTimestamp);
		}
	}
}
