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
		private const string AllMonitorsKeyName = "AllMonitors";

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

		public bool AllMonitors { get; private set; }

		public int Interval { get; private set; }

		public DisplayModes DisplayMode { get; set; }

		public IntPtr ParentHandle { get; set; }

		public void SetInterval(int value)
		{
			if (value < 5 || value > 600)
				throw new ArgumentOutOfRangeException("value", "Value must between 5 and 600");

			Interval = value;
		}

		public void AddPath(string path)
		{
			if (_paths == null)
				_paths = new List<string>();
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException("path");
			if (!System.IO.Directory.Exists(path))
				throw new ArgumentException(string.Format("Directory '{0}' not exists.", path), "path",
					new System.IO.DirectoryNotFoundException());

			_paths.Add(path);
		}

		public void ReplacePathRange(IEnumerable<string> paths)
		{
			if (paths == null)
				throw new ArgumentNullException("paths");

			if (_paths != null)
				_paths.Clear();

			foreach (var path in paths)
				AddPath(path);
		}

		public void SetUseAllMonitors(bool value)
		{
			AllMonitors = value;
		}

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

			var paths = (string)parentKey.GetValue(ImagePathsKeyName, Environment.ExpandEnvironmentVariables("%UserProfile%\\Pictures\\"));
			_paths = paths.Split(new[] { ',' }).ToList();
			Interval = (int)parentKey.GetValue(IntervalKeyName, 5);

			AllMonitors = Convert.ToBoolean(parentKey.GetValue(AllMonitorsKeyName, 1));
		}
	}
}
