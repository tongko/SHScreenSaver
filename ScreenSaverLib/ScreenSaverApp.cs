﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SinHing.ScreenSaver
{
	public class ScreenSaverApp
	{
		#region Fields

		internal const string MutexName = "ScreenSaver-36e07bf4-8628-4aa7-9aa6-428f9ba7f192";

		private System.Threading.Mutex _mutex;
		private ScreenSaverCallbackEventArgs _eventArgs = null;
		private List<ScreenSaverView> _views = new List<ScreenSaverView>();

		#endregion


		#region Properties

		public EventHandler<ScreenSaverCallbackEventArgs> CreateCallback { get; set; }

		public EventHandler<ScreenSaverCallbackEventArgs> TimerCallback { get; set; }

		public EventHandler<ScreenSaverCallbackEventArgs> LoopCallback { get; set; }

		public EventHandler<ScreenSaverCallbackEventArgs> DestroyCallback { get; set; }

		public System.Windows.Forms.Form ConfigDialog { get; set; }

		public ScreenSaverMode Mode { get; private set; }

		#endregion


		#region Methods

		public void Run(ScreenSaverSettings settings)
		{
			if (PreviousInstanceExists())
				return;

			var isPreview = settings.Mode == ScreenSaverMode.Preview;
			var callbackInfo = new CallbackInfo();
			callbackInfo.CreateCallback = CreateCallback;
			callbackInfo.TimerCallback = TimerCallback;
			callbackInfo.LoopCallback = LoopCallback;
			callbackInfo.DestroyCallback = DestroyCallback;
			var delay = settings.Interval * 1000;

			switch (settings.Mode)
			{
				case ScreenSaverMode.ConfigDialog:
					if (settings.ParentHandle.Handle == IntPtr.Zero)
					{
						ConfigDialog.ShowDialog();
						break;
					}

					var parent = new System.Windows.Forms.NativeWindow();
					parent.AssignHandle(settings.ParentHandle.Handle);
					ConfigDialog.ShowDialog(parent);
					break;
				case ScreenSaverMode.Preview:
					var pv = new ScreenSaverView(0, callbackInfo, settings.ParentHandle.Handle, System.Drawing.Rectangle.Empty,
						delay);
					pv.Update();
					InternalLoop();
					break;
				case ScreenSaverMode.FullScreen:
					List<ScreenSaverView> views;
					if (settings.SaveAllScreen)
					{
						var screens = Screen.AllScreens;
						for (var i = 0; i < screens.Length; i++)
						{
							var v = new ScreenSaverView(i, callbackInfo, IntPtr.Zero, screens[i].Bounds,
								delay + settings.InterveneDelay);
							v.Update();
						}
					}
					else
					{
						views = new List<ScreenSaverView>(1);
						var sv = new ScreenSaverView(0, callbackInfo, IntPtr.Zero, Screen.PrimaryScreen.Bounds,
							delay);
						sv.Update();
					}

					Unsafe.ShowCursor(false);
					InternalLoop();
					break;
				case ScreenSaverMode.Debug:
					var dv = new ScreenSaverView(0, callbackInfo, IntPtr.Zero, Screen.PrimaryScreen.Bounds,//new System.Drawing.Rectangle(50, 50, 640, 360),
						delay, true);
					dv.Update();
					InternalLoop();
					break;
				default:
					break;
			}
		}

		private void InternalLoop()
		{
			Unsafe.MSG msg;
			while (true)
			{
				if (Unsafe.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0))
				{
					var result = Unsafe.GetMessage(out msg, IntPtr.Zero, 0, 0);
					if (result <= 0)
						break;

					Unsafe.DispatchMessage(ref msg);
				}
				else
				{
					if (LoopCallback == null)
						continue;

					foreach (var d in LoopCallback.GetInvocationList())
					{
						var s = d.Target as System.ComponentModel.ISynchronizeInvoke;
						if (s != null && s.InvokeRequired)
							s.BeginInvoke(d, new object[] { this, _eventArgs });
						else
							d.DynamicInvoke(this, _eventArgs);
					}
				}
			}
		}

		private bool PreviousInstanceExists()
		{
			try
			{
				_mutex = System.Threading.Mutex.OpenExisting(MutexName);
				if (_mutex != null)
					return true;
			}
			catch (System.Threading.WaitHandleCannotBeOpenedException)
			{
				_mutex = new System.Threading.Mutex(true, MutexName);
			}
			return false;
		}

		#endregion

	}
}
