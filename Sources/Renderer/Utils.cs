using System;

namespace Renderer
{
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

        public static void Exchange<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}
