namespace ScreenSaver
{
	partial class ConfigDialog
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
			this.btnEdit = new System.Windows.Forms.Button();
			this.lblPaths = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.nudInterval = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.chkAllMons = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudInterval)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(39, 14);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(237, 35);
			this.label1.TabIndex = 0;
			this.label1.Text = "Use pictures from these folders:";
			// 
			// btnEdit
			// 
			this.btnEdit.Location = new System.Drawing.Point(579, 14);
			this.btnEdit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(120, 35);
			this.btnEdit.TabIndex = 1;
			this.btnEdit.Text = "&Edit paths...";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// lblPaths
			// 
			this.lblPaths.AutoEllipsis = true;
			this.lblPaths.Location = new System.Drawing.Point(39, 65);
			this.lblPaths.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblPaths.Name = "lblPaths";
			this.lblPaths.Size = new System.Drawing.Size(685, 154);
			this.lblPaths.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label3.Location = new System.Drawing.Point(16, 234);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(733, 3);
			this.label3.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(39, 260);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(133, 35);
			this.label4.TabIndex = 4;
			this.label4.Text = "Slide show &interval:";
			// 
			// nudInterval
			// 
			this.nudInterval.Location = new System.Drawing.Point(315, 257);
			this.nudInterval.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.nudInterval.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.nudInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudInterval.Name = "nudInterval";
			this.nudInterval.Size = new System.Drawing.Size(63, 27);
			this.nudInterval.TabIndex = 5;
			this.nudInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(399, 260);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(62, 20);
			this.label5.TabIndex = 6;
			this.label5.Text = "seconds";
			// 
			// chkAllMons
			// 
			this.chkAllMons.AutoSize = true;
			this.chkAllMons.Location = new System.Drawing.Point(315, 299);
			this.chkAllMons.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.chkAllMons.Name = "chkAllMons";
			this.chkAllMons.Size = new System.Drawing.Size(166, 24);
			this.chkAllMons.TabIndex = 7;
			this.chkAllMons.Text = "Show in all &monitors";
			this.chkAllMons.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label6.Location = new System.Drawing.Point(0, 365);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(773, 3);
			this.label6.TabIndex = 8;
			// 
			// btnSave
			// 
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSave.Location = new System.Drawing.Point(516, 397);
			this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(100, 35);
			this.btnSave.TabIndex = 9;
			this.btnSave.Text = "&Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.SaveClicked);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(624, 397);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 35);
			this.btnCancel.TabIndex = 10;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// ConfigDialog
			// 
			this.AcceptButton = this.btnSave;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(763, 451);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.chkAllMons);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.nudInterval);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblPaths);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigDialog";
			this.Text = "Sin Hing Screen Saver Settings";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.nudInterval)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Label lblPaths;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudInterval;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox chkAllMons;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
	}
}