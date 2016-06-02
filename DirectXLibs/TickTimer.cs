using System.Diagnostics;

namespace SinHing.DirectXLibs
{
	public class TickTimer
	{
		private Stopwatch _stopWatch;
		private double _lastUpdate;

		public TickTimer()
		{
			_stopWatch = new Stopwatch();
		}

		public double ElapseTime
		{
			get { return _stopWatch.ElapsedMilliseconds * 0.001; }
		}

		public void Start()
		{
			_stopWatch.Start();
			_lastUpdate = 0;
		}

		public void Stop()
		{
			_stopWatch.Stop();
		}

		public double Update()
		{
			var now = ElapseTime;
			var updateTime = now - _lastUpdate;
			_lastUpdate = now;

			return updateTime;
		}
	}
}
