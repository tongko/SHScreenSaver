using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SharpDX;
using SharpDX.IO;

namespace ScreenSaver
{
	using D2D1 = SharpDX.Direct2D1;
	using D3 = SharpDX.Direct3D;
	using D3D11 = SharpDX.Direct3D11;
	using DXGI = SharpDX.DXGI;
	using WIC = SharpDX.WIC;

	class D2DPainter
	{
		private object _sync = new object();

		private D2D1.DeviceContext _deviceContext;
		private D2D1.Device _d2Device;
		private D2D1.BitmapProperties1 _bmpProperties;
		private DXGI.SwapChain1 _swapChain;

		private D2D1.Bitmap1 _target;

		private WIC.BitmapSource _current;
		private WIC.BitmapSource _next;
		private NativeFileStream _currFileStream;
		private NativeFileStream _nextFileStream;
		private WIC.ImagingFactory _factory = new WIC.ImagingFactory();
		private WIC.BitmapDecoder _bmpDecoder;

		private IntPtr _hwnd;

		private int _index;
		private List<string> _paths;

		private float _currOpacity;
		private float _nextOpacity;


		public D2DPainter(IntPtr hwnd, System.Drawing.Rectangle maxPaintArea, System.Drawing.PointF dpi)
		{
			_hwnd = hwnd;

			MaxPaintArea = maxPaintArea.ToDxRectangleF();
			_paths = GetImagePaths(new[] { @"C:\Users\liew343241\Pictures\刘琦宝贝", @"D:\Users\tongko\Pictures" });
			_paths.Shuffle();
			_index = 0;
			DotPerInch = dpi;
			InitDirect2D1();
		}

		public System.Drawing.PointF DotPerInch { get; set; }

		public RectangleF MaxPaintArea { get; set; }

		private void InitDirect2D1()
		{
			lock (_sync)
			{
				var defaultDevice = new D3D11.Device(D3.DriverType.Hardware, D3D11.DeviceCreationFlags.BgraSupport);
				var device = defaultDevice.QueryInterface<D3D11.Device1>();
				var d3dContext = device.ImmediateContext.QueryInterface<D3D11.DeviceContext1>();

				DXGI.Device2 dxgiDevice = device.QueryInterface<DXGI.Device2>();
				DXGI.Adapter dxgiAdapter = dxgiDevice.Adapter;
				DXGI.Factory2 dxgiFactory2 = dxgiAdapter.GetParent<DXGI.Factory2>();

				var description = new DXGI.SwapChainDescription1
				{
					// 0 means to use automatic buffer sizing.
					Width = 0,
					Height = 0,
					// 32 bit RGBA color.
					Format = DXGI.Format.B8G8R8A8_UNorm,
					// No stereo (3D) display.
					Stereo = false,
					// No multisampling.
					SampleDescription = new DXGI.SampleDescription(1, 0),
					// Use the swap chain as a render target.
					Usage = DXGI.Usage.RenderTargetOutput,
					// Enable double buffering to prevent flickering.
					BufferCount = 2,
					// No scaling.
					Scaling = DXGI.Scaling.None,
					// Flip between both buffers.
					SwapEffect = DXGI.SwapEffect.FlipSequential
				};

				//dxgiFactory2.MakeWindowAssociation(_hwnd, DXGI.WindowAssociationFlags.IgnoreAll);
				_swapChain = new DXGI.SwapChain1(dxgiFactory2, device, _hwnd, ref description);
				_swapChain.IsFullScreen = false;

				_d2Device = new D2D1.Device(dxgiDevice);
				_deviceContext = new D2D1.DeviceContext(_d2Device, D2D1.DeviceContextOptions.None);

				_bmpProperties = new D2D1.BitmapProperties1(
					new D2D1.PixelFormat(DXGI.Format.B8G8R8A8_UNorm,
										 D2D1.AlphaMode.Premultiplied),
					DotPerInch.X, DotPerInch.Y,
					D2D1.BitmapOptions.CannotDraw | D2D1.BitmapOptions.Target);

				var backBuffer = _swapChain.GetBackBuffer<DXGI.Surface>(0);
				_target = new D2D1.Bitmap1(_deviceContext, backBuffer, _bmpProperties);
				_deviceContext.Target = _target;
			}
		}

