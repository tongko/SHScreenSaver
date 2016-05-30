//using System;
//using System.ComponentModel;
//using System.Drawing;
//using System.Windows.Forms;
//using ScreenSaver.ImageTransitions;

//namespace ScreenSaver
//{
//	public partial class FullScreenWindow : Form
//	{
//		static bool _formClosing = false;

//		bool _debug;
//		ImageTransition _transition;
//		Point _mouseLocation;
//		PointF _dpi;
//		System.Timers.Timer _timer;

//		public FullScreenWindow(Rectangle bounds, Point mouseLocation, int delay = 5, bool debug = false)
//		{
//			_debug = debug;
//			System.Diagnostics.Trace.TraceInformation("[{0}]: FullScreenWindow Initializing.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//			InitializeComponent();

//			SuspendLayout();
//			FormBorderStyle = FormBorderStyle.None;
//#if DEBUG
//			TopMost = false;
//#else
//			TopMost = true;
//#endif
//			BackColor = Color.Black;

//			//			SetStyle(ControlStyles.AllPaintingInWmPaint, true);

//			Bounds = bounds;
//			KeyPreview = true;
//			//			DoubleBuffered = true;
//			ResumeLayout();

//			if (delay > 500)
//				delay = 500;

//			_mouseLocation = mouseLocation;
//#if DEBUG
//			System.Diagnostics.Trace.TraceInformation("[{0}] Starting mouse position: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), _mouseLocation);
//#endif

//			_timer = new System.Timers.Timer(Settings.Instance.Interval * 1000);
//			_timer.Elapsed += TimerTicked;

//			using (var g = CreateGraphics())
//				_dpi = new PointF(g.DpiX, g.DpiY);

//			Cursor.Hide();
//		}

//		protected override void OnHandleCreated(EventArgs e)
//		{
//			base.OnHandleCreated(e);

//			_transition = new ImageTransition(this.Handle, ClientRectangle, TransitionEffects.Fade, //Settings.Instance.TransitionEffect,
//				Settings.Instance.RandomizeEffects, Settings.Instance.ImagePaths, true, _dpi);
//		}

//		protected override void OnSizeChanged(EventArgs e)
//		{
//			base.OnSizeChanged(e);

//			if (!IsHandleCreated)
//				return;

//			if (_transition != null)
//				_transition.Dispose();

//			_transition = new ImageTransition(Handle, ClientRectangle, TransitionEffects.Fade,
//				Settings.Instance.RandomizeEffects, Settings.Instance.ImagePaths, true, _dpi);
//		}

//		private void TimerTicked(object sender, System.Timers.ElapsedEventArgs e)
//		{
//			_timer.Stop();
//			_transition.Start();
//			_timer.Start();
//		}

//		protected override void OnClosing(CancelEventArgs e)
//		{
//			base.OnClosing(e);

//			_formClosing = true;
//			e.Cancel = false;
//		}

//		protected override void OnLoad(EventArgs e)
//		{
//			base.OnLoad(e);

//			//BringToFront();
//			//Focus();
//			KeyPreview = true;
//			ShowInTaskbar = false;

//			_transition.Start();
//			_timer.Start();
//		}

//		//protected override void OnPaint(PaintEventArgs e)
//		//{
//		//	base.OnPaint(e);

//		//	_transition.PaintTransition();
//		//}

//		protected override void OnMouseMove(MouseEventArgs e)
//		{
//			//			base.OnMouseMove(e);
//			if (_formClosing || _debug)
//				return;

//			_transition.Stop();
//			var loc = PointToScreen(e.Location);

//#if DEBUG
//			System.Diagnostics.Trace.TraceInformation("[{0}]: Mouse move location: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), loc);
//#endif

//			var deltaX = loc.X - _mouseLocation.X;
//			var deltaY = loc.Y - _mouseLocation.Y;

//			if (deltaX > 3 || deltaX < -3 || deltaY > 3 || deltaY < -3)
//			{
//				Application.Exit();
//			}
//		}

//		protected override void OnMouseUp(MouseEventArgs e)
//		{
//			//			base.OnMouseUp(e);
//			if (_debug) return;

//			_transition.Stop();
//			if (!_formClosing)
//				Application.Exit();
//		}

//		protected override void OnMouseWheel(MouseEventArgs e)
//		{
//			if (_debug) return;

//			_transition.Stop();
//			if (!_formClosing)
//				Application.Exit();
//		}

//		protected override void OnKeyDown(KeyEventArgs e)
//		{
//#if DEBUG
//			System.Diagnostics.Trace.TraceInformation("[{0}]: KeyDown: Keys.{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Enum.GetName(typeof(Keys), e.KeyData));
//#endif
//			if (_debug)
//			{
//				if (e.KeyData == Keys.Escape && !_formClosing)
//					Application.Exit();
//				return;
//			}

//			_transition.Stop();
//			if (!_formClosing)
//				Application.Exit();
//		}

//		protected override bool IsInputKey(Keys keyData)
//		{
//#if DEBUG
//			System.Diagnostics.Trace.TraceInformation("[{0}]: keyData: Keys.{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Enum.GetName(typeof(Keys), keyData));
//#endif
//			return true;
//		}
//	}
//}
