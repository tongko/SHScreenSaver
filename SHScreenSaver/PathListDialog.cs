using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenSaver
{
	public partial class PathListDialog : Form
	{
		const char PathSeparator = ';';
		List<string> _folders;

		public PathListDialog(string folders)
		{
			InitializeComponent();
			btnAddFolder.Enabled = false;
			btnRemoveFolder.Enabled = false;

			_folders = new List<string>(folders.Split(new[] { PathSeparator }));

			foreach (var item in _folders)
				lstFolder.Items.Add(item);
		}

		public string Paths
		{
			get
			{
				return string.Join(PathSeparator.ToString(), _folders);
			}
		}

		private void BrowseFolderClicked(object sender, EventArgs e)
		{
			lstFolder.SelectedIndices.Clear();

			if (folderBrowser.ShowDialog(this) != DialogResult.OK)
				return;

			var folder = folderBrowser.SelectedPath;
			if (Directory.Exists(folder))
				txtFolder.Text = folder;
		}

		private void TextFolderChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtFolder.Text))
				btnAddFolder.Enabled = false;
			else
				btnAddFolder.Enabled = true;
		}

		private void AddFolderClicked(object sender, EventArgs e)
		{
			lstFolder.SelectedIndices.Clear();

			var folder = txtFolder.Text;
			if (!Directory.Exists(folder))
			{
				MessageBox.Show(this, "Invalid path specified.", "Error", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			_folders.Add(folder);
			lstFolder.Items.Add(folder);
			txtFolder.Text = string.Empty;
		}

		private void RemoveFolderClicked(object sender, EventArgs e)
		{
			var selected = lstFolder.SelectedItems;
			if (selected.Count == 0)
			{
				btnRemoveFolder.Enabled = false;
				return;
			}

			lstFolder.Items.Remove(selected[0]);
			_folders.Remove(selected[0].Text);

			lstFolder.SelectedIndices.Clear();
		}

		private void SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstFolder.SelectedIndices.Count == 0)
				btnRemoveFolder.Enabled = false;
			else
				btnRemoveFolder.Enabled = true;
		}

		private void TextFolderClicked(object sender, EventArgs e)
		{
			lstFolder.SelectedIndices.Clear();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			lstFolder.SelectedIndices.Clear();
		}
	}
}
