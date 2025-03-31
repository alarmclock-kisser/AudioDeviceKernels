namespace AudioDeviceKernels
{
    partial class MainView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.comboBox_devices = new ComboBox();
			this.listBox_log = new ListBox();
			this.listBox_images = new ListBox();
			this.pictureBox_main = new PictureBox();
			this.panel_main = new Panel();
			((System.ComponentModel.ISupportInitialize) this.pictureBox_main).BeginInit();
			this.panel_main.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBox_devices
			// 
			this.comboBox_devices.FormattingEnabled = true;
			this.comboBox_devices.Location = new Point(12, 12);
			this.comboBox_devices.Name = "comboBox_devices";
			this.comboBox_devices.Size = new Size(224, 23);
			this.comboBox_devices.TabIndex = 0;
			// 
			// listBox_log
			// 
			this.listBox_log.FormattingEnabled = true;
			this.listBox_log.ItemHeight = 15;
			this.listBox_log.Location = new Point(12, 344);
			this.listBox_log.Name = "listBox_log";
			this.listBox_log.Size = new Size(776, 94);
			this.listBox_log.TabIndex = 1;
			// 
			// listBox_images
			// 
			this.listBox_images.FormattingEnabled = true;
			this.listBox_images.ItemHeight = 15;
			this.listBox_images.Location = new Point(12, 199);
			this.listBox_images.Name = "listBox_images";
			this.listBox_images.Size = new Size(224, 139);
			this.listBox_images.TabIndex = 2;
			// 
			// pictureBox_main
			// 
			this.pictureBox_main.BackColor = Color.White;
			this.pictureBox_main.Location = new Point(0, 0);
			this.pictureBox_main.Name = "pictureBox_main";
			this.pictureBox_main.Size = new Size(256, 256);
			this.pictureBox_main.TabIndex = 3;
			this.pictureBox_main.TabStop = false;
			// 
			// panel_main
			// 
			this.panel_main.Controls.Add(this.pictureBox_main);
			this.panel_main.Location = new Point(532, 12);
			this.panel_main.Name = "panel_main";
			this.panel_main.Size = new Size(256, 256);
			this.panel_main.TabIndex = 4;
			// 
			// MainView
			// 
			this.AutoScaleDimensions = new SizeF(7F, 15F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(800, 450);
			this.Controls.Add(this.panel_main);
			this.Controls.Add(this.listBox_images);
			this.Controls.Add(this.listBox_log);
			this.Controls.Add(this.comboBox_devices);
			this.Name = "MainView";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize) this.pictureBox_main).EndInit();
			this.panel_main.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		#endregion

		private ComboBox comboBox_devices;
		private ListBox listBox_log;
		private ListBox listBox_images;
		private PictureBox pictureBox_main;
		private Panel panel_main;
	}
}
