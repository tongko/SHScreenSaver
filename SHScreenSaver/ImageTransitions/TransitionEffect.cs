﻿using System;
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
		Timer _tickTimer;

		protected TransitionEffect(TransitionInfo info)
		{
			SyncRoot = new object();
			BackImage = info.BackImage;
			FrontImage = info.FrontImage;
			Bounds = info.WorkingArea;
			TransitionTime = info.TransitionTime;
			StepTime = info.StepTime;

			Canvas = new Bitmap(Bounds.Width, Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			((Bitmap)Canvas).SetResolution(BackImage.HorizontalResolution, BackImage.VerticalResolution);
		}

		public EventHandler TransitionStart;

		public EventHandler TransitionStep;

		public EventHandler TransitionStop;

		protected Image FrontImage { get; set; }

		protected Image BackImage { get; set; }

		protected Image Canvas { get; private set; }

		protected Rectangle Bounds { get; set; }

		protected int TransitionTime { get; set; }

		protected int StepTime { get; set; }

		protected bool Finished { get; set; }

		protected int CurrentStep { get; private set; }

		protected System.Timers.Timer TickTimer { get; set; }

		public TransitionState State { get; protected set; }

		protected object SyncRoot { get; private set; }

		private void DoRaiseEvent(TransitionState state, EventHandler handler)
		{
			lock (SyncRoot)
			{
				State = state;
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

		protected void ResizeAndCenter(ref Rectangle bounds, Rectangle? area = null)
		{
			var rc = area ?? Bounds;
			var maxSize = rc.Size;
			var ratio = Math.Min((double)maxSize.Width / bounds.Width, (double)maxSize.Height / bounds.Height);
			var newSize = new Size((int)(bounds.Width * ratio), (int)(bounds.Height * ratio));

			var cx = (rc.Width - newSize.Width) / 2;
			var cy = (rc.Height - newSize.Height) / 2;

			bounds.Location = new Point(cx, cy);
			bounds.Size = newSize;
		}

		public virtual void Draw(System.Windows.Forms.PaintEventArgs e)
		{
			lock (SyncRoot)
			{
				if (Canvas == null)
					return;

				e.Graphics.DrawImage(Canvas, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
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
					if (State == TransitionState.Transitioning)
						Step();
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
				//case TransitionEffects.ZoomIn:
				//case TransitionEffects.ZoomOut:
				case TransitionEffects.SlideLeft:
					return new TransitionSlide(info, SlideDirection.Left);
				case TransitionEffects.SlideRight:
					return new TransitionSlide(info, SlideDirection.Right);
				case TransitionEffects.SlideUp:
					return new TransitionSlide(info, SlideDirection.Up);
				case TransitionEffects.SlideDown:
					return new TransitionSlide(info, SlideDirection.Down);
				//case TransitionEffects.All:
				//	list.Add(new TransitionNone(info));
				//	list.Add(new TransitionFade(info));
				//	list.Add(new TransitionDissolve(info));
				//	list.Add(new TransitionSlide(info, SlideDirection.Left));
				//	list.Add(new TransitionSlide(info, SlideDirection.Right));
				//	list.Add(new TransitionSlide(info, SlideDirection.Up));
				//	list.Add(new TransitionSlide(info, SlideDirection.Down));
				//	break;
				default:
					return new TransitionNone(info);
			}
		}
	}
}
