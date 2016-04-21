using System.Drawing;
using System.Windows.Forms;

namespace SinHing.ScreenSaver
{
	public abstract class PaintScreen
	{
		protected virtual void OnPaintScreen(PaintEventArgs e)
		{
			var graphics = e.Graphics;

			graphics.FillRectangle(Brushes.Black, e.ClipRectangle);
		}
	}
}
