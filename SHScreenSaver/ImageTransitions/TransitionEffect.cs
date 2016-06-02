using System;
using System.ComponentModel;
using System.Drawing;
using System.Timers;

namespace ScreenSaver.ImageTransitions
{
	enum TransitionState
	{
		None,
		Started,
		Transitioning,
		Finished
	}

	abstract class TransitionEffect
	{
		#region Fields

		Timer _tickTimer;
		SharpDX.Direct2D1.DeviceContext _deviceContext;

		#endregion


		#region Ctor

		protected TransitionEffect(TransitionInfo info)
		{
			SyncRoot = new object();
			BackImage = info.BackImage;
			FrontImage = info.FrontImage;
			ClientArea = info.WorkingArea;
			MaxArea = CalculateMaxArea(BackImage, FrontImage);
			TransitionTime = info.TransitionTime;
			StepTime = info.StepTime;
			_deviceContext = info.DeviceContext;

			//Canvas = new Bitmap(MaxArea.Width, MaxArea.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			//((Bitmap)Canvas).SetResolution(BackImage.HorizontalResolution, BackImage.VerticalResolution);
		}

		#endregion


		#region Events

		public event EventHandler TransitionStart;

		public event EventHandler TransitionStep;

		public event EventHandler TransitionStop;

		#endregion


		#region Properties

		protected SharpDX.WIC.BitmapSource FrontImage { get; set; }

		protected SharpDX.WIC.BitmapSource BackImage { get; set; }

		protected Image Canvas { get; set; }

		protected SharpDX.RectangleF ClientArea { get; set; }

		protected int TransitionTime { get; set; }

		protected int StepTime { get; set; }

		protected bool Finished { get; set; }

		protected int CurrentStep { get; private set; }

		protected System.Timers.Timer TickTimer { get; set; }

		public TransitionState State { get; protected set; }

		public SharpDX.RectangleF MaxArea { get; protected set; }

		protected object SyncRoot { get; private set; }

		#endregion


		#region Methods

		private SharpDX.RectangleF CalculateMaxArea(SharpDX.WIC.BitmapSource img1, SharpDX.WIC.BitmapSource img2)
		{
			var rc1 = new SharpDX.RectangleF(0, 0, img1.Size.Width, img1.Size.Height);
			ResizeAndCenter(ref rc1);
			var rc2 = new SharpDX.RectangleF(0, 0, img2.Size.Width, img2.Size.Height);
			ResizeAndCenter(ref rc2);

			return new SharpDX.RectangleF(Math.Min(rc1.X, rc2.X), Math.Min(rc1.Y, rc2.Y),
				Math.Max(rc1.Width, rc2.Width), Math.Max(rc1.Height, rc2.Height));
		}

		protected void ResizeAndCenter(ref SharpDX.RectangleF bounds, SharpDX.RectangleF? area = null)
		{
			var rc = area ?? ClientArea;
			var maxSize = rc.Size;
			var ratio = Math.Min((float)maxSize.Width / bounds.Width, (float)maxSize.Height / bounds.Height);
			var newSize = new SharpDX.Size2F(bounds.Width * ratio, bounds.Height * ratio);

			var cx = (rc.Width - newSize.Width) / 2f;
			var cy = (rc.Height - newSize.Height) / 2f;

			bounds.Location = new SharpDX.Vector2(cx, cy);
			bounds.Size = newSize;
		}

		public void Draw(SharpDX.Direct2D1.DeviceContext deviceContext)
		{
			if (deviceContext == null)
				throw new ArgumentNullException("deviceContext");

			deviceContext.BeginDraw();

			DoDraw(deviceContext);

			deviceContext.EndDraw();
		}

		protected abstract void DoDraw(SharpDX.Direct2D1.DeviceContext deviceContext);

		private void DoRaiseEvent(TransitionState state, EventHandler handler)
		{
			lock (SyncRoot)
			{
				State = state;

				try
				{
					Draw(_deviceContext);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("Something went wrong! {0}", e);
				}

				if (handler == null)
					return;

				foreach (var d in handler.GetInvocationList())
				{
					var s = d.Target as ISynchronizeInvoke;
					if (s != null && s.InvokeRequired)
						s.BeginInvoke(d, new object[] { this, EventArgs.Empty });
					else
						d.DynamicInvoke(this, EventArgs.Empty);
				}
			}
		}

		protected virtual void OnStart()
		{
			DoRaiseEvent(TransitionState.Started, TransitionStart);
		}

		protected virtual void OnStep()
		{
			DoRaiseEvent(TransitionState.Transitioning, TransitionStep);
		}

		protected virtual void OnStop()
		{
			DoRaiseEvent(TransitionState.Finished, TransitionStop);
		}

		public virtual void Start()
		{
			lock (SyncRoot)
			{
				OnStart();

				TickTimer = new System.Timers.Timer(StepTime);
				TickTimer.Elapsed += TimerTick;
				TickTimer.Start();
			}
		}

		public virtual void Step()
		{
			lock (SyncRoot)
			{
				CurrentStep += StepTime;
				if (CurrentStep > TransitionTime)
					Stop();
				else
					OnStep();
			}
		}

		public virtual void Stop()
		{
			lock (SyncRoot)
			{
				if (TickTimer != null)
				{
					TickTimer.Stop();
					TickTimer.Dispose();
				}

				OnStop();
			}
		}

		protected virtual void TimerTick(object sender, ElapsedEventArgs e)
		{
			try
			{
				lock (SyncRoot)
				{
					TickTimer.Stop();
					if (State == TransitionState.Started)
						State = TransitionState.Transitioning;
					if (State == TransitionState.Transitioning)
						Step();
					TickTimer.Start();
				}
			}
			catch (Exception) { }
		}

		public static TransitionEffect Create(TransitionEffects effects,
			TransitionInfo info)
		{
			//var list = new System.Collections.Generic.List<TransitionEffect>();
			switch (effects)
			{
				case TransitionEffects.None:
					return new TransitionNone(info);
				case TransitionEffects.Fade:
					return new TransitionFade(info);
				case TransitionEffects.Dissolve:
					return new TransitionDissolve(info);
				case TransitionEffects.ZoomIn:
				case TransitionEffects.ZoomOut:
				case TransitionEffects.SlideLeft:
					return new TransitionSlide(info, SlideDirection.Left);
				case TransitionEffects.SlideRight:
					return new TransitionSlide(info, SlideDirection.Right);
				case TransitionEffects.SlideUp:
					return new TransitionSlide(info, SlideDirection.Up);
				case TransitionEffects.SlideDown:
					return new TransitionSlide(info, SlideDirection.Down);
				default:
					if (effects == TransitionEffects.All && info.RandomizeEffect)
					{
						var values = Enum.GetValues(typeof(TransitionEffects));
						var random = new Random();
						effects = (TransitionEffects)values.GetValue(random.Next(values.Length - 1));
					}

					return Create(effects, info);
			}
		}

		#endregion
	}
}
