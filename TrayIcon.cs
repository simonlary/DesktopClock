using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace DesktopClock
{
	public class TrayIcon
	{
		private readonly NotifyIcon notifyIcon = new()
		{
			Visible = true
		};

		private readonly Settings settings;

		public TrayIcon(Settings settings)
		{
			this.settings = settings;

			InitializeIcon();
			InitializeContextMenu();
		}

		private void InitializeIcon()
		{
			// Get the WPF resource BitmapSource
			var bitmapSource = (BitmapSource)System.Windows.Application.Current.FindResource("Icon");

			// Convert to a WinForm Bitmap
			using var stream = new MemoryStream();
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
			encoder.Save(stream);
			var bitmap = new Bitmap(stream);

			// Convert to an Icon
			using var icon = Icon.FromHandle(bitmap.GetHicon());

			// Set the NotifyIcon icon
			notifyIcon.Icon = icon;
		}

		private void InitializeContextMenu()
		{
			// Screen selection
			var screenSelectionItem = new ToolStripMenuItem("Screen");
			for (var i = 0; i < Screen.AllScreens.Length; i++)
				screenSelectionItem.DropDownItems.Add(CreateScreenMenuItem(i));

			// Clock position selection
			var clockPositionSelectionItem = new ToolStripMenuItem("Position");
			foreach (ClockPosition clockPosition in Enum.GetValues<ClockPosition>())
				clockPositionSelectionItem.DropDownItems.Add(CreatePositionMenuItem(clockPosition));

			// Create the menu
			notifyIcon.ContextMenuStrip = new ContextMenuStrip
			{
				Items =
				{
					screenSelectionItem,
					clockPositionSelectionItem,
					new ToolStripMenuItem("Quit", null, Quit_Click),
				}
			};
		}

		private ToolStripMenuItem CreateScreenMenuItem(int screenIndex)
		{
			return new ToolStripMenuItem($"Screen {screenIndex + 1}", null, (sender, e) => ScreenSelected_Click(screenIndex))
			{
				Checked = screenIndex == settings.ScreenIndex
			};
		}

		private ToolStripMenuItem CreatePositionMenuItem(ClockPosition clockPosition)
		{
			return new ToolStripMenuItem(clockPosition.GetDescription(), null, (sender, e) => PositionSelected_Click(clockPosition))
			{
				Checked = clockPosition == settings.ClockPosition
			};
		}

		private void ScreenSelected_Click(int screenIndex)
		{
			settings.ScreenIndex = screenIndex;
			settings.Save();
			InitializeContextMenu();
			OnItemClicked(TrayAction.ChangeScreen);
		}

		private void PositionSelected_Click(ClockPosition clockPosition)
		{
			settings.ClockPosition = clockPosition;
			settings.Save();
			InitializeContextMenu();
			OnItemClicked(TrayAction.ChangePosition);
		}

		private void Quit_Click(object? sender, EventArgs e)
		{
			OnItemClicked(TrayAction.Quit);
		}

		public event EventHandler<TrayAction>? ItemClicked;
		private void OnItemClicked(TrayAction action)
		{
			ItemClicked?.Invoke(this, action);
		}
	}

	public enum TrayAction
	{
		ChangeScreen,
		ChangePosition,
		Quit
	}
}
