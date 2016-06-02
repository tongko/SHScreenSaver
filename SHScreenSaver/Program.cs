using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SinHing.ScreenSaver;

namespace ScreenSaver
{
	class Program
	{
		static List<Painter> _views = new List<Painter>(100);

		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var settings = new ScreenSaverSettings();
			var parser = new ParseArguments(settings);
			parser.Parse(args);

			settings.Interval = Settings.Instance.Interval;
			settings.InterveneDelay = 300;
			settings.SaveAllScreen = Settings.Instance.AllMonitors;

			var ssApp = new ScreenSaverApp();
			ssApp.ConfigDialog = new ConfigDialog();
			ssApp.CreateCallback = OnCreate;
			ssApp.DestroyCallback = OnDestroy;
			ssApp.TimerCallback = OnTimer;
			ssApp.Run(settings);
		}

		private static void OnCreate(object sender, ScreenSaverCallbackEventArgs e)
		{
			var view = sender as ScreenSaverView;
			if (view == null)
				return;

			_views.Insert(view.Sequence, new Painter(view));
		}

		private static void OnTimer(object sender, ScreenSaverCallbackEventArgs e)
		{
			_views[e.View.Sequence]?.DoPainting();
		}

		private static void OnDestroy(object sender, ScreenSaverCallbackEventArgs e)
		{
			var view = e.View;
			_views[view.Sequence].Dispose();
			_views[view.Sequence] = null;
		}
	}
}
