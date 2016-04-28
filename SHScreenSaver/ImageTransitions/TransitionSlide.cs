using System.Drawing;
using System.Windows.Forms;

namespace ScreenSaver.ImageTransitions
{
	enum SlideDirection
	{
		None,
		Left,
		Right,
		Up,
		Down
	}

	class TransitionSlide
		: TransitionEffect
	{
		private int _distance;

		public TransitionSlide(TransitionInfo info, SlideDirection direction)
			: base(info)
		{
			Direction = direction;
		}

		public SlideDirection Direction { get; set; }

		public override void Start()
		{
			lock (SyncRoot)
			{
				_distance = 0;
				TickTimer = new System.Timers.Timer(StepTime);
				TickTimer.Elapsed += TimerTick;
				TickTimer.Start();
			}

			base.Start();
		}

		public override void Step()
		{
			lock (SyncRoot)
				_distance = System.Math.Min(Bounds.Width * CurrentStep / TransitionTime, Bounds.Width);

			base.Step();
		}

		public override void Draw(PaintEventArgs e)
		{
			lock (SyncRoot)
			{
				var g = Graphics.FromImage(Canvas);
				g.Clear(Color.Black);

				var rectBack = new Rectangle(Point.Empty, BackImage.Size);
				ResizeAndCenter(ref rectBack);
				var rectFront = new Rectangle(Point.Empty, FrontImage.Size);
				ResizeAndCenter(ref rectFront);

				switch (State)
				{
					case TransitionState.Transitioning:
						switch (Direction)
						{
							case SlideDirection.Left:
								rectBack.X += (Bounds.Width - _distance);
								rectFront.X -= _distance;
								break;
							case SlideDirection.Right:
								rectBack.X -= (Bounds.Width - _distance);
								rectFront.X += _distance;
								break;
							case SlideDirection.Up:
								rectBack.Y += (Bounds.Height - _distance);
								rectFront.Y -= _distance;
								break;
							case SlideDirection.Down:
								rectBack.Y -= (Bounds.Height - _distance);
								rectFront.Y += _distance;
								break;
						}

						if (BackImage != null)
							g.DrawImage(BackImage, rectBack, 0, 0, BackImage.Width, BackImage.Height, GraphicsUnit.Pixel);
						if (FrontImage != null)
							g.DrawImage(FrontImage, rectFront, 0, 0, FrontImage.Width, FrontImage.Height, GraphicsUnit.Pixel);
						break;
					case TransitionState.Finished:
						g.DrawImage(BackImage, rectBack);
						break;
					default:
						g.DrawImage(FrontImage, rectFront);
						break;
				}
			}
		}
	}
}
