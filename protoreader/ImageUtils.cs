using System.Drawing;
using System.Diagnostics;

namespace protoReader
{
    public class ImageUtils
    {
        public static Bitmap CreateBitmapFromBytes(int width, int height, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive values");

            int expectedLength = width * height * 2;
            if (data.Length < expectedLength)
                throw new ArgumentException($"Data array too small. Expected at least {expectedLength} bytes, got {data.Length}");

            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * 2;

                    // Read 2 bytes per pixel (assuming 16-bit grayscale or RGB565)
                    ushort pixelValue = (ushort)(data[pixelIndex] | (data[pixelIndex + 1] << 8));

                    // Convert 16-bit value to RGB (assuming RGB565 format)
                    // RGB565: RRRRR GGGGGG BBBBB
                    int red = (pixelValue >> 11) & 0x1F;
                    int green = (pixelValue >> 5) & 0x3F;
                    int blue = pixelValue & 0x1F;

                    // Scale to 8-bit values
                    red = (red * 255) / 31;
                    green = (green * 255) / 63;
                    blue = (blue * 255) / 31;

                    Color color = Color.FromArgb(red, green, blue);
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public static void MakeVid(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                throw new ArgumentException("Folder path cannot be null or empty", nameof(folderPath));

            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

            // Get all BMP files sorted alphabetically
            string[] bmpFiles = Directory.GetFiles(folderPath, "*.bmp")
                                        .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                                        .ToArray();

            if (bmpFiles.Length == 0)
                throw new InvalidOperationException("No BMP files found in the specified folder");

            string outputPath = Path.Combine(folderPath, "output.mpg");
            string tempListFile = Path.Combine(folderPath, "filelist.txt");

            try
            {
                // Create a file list for FFmpeg
                using (StreamWriter writer = new StreamWriter(tempListFile))
                {
                    foreach (string bmpFile in bmpFiles)
                    {
                        writer.WriteLine($"file '{Path.GetFileName(bmpFile)}'");
                    }
                }

                // Use FFmpeg to create video from images
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-f concat -safe 0 -i \"{tempListFile}\" -r 30 -c:v mpeg1video -q:v 2 \"{outputPath}\"",
                    WorkingDirectory = folderPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                        throw new InvalidOperationException("Failed to start FFmpeg process");

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new InvalidOperationException($"FFmpeg failed with exit code {process.ExitCode}. Error: {error}");
                    }
                }

                Console.WriteLine($"Video created successfully: {outputPath}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create video: {ex.Message}", ex);
            }
            finally
            {
                // Clean up temporary file
                if (File.Exists(tempListFile))
                {
                    try
                    {
                        File.Delete(tempListFile);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }
    }
}