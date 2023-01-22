using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DesktopClock
{
	public static class Win32
	{
		private const int WS_EX_TRANSPARENT = 0x00000020;
		private const int WS_EX_NOACTIVATE = 0x08000000;
		private const int GWL_EXSTYLE = -20;

		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		public static void MakeTransparent(Window window)
		{
			var handle = window.GetHandle();

			// Change the extended window style to include WS_EX_TRANSPARENT
			int extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);
			SetWindowLong(handle, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
		}

		public static void MakeNoActivate(Window window)
		{
			var handle = window.GetHandle();

			// Change the extended window style to include WS_EX_NOACTIVATE
			int extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);
			SetWindowLong(handle, GWL_EXSTYLE, extendedStyle | WS_EX_NOACTIVATE);
		}

		private static IntPtr GetHandle(this Window window)
		{
			return new WindowInteropHelper(window).Handle;
		}
	}
}
