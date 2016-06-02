using System;
using System.Linq;

namespace SinHing.ScreenSaver
{
	public sealed class ParseArguments
	{
		private ScreenSaverSettings _settings;

		public ParseArguments(ScreenSaverSettings setting)
		{
			if (setting == null)
				throw new ArgumentNullException("setting");

			_settings = setting;
		}

		public void Parse(string[] args)
		{
			var arguments = args.ToList();

			if (arguments.Count > 0)
			{
				if (arguments[0].Length < 2 || (arguments[0][0] != '/' && arguments[0][0] != '-'))
					throw new ArgumentException("Invalid arguments specified.", "args");

				char c = arguments[0][1];
				if (c.Equals('s') || c.Equals('S'))
					_settings.SetScreenSaverMode(ScreenSaverMode.FullScreen);
				else if (c.Equals('p') || c.Equals('P'))
					_settings.SetScreenSaverMode(ScreenSaverMode.Preview);
				else if (c.Equals('c') || c.Equals('C'))
					_settings.SetScreenSaverMode(ScreenSaverMode.ConfigDialog);
				else if (c.Equals('d') || c.Equals('D'))
					_settings.SetScreenSaverMode(ScreenSaverMode.Debug);

				if (_settings.Mode == ScreenSaverMode.FullScreen || _settings.Mode == ScreenSaverMode.Debug)
					return;

				int pointer;
				if (arguments.Count < 2 || !int.TryParse(arguments[1], out pointer))
					throw new ArgumentException("Invalid arguments specified.", "args");

				_settings.SetParentHandle(new System.Runtime.InteropServices.HandleRef(null, new IntPtr(pointer)));
			}
			else
				_settings.SetScreenSaverMode(ScreenSaverMode.FullScreen);
		}
	}
}
