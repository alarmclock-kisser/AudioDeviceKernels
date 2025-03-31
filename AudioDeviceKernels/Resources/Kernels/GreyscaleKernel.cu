extern "C" __global__ void GrayscaleKernel(unsigned char* data, int length, int intensity) {
    int idx = blockIdx.x * blockDim.x + threadIdx.x;
    int pixelIndex = idx * 3;  // RGB-Pixel haben 3 Byte

    if (pixelIndex + 2 < length) {
        // RGB-Werte auslesen
        unsigned char r = data[pixelIndex];
        unsigned char g = data[pixelIndex + 1];
        unsigned char b = data[pixelIndex + 2];

        // Grauwert berechnen (Helligkeit mit Gewichtung)
        unsigned char gray = (r * 0.299f + g * 0.587f + b * 0.114f) * (intensity / 255.0f);

        // RGB durch Grauwert ersetzen
        data[pixelIndex] = gray;
        data[pixelIndex + 1] = gray;
        data[pixelIndex + 2] = gray;
    }
}
