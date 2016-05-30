using System;
using System.Drawing;

namespace ScreenSaver.ImageTransitions
{
	class TransitionDissolve
		: TransitionEffect
	{
		private Random _random = new Random();
		private int[] _randomPixels;
		private Bitmap _transition;
		private int _imageSize;
		private int _pixelsDissolved;
		private Rectangle _workingArea;

		public TransitionDissolve(TransitionInfo info)
			: base(info)
		{
			//_pixelsDissolved = 0;

			//var bSize = info.BackImage.Width * info.BackImage.Height;
			//var fSize = info.FrontImage.Width * info.FrontImage.Height;
			//_imageSize = Math.Max(bSize, fSize);
			//_randomPixels = new int[_imageSize];
			//for (var i = 0; i < _imageSize; i++)
			//	_randomPixels[i] = i * 4;
			//for (var i = 0; i < _imageSize; i++)
			//{
			//	var j = _random.Next(_imageSize);
			//	if (i != j)
			//	{
			//		_randomPixels[i] ^= _randomPixels[j];
			//		_randomPixels[j] ^= _randomPixels[i];
			//		_randomPixels[i] ^= _randomPixels[j];
			//	}
			//}

			//if (bSize > fSize)
			//{
			//	_transition = (Bitmap)BackImage.Clone();
			//	_workingArea = new Rectangle(Point.Empty, BackImage.Size);
			//}
			//else
			//{
			//	_transition = (Bitmap)FrontImage.Clone();
			//	_workingArea = new Rectangle(Point.Empty, FrontImage.Size);
			//	var g = Graphics.FromImage(_transition);
			//	g.Clear(Color.Black);
			//	Rectangle rect = new Rectangle(Point.Empty, BackImage.Size);
			//	ResizeAndCenter(ref rect, _workingArea);
			//	g.DrawImage(BackImage, rect);
			//}
		}

		public override void Start()
		{
			//lock (SyncRoot)
			//{
			//	State = TransitionState.Transitioning;
			//	TickTimer = new System.Timers.Timer(StepTime);
			//	TickTimer.Elapsed += TimerTick;
			//	TickTimer.Start();
			//}

			//base.Start();
		}

		public override void Step()
		{
			//lock (SyncRoot)
			//{
			//	var front = FrontImage as Bitmap;
			//	var currStep = CurrentStep + StepTime;
			//	int endPoint = Math.Min(_imageSize, (int)((long)_imageSize * currStep / TransitionTime));
			//	var src = front.LockBits(_workingArea, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			//	var tar = _transition.LockBits(_workingArea, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			//	for (int i = _pixelsDissolved; i < endPoint; i++)
			//		System.Runtime.InteropServices.Marshal.WriteInt32(tar.Scan0, _randomPixels[i],
			//			System.Runtime.InteropServices.Marshal.ReadInt32(src.Scan0, _randomPixels[i]));
			//	_transition.UnlockBits(tar);
			//	front.UnlockBits(src);
			//	_pixelsDissolved = endPoint;
			//}
			//base.Step();
		}

		protected override void DoDraw(SharpDX.Direct2D1.DeviceContext deviceContext)
		{
			//	lock (SyncRoot)
			//	{
			//		var g = Graphics.FromImage(Canvas);
			//		g.Clear(Color.Black);

			//		switch (State)
			//		{
			//			case TransitionState.Transitioning:
			//				var rect = new Rectangle(Point.Empty, _workingArea.Size);
			//				ResizeAndCenter(ref rect);

			//				g.DrawImage(_transition, rect);
			//				break;
			//			case TransitionState.Finished:
			//				var rectBack = new Rectangle(Point.Empty, BackImage.Size);
			//				ResizeAndCenter(ref rectBack);

			//				g.DrawImage(BackImage, rectBack);
			//				break;
			//			default:
			//				var rectFront = new Rectangle(Point.Empty, FrontImage.Size);
			//				ResizeAndCenter(ref rectFront);

			//				g.DrawImage(FrontImage, rectFront);
			//				break;
			//		}
			//	}
			//	base.Draw(e);
		}
	}
}
