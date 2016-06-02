namespace ScreenSaver.ImageTransitions
{
	public struct TransitionInfo
	{
		public static readonly TransitionInfo Empty = new TransitionInfo();

		public SharpDX.RectangleF WorkingArea { get; set; }

		public SharpDX.WIC.BitmapSource BackImage { get; set; }

		public SharpDX.WIC.BitmapSource FrontImage { get; set; }

		public int StepTime { get; set; }

		public int TransitionTime { get; set; }

		public bool RandomizeEffect { get; set; }

		public SharpDX.Direct2D1.DeviceContext DeviceContext { get; set; }
	}
}