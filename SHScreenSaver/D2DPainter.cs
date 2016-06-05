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
		private WIC.ImagingFactory _factory = new WIC.ImagingFactory();
		private WIC.BitmapDecoder _bmpDecoder;

		private IntPtr _hwnd;

		private int _index;
		private List<string> _paths;


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
			Debug.WriteLine("Begin Paint:");
			Debug.Indent();

			var deviceContext = _deviceContext;
			deviceContext.BeginDraw();
			deviceContext.Clear(new SharpDX.Mathematics.Interop.RawColor4(0f, 0f, 0f, 1f));

			var idx = GetNextIndex();
			//var bmp = GetWICImage(GetNextIndex());

			#region TestCode

			var fileStream = new NativeFileStream(_paths[idx], NativeFileMode.Open, NativeFileAccess.Read);
			_bmpDecoder = new WIC.BitmapDecoder(_factory, fileStream,
				WIC.DecodeOptions.CacheOnDemand);
			fileStream.Dispose();

			var converter = new WIC.FormatConverter(_factory);
			converter.Initialize(_bmpDecoder.GetFrame(0),
				WIC.PixelFormat.Format32bppPRGBA);
			var bmp = converter;

			#endregion

			var rectBack = new RectangleF(0f, 0f, bmp.Size.Width, bmp.Size.Height);
			Debug.WriteLine("Before resize and center: {0}", new object[] { rectBack.ToDebugString() });
			ResizeAndCenter(ref rectBack);
			Debug.WriteLine("After resize and center:  {0}", new object[] { rectBack.ToDebugString() });

			var image = D2D1.Bitmap1.FromWicBitmap(deviceContext, bmp);
			deviceContext.DrawBitmap(image, rectBack, 1.0f, D2D1.BitmapInterpolationMode.Linear);

			deviceContext.EndDraw();

			//var rc = rectBack; //.ToRawRectangleF(); //rectBack;
			//var parameter = new DXGI.PresentParameters
			//{
			//	DirtyRectangles = new[] {
			//		new SharpDX.Mathematics.Interop.RawRectangle(
			//			(int)rc.Left, (int)rc.Top, (int)rc.Right, (int)rc.Bottom) },
			//	ScrollOffset = new SharpDX.Mathematics.Interop.RawPoint(0, 0),
			//};

			_swapChain.Present(1, DXGI.PresentFlags.None);
			//_swapChain.Present(1, DXGI.PresentFlags.DoNotWait, parameter);

			Debug.Unindent();
			Debug.WriteLine("End Paint.");
		}

		//private SharpDX.Direct2D1.Bitmap1 GetBitmap(int index)
		//{
		//	var idx = index;
		//	SharpDX.Direct2D1.Bitmap1 bmp = null;
		//	WIC.FormatConverter converter = null;
		//	NativeFileStream fileStream = null;

		//	lock (_sync)
		//	{
		//		while (converter == null)
		//		{
		//			try
		//			{
		//				fileStream = new NativeFileStream(_paths[idx], NativeFileMode.Open, NativeFileAccess.Read);

		//				_bmpDecoder = new WIC.BitmapDecoder(_factory, fileStream,
		//					WIC.DecodeOptions.CacheOnDemand);
		//			}
		//			catch (Exception)
		//			{
		//				_paths.RemoveAt(idx);
		//				if (_paths.Count == 0)
		//					throw new InvalidOperationException("No valid image files specified.");
		//				continue;
		//			}
		//			finally
		//			{
		//				Utilities.Dispose(ref fileStream);
		//			}

		//			converter = new WIC.FormatConverter(_factory);
		//			converter.Initialize(_bmpDecoder.GetFrame(0),
		//				WIC.PixelFormat.Format32bppPRGBA);

		//			bmp = D2D1.Bitmap1.FromWicBitmap(_deviceContext, converter);
		//			//Utilities.Dispose(ref bitmapDecoder);
		//		}
		//	}

		//	return bmp;
		//}

		private WIC.BitmapSource GetWICImage(int index)
		{
			var idx = index;
			WIC.FormatConverter converter = null;
			NativeFileStream fileStream = null;

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
					finally
					{
						Utilities.Dispose(ref fileStream);
					}

					converter = new WIC.FormatConverter(_factory);
					converter.Initialize(_bmpDecoder.GetFrame(0),
						WIC.PixelFormat.Format32bppPRGBA);
				}
				//				WIC.BitmapSource bmp = null;
				//RETRY:
				//				try
				//				{
				//					while (bmp == null)
				//					{
				//						var imgFactory = new WIC.ImagingFactory();
				//						var decoder = new WIC.BitmapDecoder(imgFactory,
				//							_paths[idx], WIC.DecodeOptions.CacheOnDemand);
				//						var convertor = new WIC.FormatConverter(imgFactory);
				//						convertor.Initialize(decoder.GetFrame(0),
				//							WIC.PixelFormat.Format32bppPRGBA);

				//						bmp = convertor;
				//					}
				//				}
				//				catch (Exception)
				//				{
				//					_paths.RemoveAt(idx);
				//					if (_paths.Count == 0)
				//						throw new InvalidOperationException("No valid image files specified.");
				//					goto RETRY;
				//				}

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
