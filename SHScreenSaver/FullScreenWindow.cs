using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenSaver
{
	public partial class FullScreenWindow : Form
	{
		static bool _formClosing = false;

		Screen _screen;
		PaintPictures _painter;
		Point _mouseLocation;

		public FullScreenWindow(int monIndex, Point mouseLocation, int delay = 0)
		{
			System.Diagnostics.Trace.TraceInformation("[{0}]: FullScreenWindow Initializing.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			InitializeComponent();
			FormBorderStyle = FormBorderStyle.None;
#if DEBUG
			TopMost = false;
#else
			TopMost = true;
#endif
			BackColor = Color.Black;

			var screens = Screen.AllScreens;
			if (monIndex >= screens.Length)
				throw new InvalidOperationException();

			_screen = screens[monIndex];
			SuspendLayout();
			Bounds = _screen.Bounds;
			ResumeLayout();

			_painter = new PaintPictures(ClientRectangle);
			_painter.TimerTick += PainterTimerTick;
			if (delay > 500)
				delay = 500;
			_painter.TickTimer.Interval += delay;

			_mouseLocation = mouseLocation;
#if DEBUG
			System.Diagnostics.Trace.TraceInformation("[{0}] Starting mouse position: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), _mouseLocation);
#endif
			Cursor.Hide();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			_formClosing = true;
			e.Cancel = false;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			BringToFront();
			Focus();
			KeyPreview = true;
			ShowInTaskbar = false;

			_painter.TickTimer.Start();
		}

		private void PainterTimerTick(object sender, EventArgs e)
		{
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			_painter.Paint(e.Graphics);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			//			base.OnMouseMove(e);

			if (_formClosing)
				return;

			var loc = PointToScreen(e.Location);

#if DEBUG
			System.Diagnostics.Trace.TraceInformation("[{0}]: Mouse move location: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), loc);
#endif

			var deltaX = loc.X - _mouseLocation.X;
			var deltaY = loc.Y - _mouseLocation.Y;

			if (deltaX > 3 || deltaX < -3 || deltaY > 3 || deltaY < -3)
			{
				Application.Exit();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			//			base.OnMouseUp(e);
			if (!_formClosing)
				Application.Exit();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			//			base.OnMouseWheel(e);
			if (!_formClosing)
				Application.Exit();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
#if DEBUG
			System.Diagnostics.Trace.TraceInformation("[{0}]: KeyDown: Keys.{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Enum.GetName(typeof(Keys), e.KeyData));
#endif
			Application.Exit();
		}

		protected override bool IsInputKey(Keys keyData)
		{
#if DEBUG
			System.Diagnostics.Trace.TraceInformation("[{0}]: keyData: Keys.{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Enum.GetName(typeof(Keys), keyData));
#endif
			return true;
		}
	}
}
