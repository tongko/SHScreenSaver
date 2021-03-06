﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ScreenSaver
{
	static class Extensions
	{
		private static Random _rng = new Random();

		public static void Shuffle<T>(this IList<T> list)
		{
			RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do
					provider.GetBytes(box);
				while (!(box[0] < n * (byte.MaxValue / n)));

				int k = box[0] % n--;
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		public static SharpDX.RectangleF ToDxRectangleF(this System.Drawing.Rectangle rc)
		{
			return new SharpDX.RectangleF(rc.X, rc.Y, rc.Width, rc.Height);
		}

		public static SharpDX.Mathematics.Interop.RawRectangleF ToRawRectangleF(this SharpDX.RectangleF rc)
		{
			return new SharpDX.Mathematics.Interop.RawRectangleF(rc.Left, rc.Top, rc.Right, rc.Bottom);
		}

		public static string ToDebugString(this SharpDX.RectangleF rc)
		{
			return string.Format("{{l:{0}, t:{1}, r:{2}, b:{3}}}", rc.Left, rc.Top, rc.Right, rc.Bottom);
		}

	}
}
