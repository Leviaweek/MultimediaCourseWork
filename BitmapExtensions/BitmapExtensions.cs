namespace BitmapExtensions;

using System.Drawing;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
public static class BitmapExtensions
{
    public static Bitmap ToNegative(this Bitmap source)
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
}