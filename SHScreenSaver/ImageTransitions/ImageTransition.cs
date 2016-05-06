using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ScreenSaver.ImageTransitions
{
	[Flags]
	enum TransitionEffects
	{
		None,
		Fade,
		Dissolve,
		ZoomIn,
		ZoomOut,
		SlideLeft,
		SlideRight,
		SlideUp,
		SlideDown,
		All = Fade | Dissolve | ZoomIn | ZoomOut | SlideLeft | SlideRight | SlideUp | SlideDown
	}

	class ImageTransition
	{
		private bool _isPreview;
		private PreviewWindow _previewWnd;
		private FullScreenWindow _fullScrWnd;
		private TransitionEffects _effects;
		private int _index;
		private System.Timers.Timer _timer;
		private bool _randomizeEffects;
		private List<string> _paths;
		private TransitionEffect _currentEffect;

		public ImageTransition(PreviewWindow container, Rectangle maxPaintArea, int interval, TransitionEffects effects,
			bool randomizeEffects, IEnumerable<string> imagePaths, bool randomizeImages)
		{
			if (container == null)
				throw new ArgumentNullException("container");
			if (maxPaintArea == Rectangle.Empty)
				throw new ArgumentException("Paint area cannot be empty.", "maxPaintArea");
			if (interval <= 0)
				throw new ArgumentException("Interval between transition must be greater than or equal to 1.");

			_isPreview = true;
			_previewWnd = container;
			Bounds = maxPaintArea;
			_randomizeEffects = randomizeEffects;
			_effects = effects;
			_paths = GetImagePaths(imagePaths);
			_timer = new System.Timers.Timer(interval * 1000);
			_timer.Elapsed += TimerTicked;
			_index = 0;
		}

		public ImageTransition(FullScreenWindow container, Rectangle maxPaintArea, int interval, TransitionEffects effects,
			bool randomizeEffects, IEnumerable<string> imagePaths, bool randomizeImages)
		{
			if (container == null)
				throw new ArgumentNullException("container");
			if (maxPaintArea == Rectangle.Empty)
				throw new ArgumentException("Paint area cannot be empty.", "maxPaintArea");
			if (interval <= 0)
				throw new ArgumentException("Interval between transition must be greater than or equal to 1.");

			_isPreview = false;
			_fullScrWnd = container;
			Bounds = maxPaintArea;
			_randomizeEffects = randomizeEffects;
			_effects = effects;
			_paths = GetImagePaths(imagePaths);
			_timer = new System.Timers.Timer(interval * 1000);
			_timer.Elapsed += TimerTicked;
			_index = 0;
		}

		public Rectangle Bounds { get; set; }

		public void Start()
		{
			_timer.Start();
		}

		public void Stop()
		{
			_timer.Stop();
		}

		public void PaintTransition(PaintEventArgs e)
		{
			_currentEffect?.Draw(e);
		}

		private void TimerTicked(object sender, System.Timers.ElapsedEventArgs e)
		{
			_timer.Stop();
			var prevIdx = _index;
			var idx = GetNextIndex();
			var back = GetImage(idx);
			var front = GetImage(prevIdx);

			var effects = _effects;
			var info = new TransitionInfo
			{
				BackImage = back,
				FrontImage = front,
				TransitionTime = 1000,
				StepTime = 1000 / 120,   // 60 fps
				WorkingArea = Bounds,
				RandomizeEffect = _randomizeEffects
			};

			_currentEffect = TransitionEffect.Create(effects, info);
			_currentEffect.TransitionStart += TransitionChanged;
			_currentEffect.TransitionStep += TransitionChanged;
			_currentEffect.TransitionStop += TransitionStop;
			_currentEffect.Start();
		}

		private void TransitionStop(object sender, EventArgs e)
		{
			var trans = sender as TransitionEffect;
			if (trans == null)
				return;

			if (trans.State == TransitionState.Finished)
				_timer.Start();
		}

		private void TransitionChanged(object sender, EventArgs e)
		{
			var effect = sender as TransitionEffect;
			if (effect == null)
				return;

			if (_isPreview)
				_previewWnd.Invalidate(effect.MaxArea);
			else
				_fullScrWnd.Invalidate(effect.MaxArea);
		}

		private int GetNextIndex()
		{
			if (++_index == _paths.Count)
				_index = 0;

			return _index;
		}

		private Image GetImage(int index)
		{
			Image img = null;
			while (img == null)
			{
				try
				{
					img = Image.FromFile(_paths[index]);
				}
				catch
				{
					_paths.RemoveAt(index);
					if (_paths.Count == 0)
						throw new InvalidOperationException("No images specified.");
				}
			}

			return img;
		}

		private List<string> GetImagePaths(IEnumerable<string> dirs)
		{
			var imgPaths = new List<string>();

			foreach (var item in dirs)
			{
				if (!Directory.Exists(item))
					continue;
				var paths = Directory.GetFiles(item);
				if (paths == null || paths.Length == 0)
					continue;

				foreach (var p in paths)
					imgPaths.Add(p);
			}

			return imgPaths;
		}
	}
}
