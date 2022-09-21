namespace Renderer;

public static class Utils
{
    public static double DegToRad(this double deg)
    {
        return deg * Math.PI / 180.0;
    }

    public static double RadToDeg(this double rad)
    {
        return rad * 180.0 / Math.PI;
    }
}
