using SharpDX.Direct2D1;
using SharpDX.DXGI;

namespace SinHing.DirectXLibs
{
	using D2D1 = SharpDX.Direct2D1;
	using Factory = SharpDX.Direct2D1.Factory;

	public class Direct2DApp : Direct3D11App
	{
		#region Fields


		#endregion


		#region Properties

		public Factory Factory2D { get; private set; }

		public SharpDX.DirectWrite.Factory FactoryDW { get; private set; }

		public RenderTarget RenderTarget2D { get; private set; }

		public SolidColorBrush SceneColorBrush { get; private set; }

		#endregion


		#region Methods

		protected override void Initialize(DxAppConfiguration config)
		{
			base.Initialize(config);

			Factory2D = new Factory();
			using (var surface = BackBuffer.QueryInterface<Surface>())
			{
				RenderTarget2D = new RenderTarget(Factory2D, surface,
					new RenderTargetProperties(new PixelFormat(Format.Unknown, D2D1.AlphaMode.Premultiplied)));
			}

			RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;

			FactoryDW = new SharpDX.DirectWrite.Factory();

			SceneColorBrush = new SolidColorBrush(RenderTarget2D, SharpDX.Color.White);
		}

		protected override void BeginDraw()
		{
			base.BeginDraw();
			RenderTarget2D.BeginDraw();
		}

		protected override void EndDraw()
		{
			base.EndDraw();
			RenderTarget2D.EndDraw();
		}

		#endregion
	}
}
