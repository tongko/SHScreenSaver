using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSaver
{
	class Settings
	{
		private readonly string[] KeyPaths = { "SinHing", "ScreenSaver"};
		private const string ImagePathsKeyName = "ImagePaths";
		private const string IntervalKeyName = "Interval";

		private static Settings _inst;
		private List<string> _paths;

		private Settings()
		{
			RetrieveSettings();
		}

		public static Settings Instance
		{
			get
			{
				if (_inst == null)
					return (_inst = new Settings());

				return _inst;
			}
		}

		public IReadOnlyList<string> ImagePaths { get { return _paths; } }


		public int Interval { get; private set; }

		public DisplayModes DisplayMode { get; set; }

		public IntPtr ParentHandle { get; set; }

		private void RetrieveSettings()
		{
			var parentKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);
			for (int i = 0; i < KeyPaths.Length; i++)
			{
				var key = parentKey.OpenSubKey(KeyPaths[i], true);
				if (key == null)
					key = parentKey.CreateSubKey(KeyPaths[i]);
				parentKey = key;
			}

			var paths = (string)parentKey.GetValue(ImagePathsKeyName, "C:\\Users\\liew343241\\Pictures\\刘琦宝贝\\");
			_paths = paths.Split(new[] { ' ' }).ToList();
			Interval = (int)parentKey.GetValue(IntervalKeyName, 5);
		}
	}
}
