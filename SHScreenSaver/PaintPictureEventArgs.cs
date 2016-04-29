using System;

namespace ScreenSaver
{
	class PaintPictureEventArgs
		: EventArgs
	{
		public static readonly PaintPictureEventArgs Empty = new PaintPictureEventArgs { Effect = null };

		public ImageTransitions.TransitionEffect Effect { get; set; }
	}
}
