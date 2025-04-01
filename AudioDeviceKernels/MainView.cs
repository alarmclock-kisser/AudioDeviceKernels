namespace AudioDeviceKernels
{
	public partial class MainView : Form
	{
		// ----- ----- ----- Attributes ----- ----- ----- \\
		public string Repopath;

		public ImageHandling IH;
		public AudioHandling AH;
		public CudaContextHandling CCH;



		// ----- ----- ----- Constructors ----- ----- ----- \\
		public MainView()
		{
			this.InitializeComponent();

			// Set repopath
			this.Repopath = this.GetRepopath(true);

			// Window position
			this.StartPosition = FormStartPosition.Manual;
			this.Location = new Point(0, 0);

			// Init objects
			this.IH = new ImageHandling(this.Repopath, this.listBox_images);
			this.AH = new AudioHandling(listBox_tracks, pictureBox_track, button_playback, hScrollBar_track);
			this.CCH = new CudaContextHandling(this.Repopath, this.listBox_log, this.comboBox_devices);

			// Register events
			this.listBox_images.SelectedIndexChanged += (s, e) => this.ToggleUI();

			// Load resources
			this.LoadResources();
		}






		// ----- ----- ----- Methods ----- ----- ----- \\
		public string GetRepopath(bool root = false)
		{
			string repo = AppDomain.CurrentDomain.BaseDirectory;

			if (root)
			{
				repo += @"..\..\..\";
			}

			repo = Path.GetFullPath(repo);

			return repo;
		}

		public void LoadResources()
		{
			// Load images
			string[] files = Directory.GetFiles(this.Repopath + @"Resources\Images\");
			foreach (string file in files)
			{
				this.IH.AddImage(file);
			}
		}

		public void ToggleUI()
		{
			// PictureBox
			this.pictureBox_main.Image = this.IH.CurrentImage?.Img;
		}

		public void MoveImage()
		{
			// Check image & memh
			if (this.IH.CurrentImage == null || this.CCH.CMH == null)
			{
				return;
			}

			// Move image device <-> host
			if (this.IH.CurrentImage.OnHost)
			{
				// Move to device
				this.IH.CurrentImage.Ptr = this.CCH.CMH.PushData(IH.CurrentImage.GetPixelRowsAsBytes());
				this.IH.CurrentImage.Img = null;
			}
			else if (this.IH.CurrentImage.OnDevice)
			{
				// Move to host
				this.IH.CurrentImage.SetImageFromChunks(this.CCH.CMH.PullData<byte>(this.IH.CurrentImage.Ptr));
				this.IH.CurrentImage.Ptr = 0;
			}

			ToggleUI();
		}



		// ----- ----- ----- Events ----- ----- ----- \\
		private void button_moveImage_Click(object sender, EventArgs e)
		{
			// Move image
			this.MoveImage();
		}





	}
}
