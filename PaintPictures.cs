using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScreenSaver
{
	class PaintPictures
	{
		List<string> _paths;
		int _currentIndex;
		Rectangle _paintArea;

		public PaintPictures(Rectangle paintArea)
		{
			_paths = new List<string>();
			var dirs = Settings.Instance.ImagePaths.ToList();
			foreach (var item in dirs)
			{
				Trace.TraceInformation("[{0}]: Search image in path: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item);
				Trace.Flush();
				_paths.AddRange(System.IO.Directory.GetFiles(item));
			}
			_paths.Shuffle();

			_paintArea = paintArea;
			TickTimer = new Timer { Interval = Settings.Instance.Interval * 1000 };
			TickTimer.Tick += OnTimerTick;
		}

		public event EventHandler TimerTick;

		private void OnTimerTick(object sender, EventArgs e)
		{
			if (_currentIndex >= _paths.Count)
				_currentIndex = 0;
			_currentIndex++;

			if (TimerTick != null)
				TimerTick(this, EventArgs.Empty);
		}

		public Timer TickTimer { get; private set; }

		public void Paint(Graphics graphics)
		{
			graphics.FillRectangle(Brushes.Black, _paintArea);

			Image image = null;

			while (image == null)
			{
				try
				{
					image = Image.FromFile(_paths[_currentIndex]);
				}
				catch (OutOfMemoryException)
				{
					_paths.RemoveAt(_currentIndex);
					if (_paths.Count == 0)
						throw new InvalidOperationException("Specified folders do not contain valid image files.");
				}
			}

			var size = ScaleImage(image, _paintArea.Width, _paintArea.Height);

			var cx = (_paintArea.Width - size.Width) / 2;
			var cy = (_paintArea.Height - size.Height) / 2;

			Trace.TraceInformation("[{0}]: bounds: {5}, cx: {1}, cy: {2}, sz.w: {3}, sz.h: {4} | Image dimension: w: {6} - h: {7}",
				DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), cx, cy, size.Width, size.Height, _paintArea, image.Width, image.Height);
			graphics.DrawImage(image, new Rectangle(cx, cy, size.Width, size.Height));
		}

		private Size ScaleImage(Image image, int maxW, int maxH)
		{
			var ratio = Math.Min((double)maxW / image.Width, (double)maxH / image.Height);

			return new Size((int)(image.Width * ratio), (int)(image.Height * ratio));
		}
	}
}
