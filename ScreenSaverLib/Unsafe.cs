using System;
using System.Runtime.InteropServices;

namespace SinHing.ScreenSaver
{
	using HINSTANCE = IntPtr;
	using HMENU = IntPtr;
	using HWND = IntPtr;
	using LPVOID = IntPtr;
	using LRESULT = IntPtr;

	internal delegate LRESULT WndProcHandler(HWND hwnd, int msg, UIntPtr wParam, IntPtr lParam);
	internal delegate void TimerProcHandler([In] HWND hwnd, [In] uint uMsg, [In] UIntPtr idEvent,
		[In] uint dwTime);

	internal static class Unsafe
	{
		#region Fields

		public static readonly UIntPtr VK_ESCAPE = new UIntPtr(0x1B);

		public const int CS_VREDRAW = 0x0001;
		public const int CS_HREDRAW = 0x0002;
		public const int CS_SAVEBITS = 0x0800;
		public const int CS_DBLCLKS = 0x0008;

		public const int WS_CAPTION = 0xc00000;
		public const int WS_CHILD = 0x40000000;
		public const int WS_MAXIMIZEBOX = 0x10000;
		public const int WS_MINIMIZEBOX = 0x20000;
		public const int WS_OVERLAPPED = 0x0;
		public const int WS_SIZEFRAME = 0x40000;
		public const int WS_SYSMENU = 0x80000;
		public const int WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;

		public const int WS_EX_TOPMOST = 0x00000008;

		public const int GWLP_HWNDPARENT = -8;
		public const int GWLP_USERDATA = -21;

		public const int WM_NCCREATE = 0x0081;
		public const int WM_NCDESTROY = 0x0082;
		public const int WM_DESTROY = 0x0002;
		public const int WM_ACTIVATEAPP = 0x001C;
		public const int WM_ACTIVATE = 0x0006;
		public const int WM_CLOSE = 0x0010;
		public const int WM_PAINT = 0x000F;
		public const int WM_MOUSEMOVE = 0x0200F;
		public const int WM_SYSKEYDOWN = 0x0104F;
		public const int WM_KEYDOWN = 0x0100F;


		public const int IDC_ARROW = 32512;

		public const int ERROR_CLASS_ALREADY_EXISTS = 1410;

		public const int SW_SHOW = 5;

		#endregion


		#region Structures

		[StructLayout(LayoutKind.Sequential)]
		public struct PAINTSTRUCT
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
		public struct RECT
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

			public int Width { get { return right - left; } }

			public int Height { get { return bottom - top; } }

