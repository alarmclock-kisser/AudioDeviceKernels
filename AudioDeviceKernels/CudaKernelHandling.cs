using ManagedCuda;

namespace AudioDeviceKernels
{
	public class CudaKernelHandling
	{
		// ----- ----- ----- Attributes ----- ----- ----- \\
		private CudaContextHandling CCH;
		private PrimaryContext? Ctx => this.CCH.Ctx;

		private ListBox LogBox;




		// ----- ----- ----- Constructors ----- ----- ----- \\
		public CudaKernelHandling(CudaContextHandling cudaContextHandling)
		{
			// Set attributes
			this.CCH = cudaContextHandling;
			this.LogBox = this.CCH.LogBox;
		}





		// ----- ----- ----- Methods ----- ----- ----- \\
		// Log
		public void Log(string message, string inner = "", int layer = 1, bool update = false)
		{
			string msg = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] ";
			msg += "<Kernel>";

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

		public void Dispose()
		{
			// Dispose
		}



	}
}