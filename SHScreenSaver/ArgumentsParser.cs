using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ScreenSaver
{
	class ArgumentsParser
	{
		private List<string> _args;

		static ArgumentsParser()
		{
			Instance = new ArgumentsParser();
		}

		private ArgumentsParser()
		{
		}

		public static ArgumentsParser Instance { get; private set; }

		public void Parse(string[] args)
		{
			Trace.TraceInformation("[{0}]: Arguments: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), string.Join("|", args));

			_args = args.ToList();

			if (_args.Count == 0)
			{
				Settings.Instance.DisplayMode = DisplayModes.ShowSaver;
				return;
			}

			//if (_args[0].Length < 2)
			//	throw new ArgumentException();
			Trace.TraceInformation("[{0}]: The _args[0][1] is: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), _args[0][1]);
			if (_args[0][1].Equals('s') || _args[0][1].Equals('S'))
			{
				Trace.TraceInformation("[{0}]: Screen saver mode.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				Settings.Instance.DisplayMode = DisplayModes.ShowSaver;
			}
			else if (_args[0][1].Equals('p') || _args[0][1].Equals('P'))
			{
				Trace.TraceInformation("[{0}]: Preview mode.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				Settings.Instance.DisplayMode = DisplayModes.ShowPreview;
				var i = int.Parse(_args[1]);
				Settings.Instance.ParentHandle = new IntPtr(i);
			}
			else if (_args[0][1].Equals('c') || _args[0][1].Equals('C'))
			{
				Trace.TraceInformation("[{0}]: Config mode.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				Settings.Instance.DisplayMode = DisplayModes.ShowConfig;
				if (_args.Count > 1)
				{
					var i = int.Parse(_args[1]);
					Settings.Instance.ParentHandle = new IntPtr(i);
				}
			}

			if (Settings.Instance.ParentHandle != IntPtr.Zero)
				Trace.TraceInformation("[{0}]: Parent Handle pass in via arguments is: '{1}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Settings.Instance.ParentHandle.ToInt32());
			Trace.TraceInformation("[{0}]: Parse argument completed.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
		}
	}
}
