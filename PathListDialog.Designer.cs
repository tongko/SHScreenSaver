namespace ScreenSaver
{
	partial class PathListDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.txtFolder = new System.Windows.Forms.TextBox();
			this.btnBrowseFolder = new System.Windows.Forms.Button();
			this.btnAddFolder = new System.Windows.Forms.Button();
			this.lstFolder = new System.Windows.Forms.ListView();
			this.btnRemoveFolder = new System.Windows.Forms.Button();
			this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 21);
			this.label1.TabIndex = 0;
			this.label1.Text = "Folder";
			// 
			// txtFolder
			// 
			this.txtFolder.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtFolder.Location = new System.Drawing.Point(14, 38);
			this.txtFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtFolder.Name = "txtFolder";
			this.txtFolder.Size = new System.Drawing.Size(487, 30);
			this.txtFolder.TabIndex = 1;
			this.txtFolder.Click += new System.EventHandler(this.TextFolderClicked);
			this.txtFolder.TextChanged += new System.EventHandler(this.TextFolderChanged);
			// 
			// btnBrowseFolder
			// 
			this.btnBrowseFolder.Location = new System.Drawing.Point(508, 37);
			this.btnBrowseFolder.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
			this.btnBrowseFolder.Name = "btnBrowseFolder";
			this.btnBrowseFolder.Size = new System.Drawing.Size(34, 33);
			this.btnBrowseFolder.TabIndex = 2;
			this.btnBrowseFolder.Text = "...";
			this.btnBrowseFolder.UseVisualStyleBackColor = true;
			this.btnBrowseFolder.Click += new System.EventHandler(this.BrowseFolderClicked);
			// 
			// btnAddFolder
			// 
			this.btnAddFolder.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAddFolder.Location = new System.Drawing.Point(14, 75);
			this.btnAddFolder.Name = "btnAddFolder";
			this.btnAddFolder.Size = new System.Drawing.Size(112, 32);
			this.btnAddFolder.TabIndex = 3;
			this.btnAddFolder.Text = "Add Folder";
			this.btnAddFolder.UseVisualStyleBackColor = true;
			this.btnAddFolder.Click += new System.EventHandler(this.AddFolderClicked);
			// 
			// lstFolder
			// 
			this.lstFolder.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.lstFolder.FullRowSelect = true;
			this.lstFolder.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstFolder.Location = new System.Drawing.Point(14, 113);
			this.lstFolder.MultiSelect = false;
			this.lstFolder.Name = "lstFolder";
			this.lstFolder.Size = new System.Drawing.Size(525, 532);
			this.lstFolder.TabIndex = 4;
			this.lstFolder.UseCompatibleStateImageBehavior = false;
			this.lstFolder.View = System.Windows.Forms.View.Details;
			this.lstFolder.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
			// 
			// btnRemoveFolder
			// 
			this.btnRemoveFolder.Location = new System.Drawing.Point(133, 75);
			this.btnRemoveFolder.Name = "btnRemoveFolder";
			this.btnRemoveFolder.Size = new System.Drawing.Size(115, 32);
			this.btnRemoveFolder.TabIndex = 5;
			this.btnRemoveFolder.Text = "Remove";
			this.btnRemoveFolder.UseVisualStyleBackColor = true;
			this.btnRemoveFolder.Click += new System.EventHandler(this.RemoveFolderClicked);
			// 
			// folderBrowser
			// 
			this.folderBrowser.Description = "Add folder containing image files.";
			this.folderBrowser.ShowNewFolderButton = false;
			// 
			// btnOK
			// 
			this.btnOK.AutoSize = true;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(312, 662);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 33);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.AutoSize = true;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(427, 662);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 33);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "";
			this.columnHeader1.Width = 500;
			// 
			// PathListDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(551, 712);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnRemoveFolder);
			this.Controls.Add(this.lstFolder);
			this.Controls.Add(this.btnAddFolder);
			this.Controls.Add(this.btnBrowseFolder);
			this.Controls.Add(this.txtFolder);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PathListDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Folder List";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtFolder;
		private System.Windows.Forms.Button btnBrowseFolder;
		private System.Windows.Forms.Button btnAddFolder;
		private System.Windows.Forms.ListView lstFolder;
		private System.Windows.Forms.Button btnRemoveFolder;
		private System.Windows.Forms.FolderBrowserDialog folderBrowser;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ColumnHeader columnHeader1;
	}
}