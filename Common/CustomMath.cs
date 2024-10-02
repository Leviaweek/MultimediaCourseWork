namespace Common;

public static unsafe class CustomMath
{
    public static int Step(double x, double y)
    {
        var result = x - y;
        var i = *(int*)&result;
        var mask = (i >> 31) & 1;
        return 1 - mask;
    }
}