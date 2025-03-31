namespace AudioDeviceKernels
{
	public partial class MainView : Form
	{
		// ----- ----- ----- Attributes ----- ----- ----- \\
		public string Repopath;

		public ImageHandling IH;
		public CudaContextHandling CCH;



		// ----- ----- ----- Constructors ----- ----- ----- \\
		public MainView()
		{
			this.InitializeComponent();

			this.Repopath = this.GetRepopath(true);

			// Init objects
			this.IH = new ImageHandling(this.Repopath, this.listBox_images);
			this.CCH = new CudaContextHandling(this.Repopath, this.listBox_log, this.comboBox_devices);
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

	





		// ----- ----- ----- Events ----- ----- ----- \\






	}
}
