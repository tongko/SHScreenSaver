using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.WIC;

namespace ScreenSaver.ImageTransitions
{
	using D2DDevice = SharpDX.Direct2D1.Device;
	using D2DDeviceContext = SharpDX.Direct2D1.DeviceContext;
	using D3DDevice = SharpDX.Direct3D11.Device;
	using D3DDevice1 = SharpDX.Direct3D11.Device1;
	using D3DDeviceContext1 = SharpDX.Direct3D11.DeviceContext1;
	using D2PixelFormat = SharpDX.Direct2D1.PixelFormat;
	using DXGIDevice2 = SharpDX.DXGI.Device2;
	using DXGIAdapter = SharpDX.DXGI.Adapter;
	using DXGIFactory2 = SharpDX.DXGI.Factory2;
	using WICBitmap = SharpDX.WIC.Bitmap;
	using WICPixelFormat = SharpDX.WIC.PixelFormat;

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

	class ImageTransition
	{
		private D2DDeviceContext _deviceContext;
		private SwapChain _swapChain;
		private ImagingFactory2 _imagingFactory;
		private D2PixelFormat _d2PixelFormat;
		private Guid _wicPixelFormat;

		private IntPtr _hwnd;

		private TransitionEffects _effects;
		private int _index;
		private System.Timers.Timer _timer;
		private bool _randomizeEffects;
		private List<string> _paths;
		private TransitionEffect _currentEffect;

		public ImageTransition(IntPtr hwnd, System.Drawing.Rectangle maxPaintArea, int interval, TransitionEffects effects,
			bool randomizeEffects, IEnumerable<string> imagePaths, bool randomizeImage)
		{
			if (hwnd == IntPtr.Zero)
				throw new ArgumentNullException("container");
			if (maxPaintArea == System.Drawing.Rectangle.Empty)
				throw new ArgumentException("Paint area cannot be empty.", "maxPaintArea");
			if (interval <= 0)
				throw new ArgumentException("Interval between transition must be greater than or equal to 1.");

			_hwnd = hwnd;
			MaxPaintArea = new RectangleF(maxPaintArea.X, maxPaintArea.Y, maxPaintArea.Width, maxPaintArea.Height);
			_randomizeEffects = randomizeEffects;
			_effects = effects;
			_paths = GetImagePaths(imagePaths);
			_index = 0;

			_timer = new System.Timers.Timer(interval * 1000);
			_timer.Elapsed += TimerTicked;
		}

		#region Properties

		public RectangleF MaxPaintArea { get; set; }

		#endregion


		#region Methods

		private void InitializeDirect2D()
		{
			//	Init D3DDevice & Context
			var defaultDevice = new D3DDevice(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
			var d3Device = defaultDevice.QueryInterface<D3DDevice1>();
			var d3Context = d3Device.ImmediateContext1.QueryInterface<D3DDeviceContext1>();

			//	Init DXGI context
			DXGIDevice2 dxgiDevice = d3Device.QueryInterface<DXGIDevice2>();
			DXGIAdapter dxgiAdapter = dxgiDevice.Adapter;
			DXGIFactory2 dxgiFactory2 = dxgiAdapter.GetParent<DXGIFactory2>();

			//	Init Swap Chain & D2D1DeviceContext
			var scDescription = new SwapChainDescription1
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
				SwapEffect = SwapEffect.Discard
			};

			_swapChain = new SwapChain1(dxgiFactory2, d3Device, _hwnd, ref scDescription);
			var d2Device = new D2DDevice(dxgiDevice);
			_deviceContext = new D2DDeviceContext(d2Device, DeviceContextOptions.None);

			_imagingFactory = new ImagingFactory2();

			_d2PixelFormat = new D2PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
			_wicPixelFormat = WICPixelFormat.Format32bppPRGBA;
		}

		public void Start()
		{
			_timer.Start();
		}

		public void Stop()
		{
			_timer.Stop();
		}

		public void PaintTransition(PaintEventArgs e)
		{
			_currentEffect?.Draw(e);
		}

		private void TimerTicked(object sender, System.Timers.ElapsedEventArgs e)
		{
			_timer.Stop();
			var prevIdx = _index;
			var idx = GetNextIndex();
			var back = GetImage(idx);
			var front = GetImage(prevIdx);

			var effects = _effects;
			var info = new TransitionInfo
			{
				BackImage = back,
				FrontImage = front,
				TransitionTime = 1000,
				StepTime = 1000 / 120,   // 60 fps
				WorkingArea = Bounds,
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
			var trans = sender as TransitionEffect;
			if (trans == null)
				return;

			if (trans.State == TransitionState.Finished)
				_timer.Start();
		}

		private void TransitionChanged(object sender, EventArgs e)
		{
			var effect = sender as TransitionEffect;
			if (effect == null)
				return;

			if (_isPreview)
				_previewWnd.Invalidate(effect.MaxArea);
			else
				_fullScrWnd.Invalidate(effect.MaxArea);
		}

		private int GetNextIndex()
		{
			if (++_index == _paths.Count)
				_index = 0;

			return _index;
		}

		private WICBitmap LoadImage(int index)
		{
			var decoder = new BitmapDecoder(_imagingFactory, _paths[index], NativeFileAccess.Read, DecodeOptions.CacheOnDemand);
			var frameDecoder = decoder.GetFrame(0);

			return new WICBitmap(_imagingFactory, frameDecoder, BitmapCreateCacheOption.CacheOnDemand);
		}

		private Image GetImage(int index)
		{
			Image img = null;
			while (img == null)
			{
				try
				{
					img = Image.FromFile(_paths[index]);
				}
				catch
				{
					_paths.RemoveAt(index);
					if (_paths.Count == 0)
						throw new InvalidOperationException("No images specified.");
				}
			}

			return img;
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

		#endregion
	}
}
