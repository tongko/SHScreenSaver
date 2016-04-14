using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ScreenSaver
{
	class Program
	{
		internal const string MutexName = "ScreenSaver-36e07bf4-8628-4aa7-9aa6-428f9ba7f192";
		private static Mutex _mutex;
		private static EventLogger Log = EventLogger.Instance;

		public Mutex ExecutingFlag { get; private set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			InitTrace();
			if (PreviousInstanceExists())
			{
				System.Diagnostics.Trace.TraceInformation("Another instance is running, quit application.");
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

			ExecutingFlag = new Mutex(true, MutexName + "Running");
		}

		private void Run()
		{
			var pWnd = new PreviewWindow(Settings.Instance.ParentHandle);
			pWnd.Run();
		}

		private static void InitTrace()
		{
			System.Diagnostics.Trace.Listeners.Clear();
			System.Diagnostics.Trace.Listeners.Add(
				new System.Diagnostics.TextWriterTraceListener(Environment.ExpandEnvironmentVariables("%APPDATA%\\SinHing\\Logs\\Trace.log")));
			System.Diagnostics.Trace.Listeners.Add(
				new System.Diagnostics.EventLogTraceListener("Sin Hing Screen Saver"));
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
