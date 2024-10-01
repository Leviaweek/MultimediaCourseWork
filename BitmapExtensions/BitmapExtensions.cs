using System.Runtime.Versioning;

namespace BitmapExtensions;
using System.Drawing;

[SupportedOSPlatform("windows")]
public static class BitmapExtensions
{
    public static Bitmap ToGrayscale(this Bitmap source)
    {
        var width = source.Width;
        var height = source.Height;
        var resultImage = new Bitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var sourcePixel = source.GetPixel(x, y);
                var grey = (byte)(0.299 * sourcePixel.R + 0.587 * sourcePixel.G + 0.114 * sourcePixel.B);
                var greyPixel = Color.FromArgb(grey, grey, grey);
                resultImage.SetPixel(x, y, greyPixel);
            }
        }
        return resultImage;
    }

    public static Bitmap ToTransformedGrayscale(this Bitmap source,
        double gamma,
        double low,
        double high,
        byte iMin,
        byte iMax)
    {
        var width = source.Width;
        var height = source.Height;
        var resultImage = new Bitmap(width, height);
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var sourcePixel = source.GetPixel(x, y);
                var grayPixel = sourcePixel.R;

                var normalized = (grayPixel - iMin) / (double)(iMax - iMin);
                var gammaCorrected = Math.Pow(normalized, gamma);
                var transformed = gammaCorrected * (high - low) + low;
                
                var final = (byte)Math.Max(low, Math.Min(high, transformed));
                resultImage.SetPixel(x, y, Color.FromArgb(final, final, final));
            }
        }
        return resultImage;
    }

    public static Bitmap ToNegative(this Bitmap source, byte iMax)
    {
        var width = source.Width;
        var height = source.Height;
        var negativeImage = new Bitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = source.GetPixel(x, y);
                var grayValue = pixel.R;
                
                var negativeValue = (byte)(255 - grayValue);
                
                negativeImage.SetPixel(x, y, Color.FromArgb(negativeValue, negativeValue, negativeValue));
            }
        }
        return negativeImage;
    }
    
    public static byte GetIMax(this Bitmap source)
    {
        var iMax = byte.MinValue;
        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var sourcePixel = source.GetPixel(x, y);
                var srcR = sourcePixel.R;
                if (srcR > iMax)
                    iMax = srcR;
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
                if (srcR < iMin)
                    iMin = srcR;
            }
        }
        return iMin;
    }
    
    public static IEnumerable<double> GetHistogram(this Bitmap source)
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

    public static Bitmap GetEqualized(this Bitmap source, double[] sourceArray)
    {
        var result = new Bitmap(source.Width, source.Height);
        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var pixel = source.GetPixel(x, y);
                var item = (byte)sourceArray[pixel.R];
                result.SetPixel(x, y, Color.FromArgb(item, item, item));
            }
        }

        return result;
    }
}