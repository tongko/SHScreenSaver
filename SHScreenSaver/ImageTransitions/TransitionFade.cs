using System;
using System.Drawing.Imaging;

namespace ScreenSaver.ImageTransitions
{
	class TransitionFade : TransitionEffect
	{
		private static object _sync = new object();

		ColorMatrix _backMatrix;
		ColorMatrix _frontMatrix;
		float _fade;

		public TransitionFade(TransitionInfo info) : base(info)
		{
			_frontMatrix = new ColorMatrix { Matrix33 = 1F };
			_backMatrix = new ColorMatrix { Matrix33 = 0F };
		}

		public override void Start()
		{
			lock (SyncRoot)
			{
				_backMatrix.Matrix33 = 1F;
				_frontMatrix.Matrix33 = 0F;
			}

			base.Start();
		}

		public override void Step()
		{
			lock (_sync)
			{
				_fade = Math.Min(1F, (float)CurrentStep / TransitionTime);
				_backMatrix.Matrix33 = 1F - (_frontMatrix.Matrix33 = _fade);
			}

			base.Step();
		}

		protected override void DoDraw(SharpDX.Direct2D1.DeviceContext deviceContext)
		{
			lock (SyncRoot)
			{
				//deviceContext.Clear(new SharpDX.Mathematics.Interop.RawColor4(0f, 0f, 0f, 255f));

				//var rectBack = new SharpDX.RectangleF(0f, 0f, BackImage.Size.Width, BackImage.Size.Height);
				//ResizeAndCenter(ref rectBack);
				//var rectFront = new SharpDX.RectangleF(0f, 0f, FrontImage.Size.Width, FrontImage.Size.Height);
				//ResizeAndCenter(ref rectFront);

				//if (State == TransitionState.Transitioning)
				//{
				//	var backBmp = SharpDX.Direct2D1.Bitmap.FromWicBitmap(deviceContext, BackImage);
				//	deviceContext.DrawBitmap(backBmp, rectBack.ToRawRectangleF(),
				//		_backMatrix.Matrix33, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
				//	var frontBmp = SharpDX.Direct2D1.Bitmap.FromWicBitmap(deviceContext, FrontImage);
				//	deviceContext.DrawBitmap(frontBmp, rectFront.ToRawRectangleF(),
				//		_frontMatrix.Matrix33, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
				//}
				//else if (State == TransitionState.Finished)
				//{
				//	var bmp = SharpDX.Direct2D1.Bitmap.FromWicBitmap(deviceContext, BackImage);
				//	deviceContext.DrawBitmap(bmp, rectBack.ToRawRectangleF(),
				//		_backMatrix.Matrix33, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
				//}
				//else
				//{
				//	var bmp = SharpDX.Direct2D1.Bitmap.FromWicBitmap(deviceContext, FrontImage);
				//	deviceContext.DrawBitmap(bmp, rectFront.ToRawRectangleF(),
				//		_frontMatrix.Matrix33, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
				//}
			}
		}
	}
}
