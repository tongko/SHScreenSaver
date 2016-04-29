﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HINSTANCE = System.IntPtr;
using HMENU = System.IntPtr;
using HWND = System.IntPtr;
using LPVOID = System.IntPtr;
using LRESULT = System.IntPtr;

namespace ScreenSaver
{
	class PreviewWindow : IDisposable
	{
		const string WndClassName = "SinHing.ScreenSaver.PreviewWindow";
		const int CS_VREDRAW = 0x0001;
		const int CS_HREDRAW = 0x0002;

		const int WS_CAPTION = 0xc00000;
		const int WS_CHILD = 0x40000000;
		const int WS_MAXIMIZEBOX = 0x10000;
		const int WS_MINIMIZEBOX = 0x20000;
		const int WS_OVERLAPPED = 0x0;
		const int WS_SIZEFRAME = 0x40000;
		const int WS_SYSMENU = 0x80000;
		const int WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;

		const int WS_EX_TOPMOST = 0x00000008;

		const int GWLP_USERDATA = -21;

		const int WM_NCCREATE = 0x0081;
		const int WM_NCDESTROY = 0x0082;
		const int WM_DESTROY = 0x0002;
		const int WM_ACTIVATEAPP = 0x001C;
		const int WM_ACTIVATE = 0x0006;
		const int WM_CLOSE = 0x0010;
		const int WM_PAINT = 0x000F;

		const int IDC_ARROW = 32512;

		const int ERROR_CLASS_ALREADY_EXISTS = 1410;

		const int SW_SHOW = 5;

		private HWND _hInstance;
		private HWND _parent;
		private HWND _hwnd;
		private bool disposedValue = false; // To detect redundant calls

		private PaintPictures _paint;

		private bool _flag = false;
		private System.Drawing.Rectangle _bounds;

		#region ctor

		public PreviewWindow(HWND parent)
		{
			_parent = parent;
			_hInstance = Process.GetCurrentProcess().Handle;
		}

		#endregion


		#region Methods

		public void Run()
		{
			Trace.TraceInformation("[{0}]: Run PreviewWindow.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

			var wc = new WNDCLASSEX();
			wc.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));

			wc.style = CS_HREDRAW | CS_VREDRAW;
			wc.lpfnWndProc = new WndProcHandler(WndProc);
			wc.cbClsExtra = 0;
			wc.cbWndExtra = 0;
			wc.hInstance = _hInstance;
			wc.hIcon = IntPtr.Zero;
			wc.hCursor = LoadCursor(IntPtr.Zero, IDC_ARROW);
			wc.hbrBackground = CreateSolidBrush(0);
			wc.lpszMenuName = null;
			wc.lpszClassName = WndClassName;
			wc.hIconSm = IntPtr.Zero;

			ushort cls_atom = RegisterClassEx(ref wc);
			int error = Marshal.GetLastWin32Error();
			if (cls_atom == 0 && error != ERROR_CLASS_ALREADY_EXISTS)
				throw new InvalidOperationException("Failed to register window class.");

			RECT rc = new RECT(0, 0, 100, 100);
			GetClientRect(_parent, out rc);

			Trace.TraceInformation("[{0}]: Paint Area: {1}, {2}, {3}, {4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), rc.left, rc.top, rc.right, rc.bottom);
			_paint = new PaintPictures(System.Drawing.Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom));
			_paint.TimerTick += PaintTimerTick;
			_bounds = _paint.Bounds;

