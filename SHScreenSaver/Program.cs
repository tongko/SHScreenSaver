using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ScreenSaver
{
	class Program
	{
		internal const string MutexName = "ScreenSaver-36e07bf4-8628-4aa7-9aa6-428f9ba7f192";
		private static Mutex _mutex;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			InitTrace();
			if (PreviousInstanceExists())
			{
				System.Diagnostics.Trace.TraceInformation("[{0}] Another instance is running, quit application.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				return;
			}

			if (args == null)
				args = new string[0];

			ArgumentsParser.Instance.Parse(args);
			var p = new Program();
			p.Run();
		}

		static bool PreviousInstanceExists()
		{
			try
			{
				_mutex = Mutex.OpenExisting(MutexName);
				if (_mutex != null)
					return true;
			}
			catch (WaitHandleCannotBeOpenedException)
			{
				_mutex = new Mutex(true, MutexName);
			}
			return false;
		}

		Program()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		private void Run()
		{
			Trace.TraceInformation("[{0}]: Settings.Instance.DisplayMode is {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Enum.GetName(typeof(DisplayModes), Settings.Instance.DisplayMode));
			switch (Settings.Instance.DisplayMode)
			{
				case DisplayModes.ShowConfig:
					RunShowConfig();
					break;
				case DisplayModes.ShowPreview:
					RunPreview();
					break;
				case DisplayModes.ShowSaver:
					RunFullScreen();
					break;
				default:
					break;
			}
		}

		private void RunShowConfig()
		{
			try
			{
				Trace.TraceInformation("[{0}]: Begin config settings.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

				var config = new ConfigDialog();
				var settings = Settings.Instance;
				if (settings.ParentHandle != IntPtr.Zero)
				{
					var parent = new NativeWindow();
					parent.AssignHandle(settings.ParentHandle);
					config.ShowDialog(parent);
				}
				else
					config.ShowDialog();
			}
			catch (Exception e)
			{
				Trace.TraceError("[{0}]: Error occurs while running screensaver settings. Exception: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e);
				throw;
			}
		}

		private void RunFullScreen()
		{
			try
			{
				Trace.TraceInformation("[{0}]: Begin run full screen.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				if (Settings.Instance.AllMonitors)
				{
					var screens = Screen.AllScreens;
					for (int i = 0; i < screens.Length; i++)
					{
						//if (screens[i] == Screen.PrimaryScreen)
						//	continue;

						//var thread = new Thread(new ParameterizedThreadStart(StartFullScreen));
						//thread.TrySetApartmentState(ApartmentState.STA);
						//thread.Start(i);
						new FullScreenWindow(i, Cursor.Position).Show();
					}
				}

				//Application.Run(new FullScreenWindow(0, Cursor.Position));
				Application.Run();
			}
			catch (Exception e)
			{
				Trace.TraceError("[{0}]: Error occurs while running screensaver. Exception: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e);
				throw;
			}
		}

		private void StartFullScreen(object parameter)
		{
			var r = new Random();

			int monIndex = (int)parameter;
			Application.Run(new FullScreenWindow(monIndex, Cursor.Position, r.Next(250, 500)));
		}

		private void RunPreview()
		{
			var pWnd = new PreviewWindow(Settings.Instance.ParentHandle);
			pWnd.Run();
		}

		private static void InitTrace()
		{
			var dir = Environment.ExpandEnvironmentVariables("%APPDATA%\\SinHing\\");
			if (!System.IO.Directory.Exists(dir))
				System.IO.Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(dir));
			if (!System.IO.Directory.Exists(dir + "Logs"))
				System.IO.Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(dir + "Logs"));

			System.Diagnostics.Trace.Listeners.Clear();
			System.Diagnostics.Trace.Listeners.Add(
				new System.Diagnostics.TextWriterTraceListener(dir + "Logs\\Trace.log"));
			//System.Diagnostics.Trace.Listeners.Add(
			//	new System.Diagnostics.EventLogTraceListener("Sin Hing Screen Saver"));
			System.Diagnostics.Trace.AutoFlush = true;
			System.Diagnostics.Trace.TraceInformation("Trace start: {0}", DateTime.Now);
		}

		[StructLayout(LayoutKind.Sequential)]
		struct POINT
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct MSG
		{
			public IntPtr hwnd;
			public int message;
			public UIntPtr wParam;
			public IntPtr lParam;
			public int time;
			public POINT pt;
		}

		const int PM_NOREMOVE = 0;


		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = false)]
		private static extern int DispatchMessage(ref MSG m);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = false)]
		private static extern bool GetMessage(ref MSG m, IntPtr hwnd, int wMsgFilterMin, int wMsgFilterMax);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = false)]
		private static extern bool PeekMessage(ref MSG m, IntPtr hwnd, int wMsgFilterMin, int wMsgFilterMax, int wRemoveMsg);
	}
}
