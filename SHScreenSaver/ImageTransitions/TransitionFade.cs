using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ScreenSaver.ImageTransitions
{
	class TransitionFade : TransitionEffect
	{
		private static object _sync = new object();

		ColorMatrix _backMatrix;
		ColorMatrix _frontMatrix;
		float _fade;

		public TransitionFade(TransitionInfo info) : base(info)
		{
			_frontMatrix = new ColorMatrix { Matrix33 = 1F };
			_backMatrix = new ColorMatrix { Matrix33 = 0F };
		}

		public override void Start()
		{
			lock (SyncRoot)
			{
				_backMatrix.Matrix33 = 1F;
				_frontMatrix.Matrix33 = 0F;
				TickTimer = new System.Timers.Timer(StepTime);
				TickTimer.Elapsed += TimerTick;
				TickTimer.Start();
			}
			base.Start();
		}

		public override void Step()
		{
			lock (_sync)
			{
				_fade = Math.Min(1F, (float)CurrentStep / TransitionTime);
				_backMatrix.Matrix33 = 1F - (_frontMatrix.Matrix33 = _fade);
			}

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

				if (State == TransitionState.Transitioning)
				{
					var imgAttr = new ImageAttributes();
					if (BackImage != null)
					{
						imgAttr.SetColorMatrix(_backMatrix);
						g.DrawImage(BackImage, rectBack, 0, 0, BackImage.Width, BackImage.Height, GraphicsUnit.Pixel, imgAttr);
					}
					if (FrontImage != null)
					{
						imgAttr.SetColorMatrix(_frontMatrix);
						g.DrawImage(FrontImage, rectFront, 0, 0, FrontImage.Width, FrontImage.Height, GraphicsUnit.Pixel, imgAttr);
					}
				}
				else if (State == TransitionState.Finished)
					g.DrawImage(BackImage, rectBack);
				else
					g.DrawImage(FrontImage, rectFront);
			}

			base.Draw(e);
		}
	}
}
