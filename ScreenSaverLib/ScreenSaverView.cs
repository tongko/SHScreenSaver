using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SinHing.ScreenSaver
{
	public class ScreenSaverView : IDisposable
	{
		#region Fields

		private const string WndClassName = "SH.ScreenSaver.View";
		private bool _disposedValue = false; // To detect redundant calls
		private bool _handleCreated = false;
		private Size _size;
		private Point _location;
		private Point _mouse;
		private bool _initialized = false;
		private readonly bool _debugMode;
		private CallbackInfo _callback;
		private System.Timers.Timer _timer;
		private bool _eventArgsEmpty = true;
		private ScreenSaverCallbackEventArgs _eventArgs;

		#endregion


		#region Constructor

		internal ScreenSaverView(CallbackInfo callback, IntPtr parent, Rectangle bounds, int delay, bool debug = false)
		{
			_callback = callback;
			HInstance = System.Diagnostics.Process.GetCurrentProcess().Handle;
			Parent = parent;
			IsPreview = parent != IntPtr.Zero;
			if (IsPreview)
			{
				Unsafe.RECT rc;
				Unsafe.GetClientRect(Parent, out rc);
				Bounds = Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
			}
			else
				Bounds = bounds;
			Handle = IntPtr.Zero;
			_debugMode = debug;
			_timer = new System.Timers.Timer(delay);
			_timer.Elapsed += (o, e) =>
			{
				_timer.Stop();
				DoRaiseEvent(this, callback.TimerCallback,
					_eventArgsEmpty ? new ScreenSaverCallbackEventArgs(this)
					: _eventArgs);
				_timer.Start();
			};
		}

		#endregion


		#region Events

		public event EventHandler Closing;
		public event EventHandler Closed;
		public event EventHandler HandleCreated;
		public event EventHandler HandleDestroyed;
		public event EventHandler SizeChanged;

		#endregion


		#region Properties

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle(_location, _size);
			}
			set
			{
				_location = value.Location;
				_size = value.Size;
				if (_handleCreated)
					Unsafe.SetWindowPos(Handle, HWNDInsertAfter.TOP, _location.X, _location.Y, _size.Width, _size.Height,
						SWP.SHOWWINDOW);

				//OnSizeChanged();
			}
		}

		public Size ClientSize
		{
			get { return _size; }
			set
			{
				_size = value;
				if (_handleCreated)
					Unsafe.SetWindowPos(Handle, HWNDInsertAfter.TOP, _location.X, _location.Y, _size.Width, _size.Height,
						SWP.SHOWWINDOW);

				//OnSizeChanged();
			}
		}

		public IntPtr Handle { get; private set; }

		public IntPtr Parent { get; private set; }

		public IntPtr HInstance { get; private set; }

		public bool IsPreview { get; set; }

		public bool IsDebug { get; set; }

		#endregion


		#region Methods

		internal void Close()
		{
			_timer.Stop();
			_timer.Dispose();
			Unsafe.PostMessage(Handle, Unsafe.WM_CLOSE, UIntPtr.Zero, IntPtr.Zero);
		}

		private void DoRaiseEvent(object sender, EventHandler<ScreenSaverCallbackEventArgs> handler,
			ScreenSaverCallbackEventArgs args)
		{
			if (handler == null)
				return;

			foreach (var d in handler.GetInvocationList())
			{
				var s = d.Target as System.ComponentModel.ISynchronizeInvoke;
				if (s != null && s.InvokeRequired)
					s.BeginInvoke(d, new object[] { sender, args });
				else
					d.DynamicInvoke(sender, args);
			}
		}

		public virtual void Initialize()
		{
			RegisterWindowClass();
			CreateWindow();

			_initialized = true;
			_timer.Start();
		}

		public void Update()
		{
			if (!_initialized)
				Initialize();

			if (!_handleCreated)
				return;

			Unsafe.UpdateWindow(Handle);
			Unsafe.ShowWindow(Handle, Unsafe.SW_SHOW);
		}

		private void RegisterWindowClass()
		{
			var wc = new Unsafe.WNDCLASSEX();
			if (Unsafe.GetClassInfoEx(HInstance, WndClassName, out wc))
				return;

			wc.cbSize = Marshal.SizeOf(typeof(Unsafe.WNDCLASSEX));

			wc.style = Unsafe.CS_VREDRAW | Unsafe.CS_HREDRAW | Unsafe.CS_SAVEBITS | Unsafe.CS_DBLCLKS;
			wc.lpfnWndProc = new WndProcHandler(WndProc);
			wc.cbClsExtra = 0;
			wc.cbWndExtra = 0;
			wc.hInstance = HInstance;
			wc.hIcon = IntPtr.Zero;
			wc.hCursor = Unsafe.LoadCursor(IntPtr.Zero, Unsafe.IDC_ARROW);
			wc.hbrBackground = Unsafe.CreateSolidBrush(0);
			wc.lpszMenuName = null;
			wc.lpszClassName = WndClassName;
			wc.hIconSm = IntPtr.Zero;

			ushort cls_atom = Unsafe.RegisterClassEx(ref wc);
			int error = Marshal.GetLastWin32Error();
			if (cls_atom == 0 && error != Unsafe.ERROR_CLASS_ALREADY_EXISTS)
				throw new InvalidOperationException("Failed to register window class.");
		}

		private void CreateWindow()
		{
			var style = IsPreview ? Unsafe.WS_CHILD : Unsafe.WS_OVERLAPPEDWINDOW;
			if (IsPreview)
			{
				Unsafe.RECT rc;
				Unsafe.GetClientRect(Parent, out rc);
				Handle = Unsafe.CreateWindowEx(0, WndClassName, null,
					  Unsafe.WS_CHILD, rc.left, rc.top, rc.Width, rc.Height, Parent, IntPtr.Zero,
					  HInstance, IntPtr.Zero);
			}
			else
			{
				if (_debugMode)
				{
					Handle = Unsafe.CreateWindowEx(0, WndClassName, null,
						0, 50, 50, 640, 360, IntPtr.Zero, IntPtr.Zero, HInstance, IntPtr.Zero);
				}
				else
					Handle = Unsafe.CreateWindowEx(0, WndClassName, null,
						  Unsafe.WS_EX_TOPMOST, 0, 0, _size.Width, _size.Height, Parent, IntPtr.Zero,
						  HInstance, IntPtr.Zero);
			}
			if (Handle == IntPtr.Zero)
				throw new InvalidOperationException(string.Format(
					"[{0}]: CreateWindowEx failed with last win32 error: {1}.",
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Marshal.GetLastWin32Error()));

			_eventArgs = new ScreenSaverCallbackEventArgs(this);
			_eventArgsEmpty = false;
			_handleCreated = true;
			OnHandleCreated();
		}

		public System.Windows.Forms.IWin32Window GetWin32Window()
		{
			return _handleCreated
				? System.Windows.Forms.NativeWindow.FromHandle(Handle)
				: null;
		}

		public System.Windows.Forms.Form GetWindowsForm()
		{
			return _handleCreated
				? System.Windows.Forms.Control.FromHandle(Handle) as System.Windows.Forms.Form
				: null;
		}

		protected virtual void OnHandleCreated()
		{
			DoRaiseEvent(this, _callback.CreateCallback, _eventArgs);
		}

		protected virtual void OnHandleDestroyed()
		{
			DoRaiseEvent(this, _callback.DestroyCallback, _eventArgs);
		}

		//protected virtual void OnSizeChanged()
		//{
		//	DoRaiseEvent(this, SizeChanged, EventArgs.Empty);
		//}

		//protected virtual void OnClosing()
		//{
		//	DoRaiseEvent(this, Closing, EventArgs.Empty);
		//}

		//protected virtual void OnClosed()
		//{
		//	DoRaiseEvent(this, Closed, EventArgs.Empty);
		//}

		protected virtual IntPtr WndProc(IntPtr hwnd, int msg, UIntPtr wParam, IntPtr lParam)
		{
			switch (msg)
			{
				case Unsafe.WM_MOUSEMOVE:
					if (IsPreview || _debugMode)
						break;
					var pt = new Point(Unsafe.LOWORD(lParam), Unsafe.HIWORD(lParam));
					if (_mouse.X == 0 && _mouse.Y == 0)
					{
						_mouse.X = pt.X;
						_mouse.Y = pt.Y;
					}
					else
					{
						if ((Math.Abs(pt.X - _mouse.X) > 3) || (Math.Abs(pt.Y - _mouse.Y) > 3))
							Close();
					}
					break;
				case Unsafe.WM_ACTIVATE:
					if (Unsafe.LOWORD(wParam) == 0)
						Close();
					break;
				case Unsafe.WM_ACTIVATEAPP:
					if (wParam == UIntPtr.Zero)
						Close();
					break;
				case Unsafe.WM_SYSKEYDOWN:
				case Unsafe.WM_KEYDOWN:
					if (IsPreview)
						break;
					if (_debugMode)
					{
						if (wParam == Unsafe.VK_ESCAPE)
							Close();
						else
							break;
					}
					Close();
					break;
				case Unsafe.WM_CLOSE:
					//OnClosing();
					Unsafe.DestroyWindow(Handle);
					OnHandleDestroyed();
					//OnClosed();
					break;
				case Unsafe.WM_DESTROY:
					Dispose();
					break;
				default:
					return Unsafe.DefWindowProc(hwnd, msg, wParam, lParam);
			}

			return IntPtr.Zero;
		}

		#endregion


		#region IDisposable Support

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.
				Unsafe.PostQuitMessage(0);
				_disposedValue = true;
			}
		}

		//TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~ScreenSaverView()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
