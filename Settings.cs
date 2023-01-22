using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace DesktopClock
{
	public class Settings
	{
		private const string FOLDER_NAME = "DesktopClock";
		private const string FILE_NAME = "settings.json";

		private static readonly string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FOLDER_NAME);
		private static readonly string filePath = Path.Combine(folderPath, FILE_NAME);

		public int ScreenIndex { get; set; } = 0;
		public ClockPosition ClockPosition { get; set; } = ClockPosition.BottomRight;

		public Settings() { }

		public static Settings Load()
		{
			// Load file if it exists
			if (File.Exists(filePath))
				return JsonSerializer.Deserialize<Settings>(File.ReadAllText(filePath)) ?? new Settings();

			return new Settings();
		}

		public void Save()
		{
			// Make sure the directory exists
			Directory.CreateDirectory(folderPath);

			// Write to file
			File.WriteAllText(filePath, JsonSerializer.Serialize(this));
		}
	}

	public enum ClockPosition
	{
		[Description("Top Left")]
		TopLeft = 0,

		[Description("Top Right")]
		TopRight = 1,

		[Description("Bottom Right")]
		BottomRight = 2,

		[Description("Bottom Left")]
		BottomLeft = 3
	}

	public static class ClockPositionExtension
	{
		public static string GetDescription(this ClockPosition clockPosition)
		{
			var descriptionAttributes = typeof(ClockPosition).GetField(clockPosition.ToString())?.GetCustomAttributes<DescriptionAttribute>(false);
			return descriptionAttributes?.FirstOrDefault()?.Description ?? clockPosition.ToString();
		}
	}
}
