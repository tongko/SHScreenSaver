using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSaver
{
	class ArgumentsParser
	{
		private static EventLogger Log = EventLogger.Instance;
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
			Log.WriteInfo("Arguments: \r\n" + string.Join("|", args));

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
				Log.WriteInfo("Screen saver mode.");
				Settings.Instance.DisplayMode = DisplayModes.ShowSaver;
			}
			else if (_args[0][1].Equals('p') || _args[0][1].Equals('P'))
			{
				Log.WriteInfo("Preview mode.");
				Settings.Instance.DisplayMode = DisplayModes.ShowPreview;
				var i = int.Parse(_args[1]);
				Settings.Instance.ParentHandle = new IntPtr(i);
			}
			else if (_args[0][1].Equals('c') || _args[0][1].Equals('C'))
			{
				Log.WriteInfo("Config mode.");
				Settings.Instance.DisplayMode = DisplayModes.ShowConfig;
				if (_args.Count > 1)
				{
					var i = int.Parse(_args[1]);
					Settings.Instance.ParentHandle = new IntPtr(i);
				}
			}

			if (Settings.Instance.ParentHandle != IntPtr.Zero)
				Log.WriteInfo(string.Format("Parent Handle pass in via arguments is: '{0}'", Settings.Instance.ParentHandle.ToInt32()));
		}
	}
}
