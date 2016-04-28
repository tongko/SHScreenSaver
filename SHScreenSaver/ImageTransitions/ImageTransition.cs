using System;
using System.Drawing;

namespace ScreenSaver.ImageTransitions
{
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
		All
	}

	class ImageTransition
	{
		private TransitionEffects _effects;
		private int _index;

		public ImageTransition(Rectangle bounds, TransitionEffects effects, bool randomize = false)
		{
			Bounds = bounds;
			Randomize = randomize;
			_effects = effects;
		}

		public TransitionEffect CreateTransition(Image front, Image back)
		{
			TransitionEffects effects = _effects;
			if (effects == TransitionEffects.All && Randomize)
			{
				var values = Enum.GetValues(typeof(TransitionEffects));
				var random = new Random();
				effects = (TransitionEffects)values.GetValue(random.Next(values.Length));
			}

			var info = new TransitionInfo
			{
				BackImage = back,
				FrontImage = front,
				TransitionTime = 1000,
				StepTime = 1000 / 60,   // 60 fps
				WorkingArea = Bounds
			};

			return TransitionEffect.Create(effects, info);
		}

		public Rectangle Bounds { get; set; }

		public bool Randomize { get; set; }


	}
}
