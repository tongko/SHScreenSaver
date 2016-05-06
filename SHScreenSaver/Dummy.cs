using System.Windows.Forms;
using SharpDX.Direct3D;

namespace MultiFormTest
{
	using System.ComponentModel;
	using D2DDevice = SharpDX.Direct2D1.Device;
	using D2DDeviceContext = SharpDX.Direct2D1.DeviceContext;
	using D3DDevice = SharpDX.Direct3D11.Device;
	using D3DDevice1 = SharpDX.Direct3D11.Device1;
	using D3DDeviceContext1 = SharpDX.Direct3D11.DeviceContext1;

	public partial class Dummy : Form
	{
		//D3DDevice _device;
		//SwapChain _swapChain;

		//RenderTargetView _view;
		//Texture2D _target;

		System.Timers.Timer _timer;

		//SharpDX.Direct3D11.Device1 _d3dDevice1;
		//SharpDX.Direct3D11.DeviceContext1 _d3dDeviceContext1;
		//SharpDX.Direct2D1.DeviceContext _d2dContext;
		D2DDevice _d2dDevice = null;
		D2DDeviceContext _d2dContext = null;
		SwapChain1 _swapChain;
		Bitmap1 _d2dTarget;

		public Dummy()
		{
			InitializeComponent();
			InitDirectDraw();
			_timer = new System.Timers.Timer(3000);
			_timer.Elapsed += TimerTick;
		}

		private void TimerTick(object sender, System.Timers.ElapsedEventArgs e)
		{
			System.Drawing.Point dpi;
			using (var g = CreateGraphics())
				dpi = new System.Drawing.Point((int)g.DpiX, (int)g.DpiY);

			var x = Screen.PrimaryScreen.BitsPerPixel;
			var prop = new BitmapProperties1(new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
				dpi.X, dpi.Y, BitmapOptions.CannotDraw | BitmapOptions.Target);
		}

		public void Run()
		{

		}

		private void InitDirectDraw()
		{
			var defaultDevice = new D3DDevice(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
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

			//var description = new SwapChainDescription
			//{
			//	BufferCount = 2,
			//	Flags = SwapChainFlags.None,
			//	IsWindowed = true,
			//	ModeDescription = new ModeDescription(ClientSize.Width, ClientSize.Height,
			//		new Rational(60, 1), Format.R8G8B8A8_UNorm),
			//	OutputHandle = Handle,
			//	SampleDescription = new SampleDescription(1, 0),
			//	SwapEffect = SwapEffect.FlipDiscard,
			//	Usage = Usage.RenderTargetOutput
			//};

			//D3DDevice.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport,
			//	description, out _device, out _swapChain);

			//_target = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
			//_view = new RenderTargetView(_device, _target);

			//_device.ImmediateContext.OutputMerger.SetRenderTargets(_view);


			///********************************************************///
			//// Get the default hardware device and enable debugging. Don't care about the available feature level.
			//// DeviceCreationFlags.BgraSupport must be enabled to allow Direct2D interop.
			//var defaultDevice = new SharpDX.Direct3D11.Device(
			//	DriverType.Hardware, DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport);

			//// Query the default device for the supported device and context interfaces.
			//_d3dDevice1 = defaultDevice.QueryInterface<SharpDX.Direct3D11.Device1>();
			//_d3dDeviceContext1 = _d3dDevice1.ImmediateContext.QueryInterface<SharpDX.Direct3D11.DeviceContext1>();

			//// Query for the adapter and more advanced DXGI objects.
			//SharpDX.DXGI.Device2 dxgiDevice2 = _d3dDevice1.QueryInterface<SharpDX.DXGI.Device2>();
			//SharpDX.DXGI.Adapter dxgiAdapter = dxgiDevice2.Adapter;
			//SharpDX.DXGI.Factory2 dxgiFactory2 = dxgiAdapter.GetParent<SharpDX.DXGI.Factory2>();

			//var description = new SwapChainDescription1
			//{
			//	// 0 means to use automatic buffer sizing.
			//	Width = 0,
			//	Height = 0,
			//	// 32 bit RGBA color.
			//	Format = Format.B8G8R8A8_UNorm,
			//	// No stereo (3D) display.
			//	Stereo = false,
			//	// No multisampling.
			//	SampleDescription = new SampleDescription(1, 0),
			//	// Use the swap chain as a render target.
			//	Usage = Usage.RenderTargetOutput,
			//	// Enable double buffering to prevent flickering.
			//	BufferCount = 2,
			//	// No scaling.
			//	Scaling = Scaling.None,
			//	// Flip between both buffers.
			//	SwapEffect = SwapEffect.FlipSequential,
			//};

			//// Generate a swap chain for our window based on the specified description.
			//_swapChain = new SwapChain1(dxgiFactory2, _d3dDevice1, new ComObject(window), ref description);

			//// Get the default Direct2D device and create a context.
			//_d2dDevice = new SharpDX.Direct2D1.Device(dxgiDevice2);
			//_d2dContext = new SharpDX.Direct2D1.DeviceContext(_d2dDevice, DeviceContextOptions.None);

			//// Specify the properties for the bitmap that we will use as the target of our Direct2D operations.
			//// We want a 32-bit BGRA surface with premultiplied alpha.
			//BitmapProperties1 properties = new BitmapProperties1(
			//	new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
			//	DisplayProperties.LogicalDpi, DisplayProperties.LogicalDpi, BitmapOptions.Target | BitmapOptions.CannotDraw);

			//// Get the default surface as a backbuffer and create the Bitmap1 that will hold the Direct2D drawing target.
			//Surface backBuffer = swapChain.GetBackBuffer<Surface>(0);
			//d2dTarget = new Bitmap1(d2dContext, backBuffer, properties);

			//playerBitmap = this.LoadBitmapFromContentFile("/Assets/Bitmaps/player.png");
			//terrainBitmap = this.LoadBitmapFromContentFile("/Assets/Bitmaps/terrain.png");
			//terrainBrush = new BitmapBrush1(d2dContext, terrainBitmap, new BitmapBrushProperties1()
			//{
			//	ExtendModeX = ExtendMode.Wrap,
			//	ExtendModeY = ExtendMode.Wrap,
			//});
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_device.Dispose();
			_swapChain.Dispose();
			_target.Dispose();
			_view.Dispose();

			base.OnClosing(e);
		}


	}
}
