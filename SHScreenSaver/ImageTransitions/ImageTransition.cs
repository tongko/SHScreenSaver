using System;
using System.Collections.Generic;
using System.IO;

using SharpDX;

namespace ScreenSaver.ImageTransitions
{
	using D2D1 = SharpDX.Direct2D1;
	using D3 = SharpDX.Direct3D;
	using D3D11 = SharpDX.Direct3D11;
	using DXGI = SharpDX.DXGI;
	using WIC = SharpDX.WIC;

	[Flags]
	enum TransitionEffects
	{
		None,
		Fade,
		Dissolve,
		ZoomIn,
		ZoomOut,
		SlideLeft,
		SlideRight,
		SlideUp,
		SlideDown,
		All = Fade | Dissolve | ZoomIn | ZoomOut | SlideLeft | SlideRight | SlideUp | SlideDown
	}

	class ImageTransition : IDisposable
	{
		#region Fields

		private object _sync = new object();

		private D2D1.DeviceContext _deviceContext;
		private D2D1.Device _d2Device;
		private D2D1.BitmapProperties1 _bmpProperties;
		private DXGI.SwapChain1 _swapChain;

		private D2D1.Bitmap1 _target;

		private WIC.BitmapSource _current;
		private WIC.BitmapSource _next;

		private IntPtr _hwnd;

		private TransitionEffects _effects;
		private int _index;
		private bool _randomizeEffects;
		private List<string> _paths;
		private TransitionEffect _currentEffect;

		#endregion


		#region Constructors

		public ImageTransition(IntPtr hwnd, System.Drawing.Rectangle maxPaintArea, TransitionEffects effects,
			bool randomizeEffects, IEnumerable<string> imagePaths, bool randomizeImages, System.Drawing.PointF dpi)
		{
			if (hwnd == IntPtr.Zero)
				throw new ArgumentNullException("container");
			if (maxPaintArea == System.Drawing.Rectangle.Empty)
				throw new ArgumentException("Paint area cannot be empty.", "maxPaintArea");

			_hwnd = hwnd;
			MaxPaintArea = maxPaintArea.ToDxRectangleF();
			_randomizeEffects = randomizeEffects;
			_effects = effects;
			_paths = GetImagePaths(imagePaths);
			if (randomizeImages)
				_paths.Shuffle();
			_index = 0;

			DotPerInch = dpi;

			InitDirect2D1();
		}

		#endregion


		#region Properties

		public System.Drawing.PointF DotPerInch { get; set; }

		public RectangleF MaxPaintArea { get; set; }

		public object SyncRoot { get { return _sync; } }

		#endregion


		#region Methods

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
					SwapEffect = DXGI.SwapEffect.FlipSequential,
				};

				dxgiFactory2.MakeWindowAssociation(_hwnd, DXGI.WindowAssociationFlags.IgnoreAll);
				_swapChain = new DXGI.SwapChain1(dxgiFactory2, device, _hwnd, ref description);

				_d2Device = new D2D1.Device(dxgiDevice);
				_deviceContext = new D2D1.DeviceContext(_d2Device, D2D1.DeviceContextOptions.None);

				_bmpProperties = new D2D1.BitmapProperties1(
					new D2D1.PixelFormat(DXGI.Format.B8G8R8A8_UNorm,
										 D2D1.AlphaMode.Premultiplied),
					DotPerInch.X, DotPerInch.Y,
					D2D1.BitmapOptions.CannotDraw | D2D1.BitmapOptions.Target);

