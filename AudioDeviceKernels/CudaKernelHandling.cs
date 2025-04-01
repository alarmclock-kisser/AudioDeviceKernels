using ManagedCuda;
using ManagedCuda.NVRTC;
using System.Diagnostics;

namespace AudioDeviceKernels
{
	public class CudaKernelHandling
	{
		// ----- ----- ----- Attributes ----- ----- ----- \\
		private CudaContextHandling CCH;
		private string Repopath => this.CCH.Repopath;
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

		public void VerifyFolderStructure(bool silent = false)
		{
			// Check for Dir path: Resources\Kernels\PTX & Resources\Kernels\CU , if not fully available, create them
			string ptxPath = Path.Combine(Repopath, "Resources\\Kernels\\PTX");
			string cuPath = Path.Combine(Repopath, "Resources\\Kernels\\CU");

			if (!Directory.Exists(ptxPath))
			{
				Directory.CreateDirectory(ptxPath);
			}
			if (!Directory.Exists(cuPath))
			{
				Directory.CreateDirectory(cuPath);
			}

			if (!silent)
			{
				Log("Folder structure verified", "", 1);
			}
		}



		// Compile
		public string CompileString(string kernelString)
		{
			if (Ctx == null)
			{
				Log("No CUDA context available", "", 1);
				return "";
			}

			string kernelName = kernelString.Split("void ")[1].Split("(")[0];
			kernelName = kernelName.Replace("Kernel", "");

			string logpath = Path.Combine(Repopath, "Resources\\Logs", kernelName + "Kernel" + ".log");

			Stopwatch sw = Stopwatch.StartNew();
			Log("Compiling kernel " + kernelName);

			// Load kernel file
			string kernelCode = kernelString;

			// Save also the kernel string as .c file
			string cPath = Path.Combine(Repopath, "Resources\\Kernels\\CU", kernelName + "Kernel" + ".c");
			File.WriteAllText(cPath, kernelCode);


			var rtc = new CudaRuntimeCompiler(kernelCode, kernelName);

			try
			{
				// Compile kernel
				rtc.Compile([]);

				if (rtc.GetLogAsString().Length > 0)
				{
					Log("Kernel compiled with warnings", "", 1);
					File.WriteAllText(logpath, rtc.GetLogAsString());
				}


				sw.Stop();
				long deltaMicros = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
				Log("Kernel compiled in " + deltaMicros.ToString("N0") + " µs", "Repo\\" + Path.GetRelativePath(Repopath, logpath), 2, true);

				// Get ptx code
				byte[] ptxCode = rtc.GetPTX();

				// Export ptx
				string ptxPath = Path.Combine(Repopath, "Resources\\Kernels\\PTX", kernelName + "Kernel" + ".ptx");
				File.WriteAllBytes(ptxPath, ptxCode);

				Log("PTX code exported to " + ptxPath, "", 1);

				return ptxPath;
			}
			catch (Exception ex)
			{
				File.WriteAllText(logpath, rtc.GetLogAsString());
				Log(ex.Message, ex.InnerException?.Message ?? "", 1);

				return "";
			}
		}


	}
}