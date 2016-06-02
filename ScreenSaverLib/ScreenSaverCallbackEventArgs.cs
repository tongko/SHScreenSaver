using System;

namespace SinHing.ScreenSaver
{
	public class ScreenSaverCallbackEventArgs : EventArgs
	{
		public ScreenSaverCallbackEventArgs(ScreenSaverView view)
		{
			View = view;
		}

		public ScreenSaverView View { get; set; }
	}
}
