using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using Screen = System.Windows.Forms.Screen;

namespace DesktopClock
{
	public partial class MainWindow : INotifyPropertyChanged
	{
		private readonly TrayIcon trayIcon;
		private readonly Settings settings;

		private string displayTime = "44:44";
		public string DisplayTime
		{
			get => displayTime;
			private set
			{
				displayTime = value;
				OnPropertyChanged();
			}
		}

		public MainWindow()
		{
			// Settings
			settings = Settings.Load();

			InitializeComponent();

			// Tray icon
			trayIcon = new TrayIcon(settings);
			trayIcon.ItemClicked += TrayIcon_ItemClicked;

			// DisplayTime update timer
			var timer = new DispatcherTimer(DispatcherPriority.Render)
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			timer.Tick += Timer_Tick;
			timer.Start();
			SetClockTime();

			// Listen for when the screen resolution/composition changes
			SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
		}

		private void SetClockPosition()
		{
			var screen = settings.ScreenIndex < Screen.AllScreens.Length ? Screen.AllScreens[settings.ScreenIndex] : Screen.AllScreens[0];

			switch (settings.ClockPosition)
			{
				case ClockPosition.TopLeft:
					Left = screen.WorkingArea.Left;
					Top = screen.WorkingArea.Top;
					break;
				case ClockPosition.TopRight:
					Left = screen.WorkingArea.Left + screen.WorkingArea.Width - Width;
					Top = screen.WorkingArea.Top;
					break;
				case ClockPosition.BottomRight:
					Left = screen.WorkingArea.Left + screen.WorkingArea.Width - Width;
					Top = screen.WorkingArea.Top + screen.WorkingArea.Height - Height;
					break;
				case ClockPosition.BottomLeft:
					Left = screen.WorkingArea.Left;
					Top = screen.WorkingArea.Top + screen.WorkingArea.Height - Height;
					break;
				default:
					throw new ArgumentOutOfRangeException($"Unexpected value for clock position : '{settings.ClockPosition}'.");
			}
		}

		private void SetClockTime()
		{
			DisplayTime = DateTime.Now.ToString("HH:mm");
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Win32.MakeTransparent(this);
			Win32.MakeNoActivate(this);
			SetClockPosition();
		}

		private void TrayIcon_ItemClicked(object? sender, TrayAction action)
		{
			switch (action)
			{
				case TrayAction.ChangeScreen:
				case TrayAction.ChangePosition:
					SetClockPosition();
					break;
				case TrayAction.Quit:
					Close();
					break;
				default:
					throw new ArgumentOutOfRangeException($"Unexpected tray action : '{action}'.");
			}
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			SetClockTime();
		}

		private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
		{
			SetClockPosition();
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string? propertyname = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
		}
	}
}
