using ManagedCuda;
using ManagedCuda.BasicTypes;

namespace AudioDeviceKernels
{
	public class CudaContextHandling
	{
		// ----- ----- ----- Attributes ----- ----- ----- \\
		public string Repopath;

		public ListBox LogBox;
		public ComboBox DeviceBox;

		public PrimaryContext? Ctx = null;
		public CUdevice[] Devs = [];


		public CudaMemoryHandling? CMH = null;
		public CudaKernelHandling? CKH = null;


		public int LogInterval = 100;

		// ----- ----- ----- Constructors ----- ----- ----- \\
		public CudaContextHandling(string repopath, ListBox? listBox_log = null, ComboBox? comboBox_devices = null)
		{
			// Set attributes
			this.Repopath = repopath;
			this.LogBox = listBox_log ?? new ListBox();
			this.DeviceBox = comboBox_devices ?? new ComboBox();

			// Register events
			this.DeviceBox.SelectedIndexChanged += (sender, e) => this.InitContext(this.DeviceBox.SelectedIndex);

			// Fill device box
			this.Devs = this.GetDevices();
			this.FillDeviceBox(0);

		}





		// ----- ----- ----- Methods ----- ----- ----- \\
		// Log
		public void Log(string message, string inner = "", int layer = 1, bool update = false)
		{
			string msg = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] ";
			msg += "<CUDA>";

			for (int i = 0; i <= layer; i++)
			{
				msg += " - ";
			}

			msg += message;

			if (inner != "")
			{
				msg += "  (" + inner + ")";
			}

			if (update)
			{
				this.LogBox.Items[^1] = msg;
			}
			else
			{
				this.LogBox.Items.Add(msg);
				this.LogBox.SelectedIndex = this.LogBox.Items.Count - 1;
			}
		}

		public CUdevice[] GetDevices()
		{
			// Get devices
			int deviceCount = CudaContext.GetDeviceCount();
			CUdevice[] devices = new CUdevice[deviceCount];
			for (int i = 0; i < deviceCount; i++)
			{
				devices[i] = PrimaryContext.GetCUdevice(i);
			}
			return devices;
		}

		public void FillDeviceBox(int set = -1)
		{
			// Clear
			this.DeviceBox.Items.Clear();

			// Fill
			for (int i = 0; i < this.Devs.Length; i++)
			{
				CUdevice dev = this.Devs[i];
				string name = PrimaryContext.GetDeviceName(i);
				this.DeviceBox.Items.Add(name);
			}

			// Set
			if (set >= -1 && set < this.Devs.Length)
			{
				this.DeviceBox.SelectedIndex = set;
			}
		}

		public void InitContext(int deviceIndex)
		{
			// Dispose old context
			this.Dispose();

			// Verify Devs
			if (this.Devs.Length == 0)
			{
				this.Devs = this.GetDevices();
			}

			// Check index range
			if (deviceIndex < 0 || deviceIndex >= this.Devs.Length)
			{
				this.Log("Invalid device index: " + deviceIndex, "InitContext", 1);
			}

			// Create context & set current
			this.Ctx = new PrimaryContext(this.Devs[deviceIndex]);
			this.Ctx.SetCurrent();

			// Create MemH and KernelH
			this.CMH = new CudaMemoryHandling(this);
			this.CKH = new CudaKernelHandling(this);


			// Log
			this.Log("Context initialized for device: " + deviceIndex, "InitContext", 1);
		}

		public void Dispose()
		{
			bool disposed = this.Ctx == null;

			// Dispose context
			this.Ctx?.Dispose();
			this.Ctx = null;

			// Dispose MemH and KernelH
			this.CMH?.Dispose();
			this.CMH = null;
			this.CKH?.Dispose();
			this.CKH = null;

			// Log
			if (!disposed)
			{
				this.Log("Disposed context", "Dispose", 1);
			}
		}


	}
}