			public static RECT FromRectangle(System.Drawing.Rectangle rect)
			{
				return new RECT { left = rect.X, top = rect.Y, right = rect.Right, bottom = rect.Bottom };
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CREATESTRUCT
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
		public struct WNDCLASSEX
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
		public struct POINT
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MSG
		{
			public HWND hwnd;
			public int message;
			public UIntPtr wParam;
			public IntPtr lParam;
			public int time;
			public POINT pt;
		}

		#endregion


		#region Methods

		public static int LOWORD(UIntPtr l)
		{
			return (int)(l.ToUInt32()) & 0xFFFF;
		}

		public static int LOWORD(IntPtr l)
		{
			return (int)(l.ToInt32()) & 0xFFFF;
		}

		public static int HIWORD(IntPtr l)
		{
			return ((short)(((l.ToInt32()) >> 16) & 0xffff));
		}

		[DllImport("user32.dll", EntryPoint = "RegisterClassExW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

		[DllImport("user32.dll", EntryPoint = "CreateWindowExW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern HWND CreateWindowEx(
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
		public static extern LRESULT DefWindowProc(HWND hwnd, int msg, UIntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern IntPtr GetWindowLong(HWND hwnd, int index);

		/// <summary>
		/// Changes an attribute of the specified window. The function also sets a value at the specified offset in the extra window memory. 
		/// </summary>
		/// <param name="hwnd">A handle to the window and, indirectly, the class to which the window belongs. The SetWindowLongPtr function
		/// fails if the process that owns the window specified by the hWnd parameter is at a higher process privilege in the UIPI hierarchy
		/// than the process the calling thread resides in.</param>
		/// <param name="index">The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes
		/// of extra window memory, minus the size of an integer. To set any other value, specify one of the following values.</param>
		/// <param name="newLong">The replacement value.</param>
		/// <returns>If the function succeeds, the return value is the previous value of the specified offset.
		/// If the function fails, the return value is zero.To get extended error information, call GetLastError.
		/// If the previous value is zero and the function succeeds, the return value is zero, but the function does not clear the last error
		/// information. To determine success or failure, clear the last error information by calling SetLastError with 0, then call
		/// SetWindowLongPtr.Function failure will be indicated by a return value of zero and a GetLastError result that is nonzero.
		/// </returns>
		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern IntPtr SetWindowLongPtr([In] HWND hwnd, [In] int index, [In] IntPtr newLong);

		[DllImport("user32.dll", EntryPoint = "LoadCursorW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern IntPtr LoadCursor([In, Optional] HINSTANCE hInstance, [In] ushort lpCursorName);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern IntPtr CreateSolidBrush([In] int crColor);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool GetClientRect([In] HWND hWnd, [Out] out RECT lpRect);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool ShowWindow([In] HWND hwnd, [In] int nCmdShow);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool UpdateWindow([In] HWND hwnd);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = false)]
		public static extern int DispatchMessage(ref MSG m);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = false)]
		public static extern int GetMessage([Out] out MSG m, [In, Optional] HWND hwnd, [In] int wMsgFilterMin, [In] int wMsgFilterMax);

		[DllImport("user32.dll", EntryPoint = "PeekMessageW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool PeekMessage([Out] out MSG m, [In, Optional] HWND hwnd, [In] int wMsgFilterMin, [In] int wMsgFilterMax, [In] int wRemoveMsg);

		[DllImport("Winmm.dll", EntryPoint = "timeGetTime", CallingConvention = CallingConvention.StdCall, SetLastError = false)]
		public static extern uint TimeGetTime();

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern IntPtr GetDC([In] HWND hwnd);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern int ReleaseDC([In] HWND hwnd, [In] IntPtr HDC);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern void PostQuitMessage([In] int nExitCode);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]

		public static extern bool DestroyWindow([In] HWND hwnd);

		[DllImport("user32.dll", EntryPoint = "PostMessageW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool PostMessage([In, Optional] HWND hwnd, [In] int msg, [In] UIntPtr wParam, [In] IntPtr lParam);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern IntPtr BeginPaint([In] HWND hwnd, [Out] out PAINTSTRUCT lpPaint);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool EndPaint([In] HWND hWnd, [In] ref PAINTSTRUCT lpPaint);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool InvalidateRect([In] HWND hwnd, [In] ref RECT lpRect, [In] bool erase);

		[DllImport("user32.dll", EntryPoint = "RegisterClassExW", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool GetClassInfoEx([In, Optional] HINSTANCE hinst, [In] string lpszClass, [Out] out WNDCLASSEX lpwcx);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool SetWindowPos([In] HWND hWnd, [In, Optional] HWND hWndInsertAfter, [In] int x, [In] int y,
			[In] int cx, [In] int cy, [In, MarshalAs(UnmanagedType.SysUInt)] SWP uFlags);

		/// <summary>
		/// Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates 
		/// that are relative to the upper-left corner of the screen.
		/// </summary>
		/// <param name="hWnd">A handle to the window.</param>
		/// <param name="lpRect">A pointer to a <see cref="RECT"/> structure that receives the screen coordinates of the upper-left and 
		/// lower-right corners of the window.</param>
		/// <returns>If the function succeeds, the return value is nonzero.<br/>
		/// If the function fails, the return value is zero. To get extended error information, call <see cref="System.Runtime.InteropServices.Marshal.GetLastWin32Error"/>.
		/// </returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern bool GetWindowRect([In] HWND hWnd, [Out] out RECT lpRect);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern HWND SetParent([In] HWND hWndChild, [In, Optional] HWND hWndNewParent);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern int ShowCursor([In] bool bShow);

		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false)]
		public static extern uint SetTimer([In, Optional] HWND hWnd, [In] uint nIDEvent, [In] uint uElapse,
			[In, Optional] TimerProcHandler lpTimerFunc);

		#endregion
	}

	internal static class HWNDInsertAfter
	{
		public static readonly IntPtr

		NOTOPMOST = new IntPtr(-2),
		BROADCAST = new IntPtr(0xffff),
		TOPMOST = new IntPtr(-1),
		TOP = new IntPtr(0),
		BOTTOM = new IntPtr(1);
	}

	[Flags]
	internal enum SWP : uint
	{
		NOSIZE = 0x0001,
		NOMOVE = 0x0002,
		NOZORDER = 0x0004,
		NOREDRAW = 0x0008,
		NOACTIVATE = 0x0010,
		DRAWFRAME = 0x0020,
		FRAMECHANGED = 0x0020,
		SHOWWINDOW = 0x0040,
		HIDEWINDOW = 0x0080,
		NOCOPYBITS = 0x0100,
		NOOWNERZORDER = 0x0200,
		NOREPOSITION = 0x0200,
		NOSENDCHANGING = 0x0400,
		DEFERERASE = 0x2000,
		ASYNCWINDOWPOS = 0x4000,
	}
}
