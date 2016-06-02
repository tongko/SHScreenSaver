using System;
using System.Runtime.InteropServices;

namespace SinHing.ScreenSaver
{
	public class ScreenSaverSettings
	{
		public ScreenSaverSettings()
		{
			Mode = ScreenSaverMode.FullScreen;
			InvokeMethod = InvokeMethods.OnTimer;
			ParentHandle = new HandleRef(null, IntPtr.Zero);
		}

		public ScreenSaverMode Mode { get; private set; }

		public InvokeMethods InvokeMethod { get; set; }

		internal HandleRef ParentHandle { get; private set; }

		public int Interval { get; set; }

		public int InterveneDelay { get; set; }

		public bool SaveAllScreen { get; set; }

		internal void SetScreenSaverMode(ScreenSaverMode mode)
		{
			Mode = mode;
		}

		internal void SetParentHandle(HandleRef handle)
		{
			ParentHandle = new HandleRef(this, handle.Handle);
		}
	}
}