			_hwnd = CreateWindowEx(WS_EX_TOPMOST, WndClassName, null, WS_CHILD, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, _parent,
				IntPtr.Zero, _hInstance, IntPtr.Zero);
			if (_hwnd == IntPtr.Zero)
			{
				Trace.TraceError("[{0}]: CreateWindowEx failed with last win32 error: {1}.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Marshal.GetLastWin32Error());
				return;
			}

			ShowWindow(_hwnd, SW_SHOW);
			UpdateWindow(_hwnd);

			//PaintTimerTick(_paint, EventArgs.Empty);

			_paint.TickTimer.Start();

			Trace.TraceInformation("[{0}]: PreviewWindow created and visible.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

			MSG msg, msg1;
			while (true)
			{
				if (PeekMessage(out msg, IntPtr.Zero, 0, 0, 0))
				{
					var result = GetMessage(out msg1, IntPtr.Zero, 0, 0);
					if (result <= 0)
						break;

					DispatchMessage(ref msg);
				}
				else
				{
					if (!_flag)
					{
						Trace.TraceInformation("[{0}]: Special Paint event", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
						PaintTimerTick(_paint, PaintPictureEventArgs.Empty);
					}
				}
			}

			Trace.TraceInformation("[{0}]: Exiting message pump.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
		}

		private void PaintTimerTick(object sender, PaintPictureEventArgs e)
		{
			Trace.TraceInformation("[{0}]: Paint event", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
			var effect = e.Effect;
			effect.TransitionStart += Effect_TransitionStart;
			effect.TransitionStep += Effect_TransitionStep;
			effect.TransitionStop += Effect_TransitionStop;

			effect.Start();

			_flag = true;
		}

		private void Effect_TransitionStop(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Effect_TransitionStep(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Effect_TransitionStart(object sender, EventArgs e)
		{
			var rc = RECT.FromRectangle(_bounds);
			InvalidateRect(_hwnd, ref rc, false);
		}

		protected LRESULT WndProc(HWND hwnd, int msg, UIntPtr wParam, IntPtr lParam)
		{
			switch (msg)
			{
				case WM_PAINT:
					if (_paint == null)
						break;

					PAINTSTRUCT ps;
					var hdc = BeginPaint(hwnd, out ps);
					using (var g = System.Drawing.Graphics.FromHdc(hdc))
					{
					}
					EndPaint(hwnd, ref ps);
					break;
				case WM_ACTIVATE:
					if (LOWORD(wParam) == 0)
						PostMessage(_hwnd, WM_CLOSE, UIntPtr.Zero, IntPtr.Zero);
					break;
				case WM_ACTIVATEAPP:
					if (wParam == UIntPtr.Zero)
						PostMessage(_hwnd, WM_CLOSE, wParam, IntPtr.Zero);
					break;
				case WM_DESTROY:
					PostQuitMessage(0);
					break;
				case WM_CLOSE:
					DestroyWindow(_hwnd);
					return IntPtr.Zero;
				default:
					return DefWindowProc(hwnd, msg, wParam, lParam);
			}

			return IntPtr.Zero;
		}

		private int LOWORD(UIntPtr l)
		{
			var val = l.ToUInt32();

			return (int)val & 0xFFFF;
		}

		#endregion


		#region IDisposable Support

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}

				if (_hwnd != IntPtr.Zero)
				{
					DestroyWindow(_hwnd);
					_hwnd = IntPtr.Zero;
				}

				disposedValue = true;
			}
		}

		~PreviewWindow()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion


		#region PInvoke

		delegate LRESULT WndProcHandler(HWND hwnd, int msg, UIntPtr wParam, IntPtr lParam);

		[StructLayout(LayoutKind.Sequential)]
		struct PAINTSTRUCT
		{
			public IntPtr hdc;
			public bool fErase;
			public RECT rcPaint;
			public bool fRestore;
			public bool fIncUpdate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] rgbReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			public RECT(int l, int t, int r, int b)
			{
				left = l;
				top = t;
				right = r;
				bottom = b;
			}

			public int left;
			public int top;
			public int right;
			public int bottom;

			public static RECT FromRectangle(System.Drawing.Rectangle rect)
			{
				return new RECT { left = rect.X, top = rect.Y, right = rect.Right, bottom = rect.Bottom };
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		struct CREATESTRUCT
		{
			public LPVOID lpCreateParams;
			public HINSTANCE hInstance;
			public HMENU hMenu;
			public HWND hwndParent;
			public int cy;
			public int cx;
			public int y;
			public int x;
			public int style;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszClass;
			public int dwExStyle;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct WNDCLASSEX
		{
			[MarshalAs(UnmanagedType.U4)]
			public int cbSize;
			[MarshalAs(UnmanagedType.U4)]
			public int style;
			public WndProcHandler lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszMenuName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszClassName;
			public IntPtr hIconSm;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct POINT
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct MSG
		{
			public HWND hwnd;
			public int message;
			public UIntPtr wParam;
			public IntPtr lParam;
			public int time;
			public POINT pt;
		}

		[DllImport("user32.dll", EntryPoint = "RegisterClassExW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

		[DllImport("user32.dll", EntryPoint = "CreateWindowExW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern HWND CreateWindowEx(
			[In] int dwExStyle,
			[MarshalAs(UnmanagedType.LPWStr)]
			[In, Optional] string lpClassName,
			[MarshalAs(UnmanagedType.LPWStr)]
			[In, Optional] string lpWindowName,
			[In] int dwStyle,
			[In] int x,
			[In] int y,
			[In] int nWidth,
			[In] int nHeight,
			[In, Optional] HWND hWndParent,
			[In, Optional] HMENU hMenu,
			[In, Optional] HINSTANCE hInstance,
			[In, Optional] LPVOID lpParam);

		[DllImport("user32.dll", EntryPoint = "DefWindowProcW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern LRESULT DefWindowProc(HWND hwnd, int msg, UIntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern IntPtr GetWindowLong(HWND hwnd, int index);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern int SetWindowLong(HWND hwnd, int index, IntPtr newLong);

		[DllImport("user32.dll", EntryPoint = "LoadCursorW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern IntPtr LoadCursor([In, Optional] HINSTANCE hInstance, [In] ushort lpCursorName);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern IntPtr CreateSolidBrush([In] int crColor);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern bool GetClientRect([In] HWND hWnd, [Out] out RECT lpRect);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern bool ShowWindow([In] HWND hwnd, [In] int nCmdShow);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern bool UpdateWindow([In] HWND hwnd);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = false)]
		private static extern int DispatchMessage(ref MSG m);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = false)]
		private static extern int GetMessage([Out] out MSG m, [In, Optional] HWND hwnd, [In] int wMsgFilterMin, [In] int wMsgFilterMax);

		[DllImport("user32.dll", EntryPoint = "PeekMessageW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		private static extern bool PeekMessage([Out] out MSG m, [In, Optional] HWND hwnd, [In] int wMsgFilterMin, [In] int wMsgFilterMax, [In] int wRemoveMsg);

		[DllImport("Winmm.dll", EntryPoint = "timeGetTime", CallingConvention = CallingConvention.StdCall, SetLastError = false)]
		static extern uint TimeGetTime();

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern IntPtr GetDC([In] HWND hwnd);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern int ReleaseDC([In] HWND hwnd, [In] IntPtr HDC);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern void PostQuitMessage([In] int nExitCode);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]

		static extern bool DestroyWindow([In] HWND hwnd);

		[DllImport("user32.dll", EntryPoint = "PostMessageW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern bool PostMessage([In, Optional] HWND hwnd, [In] int msg, [In] UIntPtr wParam, [In] IntPtr lParam);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern IntPtr BeginPaint([In] HWND hwnd, [Out] out PAINTSTRUCT lpPaint);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern bool EndPaint([In] HWND hWnd, [In] ref PAINTSTRUCT lpPaint);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		static extern bool InvalidateRect([In] HWND hwnd, [In] ref RECT lpRect, [In] bool erase);

		#endregion
	}
}