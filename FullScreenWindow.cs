using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenSaver
{
	public partial class FullScreenWindow : Form
	{
		Screen _screen;
		PaintPictures _painter;
		Point _mouseLocation;

		public FullScreenWindow(int monIndex, Point mouseLocation)
		{
			InitializeComponent();
			FormBorderStyle = FormBorderStyle.None;
			TopMost = true;
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

			_mouseLocation = mouseLocation;
		}

		private void PainterTimerTick(object sender, EventArgs e)
		{
			var g = CreateGraphics();
			_painter.Paint(g);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			var loc = e.Location;
			var deltaX = loc.X - _mouseLocation.X;
			var deltaY = loc.Y - _mouseLocation.Y;

			if (deltaX > 3 || deltaX < -3 || deltaY > 3 || deltaY < -3)
				Application.Exit();
		}
	}
}
