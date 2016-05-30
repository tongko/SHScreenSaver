using System;
using System.Windows.Forms;
using SharpDX.Windows;

namespace SinHing.DirectXLibs
{
	public abstract class DxApp : IDisposable
	{
		private readonly TickTimer _clock = new TickTimer();
		private FormWindowState _windowState;
		private Form _form;
		private float _frameAccumulator;
		private int _frameCount;

		#region Properties

		public IntPtr DisplayHandle { get { return _form.Handle; } }

		public DxAppConfiguration Config { get; private set; }

		public float FrameDelta { get; private set; }

		public float FramePerSecond { get; private set; }

		protected System.Drawing.Size RenderingSize { get { return _form.ClientSize; } }

		#endregion


		#region Methods

		protected abstract void Initialize(DxAppConfiguration config);

		protected virtual Form CreateForm(DxAppConfiguration config)
		{
			return new RenderForm(config.Title)
			{
				ClientSize = new System.Drawing.Size(config.Width, config.Height)
			};
		}

		public void Run()
		{
			Run(new DxAppConfiguration());
		}

		public void Run(DxAppConfiguration config)
		{
			Config = config ?? new DxAppConfiguration();
			_form = CreateForm(Config);

			var isFormClosed = false;
			var formIsResizing = false;

			_form.MouseClick += (sender, e) => { OnMouseClick(e); };
			_form.KeyDown += (sender, e) => { OnKeyDown(e); };
			_form.KeyUp += (sender, e) => { OnKeyUp(e); };
			_form.Resize += (o, e) =>
			{
				if (_form.WindowState != _windowState)
					OnResize(e);

				_windowState = _form.WindowState;
			};

			_form.ResizeBegin += (o, e) => { formIsResizing = true; };
			_form.ResizeEnd += (o, e) => { formIsResizing = false; OnResize(e); };
			_form.FormClosed += (o, e) => { isFormClosed = true; };

			LoadContent();

			_clock.Start();
			BeginRun();
			RenderLoop.Run(_form, () =>
			{
				if (isFormClosed)
					return;

				OnUpdate();
				if (!formIsResizing)
					Render();
			});

			UnloadContent();
			EndRun();

			Dispose();
		}

		public void Exit()
		{
			_form.Close();
		}

		protected abstract void OnMouseClick(MouseEventArgs e);

		protected virtual void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Exit();
		}

		protected abstract void OnKeyUp(KeyEventArgs e);

		protected abstract void OnResize(EventArgs e);

		protected abstract void LoadContent();

		protected abstract void UnloadContent();

		protected abstract void Update(TickTimer time);

		protected abstract void Draw(TickTimer time);

		protected abstract void BeginRun();

		protected abstract void EndRun();

		protected virtual void BeginDraw()
		{
		}

		protected virtual void EndDraw()
		{
		}

		private void OnUpdate()
		{
			FrameDelta = (float)_clock.Update();
			Update(_clock);
		}

		private void Render()
		{
			_frameAccumulator += FrameDelta;
			++_frameCount;
			if (_frameAccumulator >= 1.0f)
			{
				FramePerSecond = _frameCount / _frameAccumulator;

				_form.Text = Config.Title + " - FPS: " + FramePerSecond;
				_frameAccumulator = 0.0f;
				_frameCount = 0;
			}

			BeginDraw();
			Draw(_clock);
			EndDraw();
		}

		#endregion


		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					if (_form != null)
						_form.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~DxApp() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
