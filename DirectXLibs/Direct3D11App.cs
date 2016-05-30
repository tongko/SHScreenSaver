using System;
using System.Windows.Forms;

namespace SinHing.DirectXLibs
{
	using D3 = SharpDX.Direct3D;
	using D3D11 = SharpDX.Direct3D11;
	using DXGI = SharpDX.DXGI;

	public class Direct3D11App : DxApp
	{
		#region Fields

		private DXGI.SwapChain _swapChain;

		#endregion


		#region Properties

		public D3D11.Device Device { get; private set; }

		public D3D11.Texture2D BackBuffer { get; private set; }

		public D3D11.RenderTargetView BackBufferView { get; private set; }


		#endregion


		#region Methods

		protected override void BeginRun()
		{
			base.BeginDraw();
			Device.ImmediateContext.Rasterizer.SetViewport(
				new SharpDX.Viewport(0, 0, Config.Width, Config.Height));
			Device.ImmediateContext.OutputMerger.SetTargets(BackBufferView);
		}

		protected override void Draw(TickTimer time)
		{
			_swapChain.Present(Config.WaitVerticalBlanking ? 1 : 0, DXGI.PresentFlags.None);
		}

		protected override void EndRun()
		{
		}

		protected override void Initialize(DxAppConfiguration config)
		{
			var desc = new DXGI.SwapChainDescription
			{
				BufferCount = 1,
				ModeDescription = new DXGI.ModeDescription(
					config.Width, config.Height, new DXGI.Rational(60, 1), DXGI.Format.R8G8B8A8_UNorm),
				IsWindowed = true,
				OutputHandle = DisplayHandle,
				SampleDescription = new DXGI.SampleDescription(1, 0),
				SwapEffect = DXGI.SwapEffect.Discard,
				Usage = DXGI.Usage.RenderTargetOutput
			};

			D3D11.Device device;
			DXGI.SwapChain swapChain;

			D3D11.Device.CreateWithSwapChain(
				D3.DriverType.Hardware,
				D3D11.DeviceCreationFlags.BgraSupport,
				new[] { D3.FeatureLevel.Level_10_0 },
				desc, out device, out swapChain);
			Device = device;
			_swapChain = swapChain;

			var factory = _swapChain.GetParent<DXGI.Factory>();
			factory.MakeWindowAssociation(DisplayHandle, DXGI.WindowAssociationFlags.IgnoreAll);

			BackBuffer = D3D11.Resource.FromSwapChain<D3D11.Texture2D>(_swapChain, 0);
			BackBufferView = new D3D11.RenderTargetView(Device, BackBuffer);
		}

		protected override void LoadContent()
		{
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
		}

		protected override void OnResize(EventArgs e)
		{
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(TickTimer time)
		{
		}

		#endregion
	}
}
