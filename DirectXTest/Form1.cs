using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.WIC;

namespace DirectXTest
{
	using System.ComponentModel;
	using D2DDevice = SharpDX.Direct2D1.Device;
	using D2DDeviceContext = SharpDX.Direct2D1.DeviceContext;
	using D3DDevice = SharpDX.Direct3D11.Device;
	using D3DDevice1 = SharpDX.Direct3D11.Device1;
	using D3DDeviceContext1 = SharpDX.Direct3D11.DeviceContext1;
	using PixelFormat = SharpDX.Direct2D1.PixelFormat;

	public partial class Form1 : Form
	{
		private object _sync = new object();
		System.Timers.Timer _timer;

		D2DDevice _d2dDevice = null;
		D2DDeviceContext _d2dContext = null;
		SwapChain1 _swapChain;
		Bitmap1 _d2dTarget;
		Bitmap1 _d2dSource;
		BitmapProperties1 _bmpProps;
		BitmapBrush1 _brush;

		System.Drawing.Point _dpi;
		int _currentIndex = 0;
		List<string> _paths;

		bool _doExit = false;

		public Form1()
		{
			InitializeComponent();

			SuspendLayout();
			WindowState = FormWindowState.Maximized;
			FormBorderStyle = FormBorderStyle.None;
			KeyPreview = true;
			ResumeLayout(false);

			using (var g = CreateGraphics())
				_dpi = new System.Drawing.Point((int)g.DpiX, (int)g.DpiY);

			_timer = new System.Timers.Timer(3000);
			_timer.Elapsed += TimerTick;

			_paths = new List<string>(System.IO.Directory.GetFiles(@"C:\Users\liew343241\Pictures\刘琦宝贝\"));

			InitDirectDraw();
		}

		private void TimerTick(object sender, System.Timers.ElapsedEventArgs e)
		{
			_timer.Stop();
			_d2dSource = GetCurrentImage();
			_brush = new BitmapBrush1(_d2dContext, _d2dSource, new BitmapBrushProperties1
			{
				ExtendModeX = ExtendMode.Wrap,
				ExtendModeY = ExtendMode.Wrap
			});
			_timer.Start();
		}

		private void InitDirectDraw()
		{
			lock (_sync)
			{
				var defaultDevice = new D3DDevice(DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport);
				var device = defaultDevice.QueryInterface<D3DDevice1>();
				var d3dContext = device.ImmediateContext.QueryInterface<D3DDeviceContext1>();

				SharpDX.DXGI.Device2 dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device2>();
				SharpDX.DXGI.Adapter dxgiAdapter = dxgiDevice.Adapter;
				SharpDX.DXGI.Factory2 dxgiFactory2 = dxgiAdapter.GetParent<SharpDX.DXGI.Factory2>();

				var description = new SwapChainDescription1
				{
					// 0 means to use automatic buffer sizing.
					Width = 0,
					Height = 0,
					// 32 bit RGBA color.
					Format = Format.B8G8R8A8_UNorm,
					// No stereo (3D) display.
					Stereo = false,
					// No multisampling.
					SampleDescription = new SampleDescription(1, 0),
					// Use the swap chain as a render target.
					Usage = Usage.RenderTargetOutput,
					// Enable double buffering to prevent flickering.
					BufferCount = 2,
					// No scaling.
					Scaling = Scaling.None,
					// Flip between both buffers.
					SwapEffect = SwapEffect.FlipSequential,
				};

				_swapChain = new SwapChain1(dxgiFactory2, device, Handle, ref description);
				_d2dDevice = new D2DDevice(dxgiDevice);
				_d2dContext = new D2DDeviceContext(_d2dDevice, DeviceContextOptions.None);

				_bmpProps = new BitmapProperties1(new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
					_dpi.X, _dpi.Y, BitmapOptions.CannotDraw | BitmapOptions.Target);

				var backBuffer = _swapChain.GetBackBuffer<Surface>(0);
				_d2dTarget = new Bitmap1(_d2dContext, backBuffer, _bmpProps);
				_d2dSource = GetCurrentImage();

				_brush = new BitmapBrush1(_d2dContext, _d2dSource, new BitmapBrushProperties1
				{
					ExtendModeX = ExtendMode.Wrap,
					ExtendModeY = ExtendMode.Wrap
				});
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			lock (_sync)
				_doExit = true;

			_d2dDevice.Dispose();
			_swapChain.Dispose();
			_d2dTarget.Dispose();

			base.OnClosing(e);
		}

		private Bitmap1 GetCurrentImage()
		{
			lock (_sync)
			{
				var idx = _currentIndex++;
				if (_currentIndex == _paths.Count)
					_currentIndex = 0;

				Bitmap1 image = null;
				while (image == null)
				{
					image = LoadBitmapFromContentFile(_paths[idx]);
					if (image == null)
					{
						_paths.RemoveAt(idx);
						if (_paths.Count == 0)
							throw new System.IndexOutOfRangeException();
					}
				}

				return image;
			}
		}

		private Bitmap1 LoadBitmapFromContentFile(string filePath)
		{
			Bitmap1 newBitmap;

			lock (_sync)
			{
				// Neccessary for creating WIC objects.
				ImagingFactory imagingFactory = new ImagingFactory();
				NativeFileStream fileStream = new NativeFileStream(filePath, NativeFileMode.Open, NativeFileAccess.Read);

				// Used to read the image source file.
				BitmapDecoder bitmapDecoder = null;
				try
				{
					bitmapDecoder = new BitmapDecoder(imagingFactory, fileStream, DecodeOptions.CacheOnDemand);
				}
				catch (SharpDXException)
				{
					return null;
				}

				// Get the first frame of the image.
				BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

				// Convert it to a compatible pixel format.
				FormatConverter converter = new FormatConverter(imagingFactory);
				converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA);

				// Create the new Bitmap1 directly from the FormatConverter.
				newBitmap = Bitmap1.FromWicBitmap(_d2dContext, converter);

				Utilities.Dispose(ref bitmapDecoder);
				Utilities.Dispose(ref fileStream);
				Utilities.Dispose(ref imagingFactory);

				return newBitmap;
			}
		}

		public void Run()
		{
			Show();
			_timer.Start();

			while (!_doExit)
			{
				lock (_sync)
				{
					// Set the Direct2D drawing target.
					_d2dContext.Target = _d2dTarget;

					// Clear the target and draw some geometry with the brushes we created. 
					_d2dContext.BeginDraw();
					_d2dContext.Clear(new SharpDX.Mathematics.Interop.RawColor4(0f, 0f, 0f, 255f));

					// Calculate the center of the screen
					//int halfWidth = _swapChain.Description.ModeDescription.Width / 2;
					//int halfHeight = _swapChain.Description.ModeDescription.Height / 2;

					// Translate the origin of coordinates for drawing the bitmap filled rectangle
					//_d2dContext.Transform = Matrix3x2.Translation(halfWidth - 350, halfHeight);
					//_d2dContext.FillRectangle(new RectangleF(0, 0, ClientSize.Width, ClientSize.Height), _brush);
					var bmpRc = new RectangleF(0f, 0f, _d2dSource.Size.Width, _d2dSource.Size.Height);
					var maxArea = new RectangleF(0f, 0f, Size.Width, Size.Height);
					ResizeAndCenter(ref bmpRc, maxArea);
					_d2dContext.DrawBitmap(_d2dSource, bmpRc, 1f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

					//_d2dContext.DrawImage(_d2dSource);

					// Translate again for drawing the player bitmap
					//_d2dContext.Transform = Matrix3x2.Translation(halfWidth, halfHeight - playerBitmap.Size.Height);
					//_d2dContext.DrawBitmap(playerBitmap, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

					_d2dContext.EndDraw();

					// Present the current buffer to the screen.
					_swapChain.Present(1, PresentFlags.None);
				}

				Application.DoEvents();
			}
		}

		protected void ResizeAndCenter(ref RectangleF bounds, RectangleF? area = null)
		{
			var rc = area ?? new RectangleF(0, 0, ClientSize.Width, ClientSize.Height);
			var maxSize = rc.Size;
			var ratio = System.Math.Min((double)maxSize.Width / bounds.Width, (double)maxSize.Height / bounds.Height);
			var newSize = new Size2F((int)(bounds.Width * ratio), (int)(bounds.Height * ratio));

			var cx = (rc.Width - newSize.Width) / 2f;
			var cy = (rc.Height - newSize.Height) / 2f;

			bounds.Location = new Vector2(cx, cy);
			bounds.Size = newSize;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			Close();
		}

		protected override bool IsInputKey(Keys keyData)
		{
			return true;
		}
	}
}
