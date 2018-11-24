using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Controls.WindowsForms
{
	static class NativeMethods
	{
		public static Point GetScrollPosition(IntPtr handle)
		{
			Point pt = new Point();
			SendMessage(handle, 0x4DD, IntPtr.Zero, ref pt);
			return pt;
		}

		public static void SetScrollPosition(IntPtr handle, Point value) { SendMessage(handle, 0x4DE, IntPtr.Zero, ref value); }

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr handle, int message, IntPtr wParam, ref Point lParam);

		[DllImport("user32.dll")]
		public static extern int LockWindowUpdate(IntPtr handle);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTextMetrics(IntPtr hdc, TextMetrics lptm);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiob);
	}
}
