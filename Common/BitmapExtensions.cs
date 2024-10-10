using System.Numerics;
using System.Drawing;
using System.Runtime.Versioning;

namespace Common;

[SupportedOSPlatform("windows")]
public static class BitmapExtensions
{
    public static byte GetIMax(this Bitmap source)
    {
        var iMax = byte.MinValue;
        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var sourcePixel = source.GetPixel(x, y);
                var srcR = sourcePixel.R;
                iMax = Math.Max(iMax, srcR);
            }
        }
        return iMax;
    }
    
    public static byte GetIMin(this Bitmap source)
    {
        var iMin = byte.MaxValue;
        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var sourcePixel = source.GetPixel(x, y);
                var srcR = sourcePixel.R;
                iMin = Math.Min(iMin, srcR);
            }
        }
        return iMin;
    }
    
    public static IEnumerable<double> GetFlatten(this Bitmap source)
    {
        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var intensity = source.GetPixel(x, y).R;
                yield return intensity;
            }
        }
    }

    public static Bitmap Transform(this Bitmap source, Func<Color, Color> action)
    {
        var result = new Bitmap(source.Width, source.Height);
        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var pixel = source.GetPixel(x, y);
                var transformed = action(pixel);
                result.SetPixel(x, y, transformed);
            }
        }
        return result;
    }

    public static Color ByteToColor(byte color) => Color.FromArgb(color, color, color);
    
    public static Bitmap Transform<T>(this Bitmap source, Func<Color, T> action) where T : INumber<T> =>
        source.Transform(color => ByteToColor(Convert.ToByte(action(color))));
    
    public static Bitmap AddSaltAndPepperNoise(this Bitmap image, double density)
    {
        var noisyImage = new Bitmap(image);
        var rand = new Random();

        var numNoisyPixels = (int)(density * image.Width * image.Height);

        for (var i = 0; i < numNoisyPixels; i++)
        {
            var x = rand.Next(image.Width);
            var y = rand.Next(image.Height);

            var noiseColor = rand.NextDouble() < 0.5 ? Color.Black : Color.White;
            noisyImage.SetPixel(x, y, noiseColor);
        }

        return noisyImage;
    }
    
    public static Bitmap ApplyMeanFilter(this Bitmap image, int filterSize)
    {
        var result = new Bitmap(image.Width, image.Height);

        var offset = filterSize / 2;
        for (var y = offset; y < image.Height - offset; y++)
        {
            for (var x = offset; x < image.Width - offset; x++)
            {
                int sumR = 0, sumG = 0, sumB = 0;

                for (var fy = -offset; fy <= offset; fy++)
                {
                    for (var fx = -offset; fx <= offset; fx++)
                    {
                        var pixel = image.GetPixel(x + fx, y + fy);

                        sumR += pixel.R;
                        sumG += pixel.G;
                        sumB += pixel.B;
                    }
                }

                var numPixels = filterSize * filterSize;
                var avgR = sumR / numPixels;
                var avgG = sumG / numPixels;
                var avgB = sumB / numPixels;

                result.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
            }
        }

        return result;
    }
    
    public static Bitmap ApplyMedianFilter(this Bitmap image, int filterSize)
    {
        var result = new Bitmap(image.Width, image.Height);

        var offset = filterSize / 2;
        for (var y = offset; y < image.Height - offset; y++)
        {
            for (var x = offset; x < image.Width - offset; x++)
            {
                var rValues = new int[filterSize * filterSize];
                var gValues = new int[filterSize * filterSize];
                var bValues = new int[filterSize * filterSize];

                var index = 0;
                
                for (var fy = -offset; fy <= offset; fy++)
                {
                    for (var fx = -offset; fx <= offset; fx++)
                    {
                        var pixel = image.GetPixel(x + fx, y + fy);

                        rValues[index] = pixel.R;
                        gValues[index] = pixel.G;
                        bValues[index] = pixel.B;
                        index++;
                    }
                }
                
                var medianR = GetMedian(rValues);
                var medianG = GetMedian(gValues);
                var medianB = GetMedian(bValues);
                
                result.SetPixel(x, y, Color.FromArgb(medianR, medianG, medianB));
            }
        }

        return result;
    }

    private static int GetMedian(int[] values)
    {
        Array.Sort(values);
        var middle = values.Length / 2;
        return values[middle];
    }
}