				var backBuffer = _swapChain.GetBackBuffer<DXGI.Surface>(0);
				System.Diagnostics.Debug.Assert(backBuffer != null);
				_target = new D2D1.Bitmap1(_deviceContext, backBuffer, _bmpProperties);
				System.Diagnostics.Debug.Assert(_target.NativePointer != IntPtr.Zero);
			}
		}

		public void Start()
		{
			lock (_sync)
			{
				if (_current == null)
					_current = GetWICImage(_index);
				_next = GetWICImage(GetNextIndex());

				var info = new TransitionInfo
				{
					BackImage = _next,
					FrontImage = _current,
					TransitionTime = 1000,
					StepTime = 1000 / 60, // 60 fps
					WorkingArea = MaxPaintArea,
					RandomizeEffect = _randomizeEffects,
					DeviceContext = _deviceContext,
				};

				_currentEffect = TransitionEffect.Create(_effects, info);
				_currentEffect.Start();
			}
		}

		public void Stop()
		{
			lock (_sync)
			{
				_currentEffect.Stop();
			}
		}

		public void PaintTransition()
		{
			try
			{
				_currentEffect?.Draw(_deviceContext);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Something is wrong! {0}", e);
			}

			var rc = _currentEffect.MaxArea;
			var parameter = new DXGI.PresentParameters
			{
				DirtyRectangles = new[] {
					new SharpDX.Mathematics.Interop.RawRectangle((int)rc.Left, (int)rc.Top, (int)rc.Right, (int)rc.Bottom) },
				ScrollOffset = new SharpDX.Mathematics.Interop.RawPoint(0, 0),
			};

			_swapChain.Present(1, DXGI.PresentFlags.DoNotWait, parameter);
		}

		private void TimerTicked(object sender, System.Timers.ElapsedEventArgs e)
		{
			var prevIdx = _index;
			var idx = GetNextIndex();
			var back = GetWICImage(idx);
			var front = GetWICImage(prevIdx);

			var effects = _effects;
			var info = new TransitionInfo
			{
				BackImage = back,
				FrontImage = front,
				TransitionTime = 1000,
				StepTime = 1000 / 120,   // 60 fps
				WorkingArea = MaxPaintArea,
				RandomizeEffect = _randomizeEffects
			};

			_currentEffect = TransitionEffect.Create(effects, info);
			_currentEffect.TransitionStart += TransitionChanged;
			_currentEffect.TransitionStep += TransitionChanged;
			_currentEffect.TransitionStop += TransitionStop;
			_currentEffect.Start();
		}

		private void TransitionStop(object sender, EventArgs e)
		{
		}

		private void TransitionChanged(object sender, EventArgs e)
		{
		}

		private WIC.BitmapSource GetWICImage(int index)
		{
			var idx = index;
			lock (_sync)
			{
				WIC.BitmapSource bmp = null;
RETRY:
				try
				{
					while (bmp == null)
					{
						var imgFactory = new WIC.ImagingFactory();
						var decoder = new WIC.BitmapDecoder(imgFactory,
							_paths[idx], WIC.DecodeOptions.CacheOnDemand);
						var convertor = new WIC.FormatConverter(imgFactory);
						convertor.Initialize(decoder.GetFrame(0),
							WIC.PixelFormat.Format32bppPRGBA);
						//WIC.PixelFormat.Format32bppPRGBA, WIC.BitmapDitherType.None,
						//null, 0.0, WIC.BitmapPaletteType.Custom);

						bmp = convertor;
					}
				}
				catch (Exception)
				{
					_paths.RemoveAt(idx);
					if (_paths.Count == 0)
						throw new InvalidOperationException("No valid image files specified.");
					goto RETRY;
				}

				return bmp;
			}
		}

		private int GetNextIndex()
		{
			if (++_index == _paths.Count)
				_index = 0;

			return _index;
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

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}

				if (_target != null)
					_target.Dispose();
				if (_current != null)
					_current.Dispose();
				if (_next != null)
					_next.Dispose();

				if (_swapChain != null)
					_swapChain.Dispose();
				if (_deviceContext != null)
					_deviceContext.Dispose();
				if (_d2Device != null)
					_d2Device.Dispose();

				disposedValue = true;
			}
		}

		~ImageTransition()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		#endregion

		#endregion
	}
}
