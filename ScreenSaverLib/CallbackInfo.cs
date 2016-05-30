using System;

namespace SinHing.ScreenSaver
{
	public struct CallbackInfo
	{
		public EventHandler<ScreenSaverCallbackEventArgs> CreateCallback;

		public EventHandler<ScreenSaverCallbackEventArgs> TimerCallback;

		public EventHandler<ScreenSaverCallbackEventArgs> LoopCallback;

		public EventHandler<ScreenSaverCallbackEventArgs> DestroyCallback;
	}
}
