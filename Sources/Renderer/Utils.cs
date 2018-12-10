using System;

namespace Renderer
{
    public static class Utils
    {
        /// <summary>
        /// Переводит градусы в радианы.
        /// </summary>
        public static double DegToRad(this double deg)
        {
            return deg * Math.PI / 180.0;
        }

        /// <summary>
        /// Переводит радианы в градусы.
        /// </summary>
        public static double RadToDeg(this double rad)
        {
            return rad * 180.0 / Math.PI;
        }

        /// <summary>
        /// Обменивает значения двух переменных.
        /// </summary>
        public static void Exchange<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}
