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
			this.listBox_tracks = new ListBox();
			this.button_moveImage = new Button();
			this.pictureBox_track = new PictureBox();
			this.hScrollBar_track = new HScrollBar();
			this.button_playback = new Button();
			((System.ComponentModel.ISupportInitialize) this.pictureBox_main).BeginInit();
			this.panel_main.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) this.pictureBox_track).BeginInit();
			this.SuspendLayout();
			// 
			// comboBox_devices
			// 
			this.comboBox_devices.FormattingEnabled = true;
			this.comboBox_devices.Location = new Point(12, 12);
			this.comboBox_devices.Name = "comboBox_devices";
			this.comboBox_devices.Size = new Size(296, 23);
			this.comboBox_devices.TabIndex = 0;
			// 
			// listBox_log
			// 
			this.listBox_log.FormattingEnabled = true;
			this.listBox_log.ItemHeight = 15;
			this.listBox_log.Location = new Point(12, 870);
			this.listBox_log.Name = "listBox_log";
			this.listBox_log.Size = new Size(814, 94);
			this.listBox_log.TabIndex = 1;
			// 
			// listBox_images
			// 
			this.listBox_images.FormattingEnabled = true;
			this.listBox_images.ItemHeight = 15;
			this.listBox_images.Location = new Point(602, 580);
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
			this.panel_main.Location = new Point(314, 12);
			this.panel_main.Name = "panel_main";
			this.panel_main.Size = new Size(512, 512);
			this.panel_main.TabIndex = 4;
			// 
			// listBox_tracks
			// 
			this.listBox_tracks.FormattingEnabled = true;
			this.listBox_tracks.ItemHeight = 15;
			this.listBox_tracks.Location = new Point(602, 725);
			this.listBox_tracks.Name = "listBox_tracks";
			this.listBox_tracks.Size = new Size(224, 139);
			this.listBox_tracks.TabIndex = 5;
			// 
			// button_moveImage
			// 
			this.button_moveImage.Location = new Point(521, 580);
			this.button_moveImage.Name = "button_moveImage";
			this.button_moveImage.Size = new Size(75, 23);
			this.button_moveImage.TabIndex = 6;
			this.button_moveImage.Text = "Move img";
			this.button_moveImage.UseVisualStyleBackColor = true;
			this.button_moveImage.Click += this.button_moveImage_Click;
			// 
			// pictureBox_track
			// 
			this.pictureBox_track.Location = new Point(12, 725);
			this.pictureBox_track.Name = "pictureBox_track";
			this.pictureBox_track.Size = new Size(584, 119);
			this.pictureBox_track.TabIndex = 7;
			this.pictureBox_track.TabStop = false;
			// 
			// hScrollBar_track
			// 
			this.hScrollBar_track.Location = new Point(12, 847);
			this.hScrollBar_track.Name = "hScrollBar_track";
			this.hScrollBar_track.Size = new Size(584, 17);
			this.hScrollBar_track.TabIndex = 8;
			// 
			// button_playback
			// 
			this.button_playback.Location = new Point(12, 696);
			this.button_playback.Name = "button_playback";
			this.button_playback.Size = new Size(75, 23);
			this.button_playback.TabIndex = 9;
			this.button_playback.Text = "Play";
			this.button_playback.UseVisualStyleBackColor = true;
			// 
			// MainView
			// 
			this.AutoScaleDimensions = new SizeF(7F, 15F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(838, 976);
			this.Controls.Add(this.button_playback);
			this.Controls.Add(this.hScrollBar_track);
			this.Controls.Add(this.pictureBox_track);
			this.Controls.Add(this.button_moveImage);
			this.Controls.Add(this.listBox_tracks);
			this.Controls.Add(this.panel_main);
			this.Controls.Add(this.listBox_images);
			this.Controls.Add(this.listBox_log);
			this.Controls.Add(this.comboBox_devices);
			this.Name = "MainView";
			this.Text = "CUDA Device Kernels";
			((System.ComponentModel.ISupportInitialize) this.pictureBox_main).EndInit();
			this.panel_main.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) this.pictureBox_track).EndInit();
			this.ResumeLayout(false);
		}

		#endregion

		private ComboBox comboBox_devices;
		private ListBox listBox_log;
		private ListBox listBox_images;
		private PictureBox pictureBox_main;
		private Panel panel_main;
		private ListBox listBox_tracks;
		private Button button_moveImage;
		private PictureBox pictureBox_track;
		private HScrollBar hScrollBar_track;
		private Button button_playback;
	}
}
