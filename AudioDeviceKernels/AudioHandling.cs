using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace AudioDeviceKernels
{
	public class AudioHandling
	{
		// ----- ATTRIBUTES ----- \\
		private ListBox TracksList;
		private PictureBox WaveBox;
		private Button PlaybackButton;
		private HScrollBar OffsetScrollbar;

		public List<AudioObject> Tracks = [];
		public int SamplesPerPixel { get; private set; } = 1024;
		public long Offset { get; private set; } = 0;

		private bool IsPlaying = false;

		// ----- LAMBDA ----- \\
		public AudioObject? CurrentTrack => TracksList.SelectedIndex >= 0 && TracksList.SelectedIndex < Tracks.Count ? Tracks[TracksList.SelectedIndex] : null;
		public Image? CurrentWave => DrawWaveform();

		public AudioObject? this[int index] => index >= 0 && index < Tracks.Count ? Tracks[index] : null;
		public AudioObject? this[string name] => Tracks.FirstOrDefault(t => t.Name == name);


		// ----- CONSTRUCTOR ----- \\
		public AudioHandling(ListBox? tracksList = null, PictureBox? view_pBox = null, Button? playback_button = null, HScrollBar? offset_scrollbar = null)
		{
			// Set attributes
			this.TracksList = tracksList ?? new ListBox();
			this.WaveBox = view_pBox ?? new PictureBox();
			this.PlaybackButton = playback_button ?? new Button();
			this.OffsetScrollbar = offset_scrollbar ?? new HScrollBar();

			// Register events
			WaveBox.MouseWheel += OnMouseWheel;
			TracksList.SelectedIndexChanged += (s, e) => ChangeTrack();
			PlaybackButton.Click += (s, e) => TogglePlayback();
			OffsetScrollbar.Scroll += (s, e) => SetOffset(OffsetScrollbar.Value);
			TracksList.MouseDown += (s, e) => RemoveTrack(TracksList.SelectedIndex);
		}


		// ----- METHODS ----- \\
		private void OnMouseWheel(object? sender, MouseEventArgs e)
		{
			if (CurrentTrack == null) return;

			if (Control.ModifierKeys == Keys.Control)
			{
				// Scroll Offset
				SetOffset(Offset - e.Delta * SamplesPerPixel / 8);
			}
			else
			{
				// Zoom mit Faktor 1.5 pro Stufe
				float zoomFactor = e.Delta < 0 ? 1.5f : 1 / 1.5f;
				SetZoom((int) (SamplesPerPixel * zoomFactor));
			}
		}

		private void SetZoom(int newZoom)
		{
			if (CurrentTrack == null) return;

			SamplesPerPixel = Math.Clamp(newZoom, 1, 16384);
			UpdateScrollbar();
			WaveBox.Image = CurrentWave;
		}

		private void SetOffset(long newOffset)
		{
			if (CurrentTrack == null) return;

			long maxOffset = Math.Max(0, CurrentTrack.Length - (WaveBox.Width * SamplesPerPixel));
			Offset = Math.Clamp(newOffset, 0, maxOffset);

			OffsetScrollbar.Value = (int) Offset;
			WaveBox.Image = CurrentWave;
		}

		private void ChangeTrack()
		{
			StopPlayback();
			Offset = 0;
			UpdateScrollbar();
			WaveBox.Image = CurrentWave;
		}

		private void UpdateScrollbar()
		{
			if (CurrentTrack == null) return;

			long maxOffset = Math.Max(0, CurrentTrack.Length - (WaveBox.Width * SamplesPerPixel));

			OffsetScrollbar.Minimum = 0;
			OffsetScrollbar.Maximum = (int) maxOffset;
			OffsetScrollbar.Value = (int) Offset;
			OffsetScrollbar.LargeChange = SamplesPerPixel;
		}

		private void TogglePlayback()
		{
			if (IsPlaying)
			{
				StopPlayback();
			}
			else
			{
				StartPlayback();
			}
		}

		private void StartPlayback()
		{
			if (CurrentTrack == null) return;

			CurrentTrack.PlayStop();
			IsPlaying = true;
			PlaybackButton.Text = "Stop";
		}

		private void StopPlayback()
		{
			if (CurrentTrack == null) return;

			CurrentTrack.Stop();
			IsPlaying = false;
			PlaybackButton.Text = "Play";
		}

		public void AddTrack(string path)
		{
			AudioObject track = new(path);
			Tracks.Add(track);
			FillTracksList();
		}

		public void RemoveTrack(int index)
		{
			// Only if NOT LEFT MOUSE BUTTON AND RIGHT MOUSE BUTTON
			if (!(Control.MouseButtons == MouseButtons.Right && Control.MouseButtons != MouseButtons.Left))
			{
				return;
			}

			if (index < 0 || index >= Tracks.Count)
			{
				return;
			}

			Tracks[index].Dispose();
			Tracks.RemoveAt(index);
			FillTracksList();
		}

		public void FillTracksList()
		{
			TracksList.Items.Clear();

			foreach (AudioObject track in Tracks)
			{
				string entry = (track.OnDevice ? "o" : "*") + " ";
				entry += track.Name.Length > 35 ? track.Name.Substring(0, 32) + "..." : track.Name;
				TracksList.Items.Add(entry);
			}
		}

		public Image? DrawWaveform()
		{
			// Get samplesperpixel
			int samplesPerPixel = CurrentTrack?.GetFitResolution(WaveBox.Width) ?? 1;
			return CurrentTrack?.GetWaveform(WaveBox, Offset, samplesPerPixel, Color.BlueViolet, Color.White);
		}
	}






	public class AudioObject
	{
		// ----- ATTRIBUTES ----- \\
		public string Name = "";
		public string Pth = "";

		public float[] Data = [];
		public long Pointer = 0;
		public char Form = 'f';
		public int Overlap = 0;

		public int Samplerate = 44100;
		public int Bitdepth = 24;
		public int Channels = 2;


		public WaveOutEvent Player = new();

		public Timer TimerPlayback = new();

		// ----- LAMBDA ----- \\
		public long Length => Data.LongLength;
		public double Duration => (double) Length / Samplerate / Channels / Bitdepth / 8;
		public string Meta => GetMeta();


		public bool Playing => Player.PlaybackState == PlaybackState.Playing;
		public long Position => Player.GetPosition();
		public double PositionSeconds => (double) Position / Samplerate / Channels / Bitdepth / 8;


		public bool OnHost => Data.LongLength > 0 && Pointer == 0;
		public bool OnDevice => Data.LongLength == 0 && Pointer != 0;


		// ----- CONSTRUCTOR ----- \\
		public AudioObject(string pth)
		{
			// Try load audio using naudio
			try
			{
				// Set path & name
				this.Pth = pth;
				this.Name = Path.GetFileName(pth);

				// Load audio
				using (AudioFileReader reader = new(pth))
				{
					// Set attributes
					this.Samplerate = reader.WaveFormat.SampleRate;
					this.Bitdepth = reader.WaveFormat.BitsPerSample;
					this.Channels = reader.WaveFormat.Channels;

					// Read data
					this.Data = new float[reader.Length];
					reader.Read(this.Data, 0, this.Data.Length);
				}
			}
			catch (Exception ex)
			{
				return;
			}
		}



		// ----- METHODS ----- \\
		// Dispose
		public void Dispose()
		{
			// Dispose data & reset pointer
			this.Data = [];
			this.Pointer = 0;
		}


		// Get Meta
		public string GetMeta()
		{
			StringBuilder sb = new();
			sb.Append(Samplerate / 1000 + " kHz, ");
			sb.Append(Bitdepth + " b, ");
			sb.Append(Channels + " ch, ");
			sb.Append((Data.Length / Samplerate / Channels / Bitdepth / 8) + " s, ");
			sb.Append(Data.LongLength + " samples");
			return sb.ToString();
		}


		// I/O
		public byte[] GetBytes()
		{
			int bytesPerSample = Bitdepth / 8;
			byte[] bytes = new byte[Data.Length * bytesPerSample];

			for (int i = 0; i < Data.Length; i++)
			{
				byte[] byteArray;
				float sample = Data[i];

				switch (Bitdepth)
				{
					case 16:
						short shortSample = (short) (sample * short.MaxValue);
						byteArray = BitConverter.GetBytes(shortSample);
						break;
					case 24:
						int intSample24 = (int) (sample * (1 << 23));
						byteArray = new byte[3];
						byteArray[0] = (byte) (intSample24 & 0xFF);
						byteArray[1] = (byte) ((intSample24 >> 8) & 0xFF);
						byteArray[2] = (byte) ((intSample24 >> 16) & 0xFF);
						break;
					case 32:
						int intSample32 = (int) (sample * int.MaxValue);
						byteArray = BitConverter.GetBytes(intSample32);
						break;
					default:
						throw new ArgumentException("Unsupported bit depth");
				}

				Buffer.BlockCopy(byteArray, 0, bytes, i * bytesPerSample, bytesPerSample);
			}

			return bytes;
		}

		public void ExportAudioWav(string filepath)
		{
			int sampleRate = Samplerate;
			int bitDepth = Bitdepth;
			int channels = Channels;
			float[] audioData = Data;

			// Berechne die tatsächliche Länge der Audiodaten
			int actualLength = audioData.Length / (bitDepth / 8) / channels;

			using (var fileStream = new FileStream(filepath, FileMode.Create))
			using (var writer = new BinaryWriter(fileStream))
			{
				// RIFF header
				writer.Write(Encoding.ASCII.GetBytes("RIFF"));
				writer.Write(36 + actualLength * channels * (bitDepth / 8)); // File size
				writer.Write(Encoding.ASCII.GetBytes("WAVE"));

				// fmt subchunk
				writer.Write(Encoding.ASCII.GetBytes("fmt "));
				writer.Write(16); // Subchunk1Size (16 for PCM)
				writer.Write((short) 1); // AudioFormat (1 for PCM)
				writer.Write((short) channels); // NumChannels
				writer.Write(sampleRate); // SampleRate
				writer.Write(sampleRate * channels * (bitDepth / 8)); // ByteRate
				writer.Write((short) (channels * (bitDepth / 8))); // BlockAlign
				writer.Write((short) bitDepth); // BitsPerSample

				// data subchunk
				writer.Write(Encoding.ASCII.GetBytes("data"));
				writer.Write(actualLength * channels * (bitDepth / 8)); // Subchunk2Size

				// Convert float array to the appropriate bit depth and write to file
				for (int i = 0; i < actualLength * channels; i++)
				{
					float sample = audioData[i];
					switch (bitDepth)
					{
						case 16:
							var shortSample = (short) (sample * short.MaxValue);
							writer.Write(shortSample);
							break;
						case 24:
							var intSample24 = (int) (sample * (1 << 23));
							writer.Write((byte) (intSample24 & 0xFF));
							writer.Write((byte) ((intSample24 >> 8) & 0xFF));
							writer.Write((byte) ((intSample24 >> 16) & 0xFF));
							break;
						case 32:
							var intSample32 = (int) (sample * int.MaxValue);
							writer.Write(intSample32);
							break;
						default:
							throw new ArgumentException("Unsupported bit depth");
					}
				}
			}
		}


		// Playback
		public void PlayStop(Button? playbackButton = null)
		{
			if (Player.PlaybackState == PlaybackState.Playing)
			{
				if (playbackButton != null)
				{
					playbackButton.Text = "⏵";
				}
				TimerPlayback.Stop();
				Player.Stop();
			}
			else
			{
				byte[] bytes = GetBytes();

				MemoryStream ms = new(bytes);
				RawSourceWaveStream raw = new(ms, new WaveFormat(Samplerate, Bitdepth, Channels));

				Player.Init(raw);

				if (playbackButton != null)
				{
					playbackButton.Text = "⏹";
				}
				TimerPlayback.Start();
				Player.Play();

				while (Player.PlaybackState == PlaybackState.Playing)
				{
					Application.DoEvents();
					Thread.Sleep(100);
				}

				if (playbackButton != null)
				{
					playbackButton.Text = "⏵";
				}
			}
		}

		public void Stop(Button? playbackButton = null)
		{
			if (Player.PlaybackState == PlaybackState.Playing)
			{
				if (playbackButton != null)
				{
					playbackButton.Text = "⏵";
				}
				TimerPlayback.Stop();
				Player.Stop();
			}
		}


		// Normalize
		public void Normalize(float target = 1)
		{
			// Abort if no data or playing
			if (Data.Length == 0 || Playing)
			{
				return;
			}

			// Get max value
			float max = Data.Max(Math.Abs);

			// Normalize (Parallel)
			Parallel.For(0, Data.Length, i =>
			{
				Data[i] = Data[i] / max * target;
			});
		}


		// Waveform
		public Bitmap GetWaveform(PictureBox waveBox, long offset = 0, int samplesPerPixel = 1, Color? graphColor = null, Color? bgColor = null)
		{
			// Determine offset
			offset = Math.Clamp(offset, 0, Data.LongLength);

			// Validate inputs
			if (Data.Length == 0 || waveBox.Width <= 0 || waveBox.Height <= 0)
			{
				return new Bitmap(1, 1);
			}
			// Set colors
			Color waveColor = graphColor ?? Color.Blue;
			Color bg = bgColor ?? (waveColor.GetBrightness() > 0.5f ? Color.White : Color.Black);

			// Create bitmap & graphics
			Bitmap bmp = new(waveBox.Width, waveBox.Height);
			using Graphics g = Graphics.FromImage(bmp);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.Clear(bg);
			using Pen pen = new(waveColor);

			// Y-axis settings
			float centerY = waveBox.Height / 2f;
			float scale = centerY;

			// Draw waveform
			for (int x = 0; x < waveBox.Width; x++)
			{
				long sampleIdx = offset + x * samplesPerPixel;
				if (sampleIdx >= Data.Length) break;

				float min = float.MaxValue, max = float.MinValue;

				for (int i = 0; i < samplesPerPixel && sampleIdx + i < Data.Length; i++)
				{
					float sample = Data[sampleIdx + i];
					if (sample > max) max = sample;
					if (sample < min) min = sample;
				}

				float yMax = centerY - max * scale;
				float yMin = centerY - min * scale;

				g.DrawLine(pen, x, Math.Clamp(yMax, 0, waveBox.Height), x, Math.Clamp(yMin, 0, waveBox.Height));
			}

			return bmp;
		}

		public int GetFitResolution(int width)
		{
			// Use the number of channels to ensure correct scaling
			int totalSamples = Data.Length / Channels / (Bitdepth / 8);
			return Math.Max(1, (int) Math.Ceiling((double) totalSamples / width));
		}


		// Chunking
		public List<float[]> MakeChunks(int chunkSize, int overlap = 0)
		{
			// If overlap is 0 take half of chunk size
			overlap = overlap == 0 ? chunkSize / 2 : overlap;

			// Verify overlap
			Overlap = Math.Max(0, Math.Min(overlap, chunkSize / 2));

			// Get chunk count & make chunks List
			int stepSize = chunkSize - Overlap;
			int chunkCount = (int) Math.Ceiling((double) Data.Length / stepSize);

			List<float[]> chunks = new(chunkCount);
			int index = 0;

			while (index < Data.Length)
			{
				// Set default chunk size
				int length = Math.Min(chunkSize, Data.Length - index);
				float[] chunk = new float[chunkSize]; // Immer volle Größe

				// Copy data to chunk
				Array.Copy(Data, index, chunk, 0, length);
				chunks.Add(chunk);

				// Increase index
				index += stepSize;
			}

			return chunks;
		}

		public void AggregateChunks(List<float[]> chunks)
		{
			if (chunks == null || chunks.Count == 0)
			{
				Data = [];
				return;
			}

			// Get step size & total length
			int stepSize = chunks[0].Length - Overlap;
			int totalLength = stepSize * (chunks.Count - 1) + chunks[^1].Length;
			float[] aggregated = new float[totalLength];

			int index = 0;
			foreach (float[] chunk in chunks)
			{
				// Get copy length (min of chunk length or remaining space)
				int copyLength = Math.Min(chunk.Length, aggregated.Length - index);

				Array.Copy(chunk, 0, aggregated, index, copyLength);
				index += stepSize;
			}

			// Set Floats & Length
			Data = aggregated;
		}


		// Debug
		public string GetFirstSamples(int count = 0, bool skipSilence = false)
		{
			// Set count if not set
			count = count == 0 ? Samplerate : count;

			// Abort if no data
			if (Data.Length == 0)
			{
				return "";
			}

			// Skip silence
			int start = 0;
			var data = Data;
			if (skipSilence)
			{
				while (start < Data.Length && Math.Abs(Data[start]) < 0.01f)
				{
					start++;
				}
				count = Math.Min(count, Data.Length - start);
				data = Data.Skip(start).Take(count).ToArray();
			}

			// Get samples
			StringBuilder sb = new();
			sb.Append("[");
			for (int i = 0; i < count; i++)
			{
				sb.Append(data[i] + ", ");
			}
			sb.Remove(sb.Length - 2, 2);
			sb.Append("]");
			return sb.ToString();
		}


	}
}
