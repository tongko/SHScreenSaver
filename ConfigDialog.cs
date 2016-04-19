using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenSaver
{
	public partial class ConfigDialog : Form
	{
		const char PathSeparator = ';';

		public ConfigDialog()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			lblPaths.Text = string.Join(PathSeparator.ToString(), Settings.Instance.ImagePaths);
			nudInterval.Value = Settings.Instance.Interval;
			chkAllMons.Checked = Settings.Instance.AllMonitors;
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			var dialog = new PathListDialog(lblPaths.Text);
			if (dialog.ShowDialog(this) != DialogResult.OK)
				return;

			lblPaths.Text = dialog.Paths;
		}

		private void SaveClicked(object sender, EventArgs e)
		{
			var settings = Settings.Instance;
			settings.ReplacePathRange(lblPaths.Text.Split(new[] { PathSeparator }));
			settings.SetInterval((int)nudInterval.Value);
			settings.SetUseAllMonitors(chkAllMons.Checked);

			settings.SaveSettings();
		}
	}
}
