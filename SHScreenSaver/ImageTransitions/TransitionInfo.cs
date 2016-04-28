namespace ScreenSaver.ImageTransitions
{
	public struct TransitionInfo
	{
		public static readonly TransitionInfo Empty = new TransitionInfo();

		public System.Drawing.Rectangle WorkingArea { get; set; }

		public System.Drawing.Image BackImage { get; set; }

		public System.Drawing.Image FrontImage { get; set; }

		public int StepTime { get; set; }

		public int TransitionTime { get; set; }
	}
}