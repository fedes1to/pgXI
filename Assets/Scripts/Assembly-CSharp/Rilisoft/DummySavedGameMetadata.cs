using System;
using GooglePlayGames.BasicApi.SavedGame;

namespace Rilisoft
{
	internal sealed class DummySavedGameMetadata : ISavedGameMetadata
	{
		private string _filename;

		public string CoverImageURL
		{
			get
			{
				return "http://example.com";
			}
		}

		public string Description
		{
			get
			{
				return GetType().Name;
			}
		}

		public string Filename
		{
			get
			{
				return _filename;
			}
		}

		public bool IsOpen
		{
			get
			{
				return false;
			}
		}

		public DateTime LastModifiedTimestamp { get; private set; }

		public TimeSpan TotalTimePlayed
		{
			get
			{
				return TimeSpan.Zero;
			}
		}

		public DummySavedGameMetadata(string filename)
		{
			_filename = filename ?? string.Empty;
			LastModifiedTimestamp = DateTime.Now;
		}
	}
}
