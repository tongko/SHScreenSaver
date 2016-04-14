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
			Trace.TraceInformation("Arguments: {0}", string.Join("|", args));

			_args = args.ToList();

			if (_args.Count == 0)
			{
				Settings.Instance.DisplayMode = DisplayModes.ShowSaver;
				return;
			}

			if (_args[0].Length < 2)
				throw new ArgumentException();

			if (_args[0][1].Equals('S') || _args[0][1].Equals('S'))
			{
				Trace.TraceInformation("Screen saver mode.");
				Settings.Instance.DisplayMode = DisplayModes.ShowSaver;
			}
			else if (_args[0][1].Equals('p') || _args[0][1].Equals('P'))
			{
				Trace.TraceInformation("Preview mode.");
				Settings.Instance.DisplayMode = DisplayModes.ShowPreview;
				var i = int.Parse(_args[1]);
				Settings.Instance.ParentHandle = new IntPtr(i);
			}
			else if (_args[0][1].Equals('c') || _args[0][1].Equals('C'))
			{
				Trace.TraceInformation("Config mode.");
				Settings.Instance.DisplayMode = DisplayModes.ShowConfig;
				if (_args.Count > 1)
				{
					var i = int.Parse(_args[1]);
					Settings.Instance.ParentHandle = new IntPtr(i);
				}
			}

			if (Settings.Instance.ParentHandle != IntPtr.Zero)
				Trace.TraceInformation(string.Format("Parent Handle pass in via arguments is: '{0}'", Settings.Instance.ParentHandle.ToInt32()));
		}
	}
}
