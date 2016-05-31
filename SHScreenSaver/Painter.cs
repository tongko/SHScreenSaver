using System.Drawing;
using SinHing.ScreenSaver;

namespace ScreenSaver
{
	class Painter : System.IDisposable
	{
		//private const int DCX_WINDOW = 0x00000001;
		//private const int DCX_CACHE = 0x00000002;
		//private const int DCX_LOCKWINDOWUPDATE = 0x00000400;

		//[DllImport("user32.dll")]
		//private static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr hrgn, uint flags);
		//[DllImport("user32.dll")]
		//private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		private D2DPainter _p;

		public Painter(ScreenSaverView view)
		{
			View = view;
			_p = new D2DPainter(view.Handle, new System.Drawing.Rectangle(
				Point.Empty, view.ClientSize), new System.Drawing.PointF(96f, 96f));
		}

		public ScreenSaverView View { get; set; }

		public void DoPainting()
		{
			_p.DoPaint();
			//IntPtr handle = IntPtr.Zero, hdc = IntPtr.Zero;
			//try
			//{
			//	handle = View.Handle;
			//	hdc = GetDCEx(handle, IntPtr.Zero, DCX_WINDOW | DCX_CACHE | DCX_LOCKWINDOWUPDATE);
			//	using (var g = Graphics.FromHdc(hdc))
			//	{
			//		g.Clear(Color.Black);
			//		g.DrawString("Hello World", new Font("Arial", 11f),
			//			Brushes.Orange, new PointF(15f, 15f));
			//	}
			//}
			//catch (Exception e)
			//{
			//	System.Windows.Forms.MessageBox.Show(e.ToString());
			//}
			//finally
			//{
			//	ReleaseDC(handle, hdc);
			//}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~Painter() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
