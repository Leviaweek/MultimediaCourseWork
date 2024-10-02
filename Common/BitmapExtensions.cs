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
}