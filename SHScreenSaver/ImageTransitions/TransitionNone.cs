using System.Drawing;

namespace ScreenSaver.ImageTransitions
{
	class TransitionNone : TransitionEffect
	{
		public TransitionNone(TransitionInfo info)
			: base(info)
		{
		}

		public override void Draw(System.Windows.Forms.PaintEventArgs e)
		{
			lock (SyncRoot)
			{
				var g = Graphics.FromImage(Canvas);
				g.Clear(Color.Black);

				var rect = new Rectangle(Point.Empty, BackImage.Size);
				ResizeAndCenter(ref rect);

				g.DrawImage(BackImage, rect);
			}

			base.Draw(e);
		}

		public override void Start()
		{
			base.Start();

			Stop();
		}
	}
}