		public void DoPaint()
		{
			var idx = _index;

			if (_currFileStream != null)
				_currFileStream.Dispose();
			_current = GetWICImage(idx, ref _currFileStream);
			var currBmp = D2D1.Bitmap1.FromWicBitmap(_deviceContext, _current);
			var rcCurr = new RectangleF(0f, 0f, currBmp.Size.Width, currBmp.Size.Height);
			ResizeAndCenter(ref rcCurr, MaxPaintArea);

			if (_nextFileStream != null)
				_nextFileStream.Dispose();
			_next = GetWICImage(GetNextIndex(), ref _currFileStream);
			var nextBmp = D2D1.Bitmap1.FromWicBitmap(_deviceContext, _next);
			var rcNext = new RectangleF(0f, 0f, nextBmp.Size.Width, nextBmp.Size.Height);
			ResizeAndCenter(ref rcNext, MaxPaintArea);

			_deviceContext.BeginDraw();

			_deviceContext.Clear(new SharpDX.Mathematics.Interop.RawColor4(0f, 0f, 0f, 1f));
			_deviceContext.DrawBitmap(currBmp, rcCurr, 1.0f, D2D1.BitmapInterpolationMode.Linear);

			_deviceContext.EndDraw();
			_swapChain.Present(1, DXGI.PresentFlags.None);

			_currOpacity = 1.0f;
			_nextOpacity = 0.01f;
			float currStep = 0f, stepTime = 1000f / 60f;
			var timer = new System.Timers.Timer(stepTime);
			timer.Elapsed += (o, e) =>
			{
				timer.Stop();
				lock (_sync)
				{
					var fade = Math.Min(1f, (float)currStep / 1000f);
					_currOpacity = 1f - (_nextOpacity = fade);
					currStep += stepTime;
				}
				if (currStep > 1000f)
					_nextOpacity = 2.0f;
				else
					timer.Start();
			};
			timer.Start();

			while (_nextOpacity <= 1.0f)
			{
				lock (_sync)
				{
					_deviceContext.BeginDraw();
					_deviceContext.Clear(new SharpDX.Mathematics.Interop.RawColor4(0f, 0f, 0f, 1f));
					_deviceContext.DrawBitmap(currBmp, rcCurr, _currOpacity, D2D1.BitmapInterpolationMode.Linear);
					_deviceContext.DrawBitmap(nextBmp, rcNext, _nextOpacity, D2D1.BitmapInterpolationMode.Linear);
					_deviceContext.EndDraw();
					_swapChain.Present(1, DXGI.PresentFlags.None);
				}
			}

			_deviceContext.BeginDraw();
			_deviceContext.Clear(new SharpDX.Mathematics.Interop.RawColor4(0f, 0f, 0f, 1f));
			_deviceContext.DrawBitmap(nextBmp, rcNext, 1.0f, D2D1.BitmapInterpolationMode.Linear);
			_deviceContext.EndDraw();
			_swapChain.Present(1, DXGI.PresentFlags.None);
		}

		private WIC.BitmapSource GetWICImage(int index, ref NativeFileStream fileStream)
		{
			var idx = index;
			WIC.FormatConverter converter = null;

			lock (_sync)
			{
				while (converter == null)
				{
					try
					{
						fileStream = new NativeFileStream(_paths[idx], NativeFileMode.Open, NativeFileAccess.Read);

						_bmpDecoder = new WIC.BitmapDecoder(_factory, fileStream,
							WIC.DecodeOptions.CacheOnDemand);
					}
					catch (Exception)
					{
						_paths.RemoveAt(idx);
						if (_paths.Count == 0)
							throw new InvalidOperationException("No valid image files specified.");
						continue;
					}

					converter = new WIC.FormatConverter(_factory);
					converter.Initialize(_bmpDecoder.GetFrame(0),
						WIC.PixelFormat.Format32bppPRGBA);
				}

				return converter;
			}
		}

		private int GetNextIndex()
		{
			if (++_index == _paths.Count)
				_index = 0;

			return _index;
		}

		protected void ResizeAndCenter(ref SharpDX.RectangleF bounds, SharpDX.RectangleF? area = null)
		{
			var rc = area ?? MaxPaintArea;
			var maxSize = rc.Size;
			var ratio = Math.Min((float)maxSize.Width / bounds.Width, (float)maxSize.Height / bounds.Height);
			var newSize = new SharpDX.Size2F(bounds.Width * ratio, bounds.Height * ratio);

			var cx = (rc.Width - newSize.Width) / 2f;
			var cy = (rc.Height - newSize.Height) / 2f;

			bounds.Location = new SharpDX.Vector2(cx, cy);
			bounds.Size = newSize;
		}

		private List<string> GetImagePaths(IEnumerable<string> dirs)
		{
			var imgPaths = new List<string>();

			foreach (var item in dirs)
			{
				if (!Directory.Exists(item))
					continue;
				var paths = Directory.GetFiles(item);
				if (paths == null || paths.Length == 0)
					continue;

				foreach (var p in paths)
					imgPaths.Add(p);
			}

			return imgPaths;
		}


	}
}
