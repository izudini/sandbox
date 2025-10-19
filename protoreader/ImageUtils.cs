using System.Drawing;

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
    }
}