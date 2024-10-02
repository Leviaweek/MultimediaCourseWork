namespace Common;

using System.Drawing;
using System.Runtime.Versioning;

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

    public static Bitmap Transform<T>(this Bitmap source, Func<Color, T> action, Func<T, Color> calculatePixel)
    {
        var result = new Bitmap(source.Width, source.Height);
        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var pixel = source.GetPixel(x, y);
                var transformed = action(pixel);
                var color = calculatePixel(transformed);
                result.SetPixel(x, y, color);
            }
        }
        return result;
    }
}