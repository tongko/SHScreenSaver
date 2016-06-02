namespace SinHing.DirectXLibs
{
	public class DxAppConfiguration
	{
		public DxAppConfiguration(string title = "DirectX Application",
			int width = 800, int height = 600)
		{
			Title = title;
			Width = width;
			Height = height;
			WaitVerticalBlanking = false;
		}

		public string Title { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public bool WaitVerticalBlanking { get; set; }
	}
}
