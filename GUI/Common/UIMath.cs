namespace KWUI.Common
{
    using System;
    using System.Drawing;

    public static class UIMath
    { 
        public static T Clamp<T> (T value, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(value) > 0)
            {
                return min;
            }
            else
            {
                if (max.CompareTo(value) < 0)
                {
                    return max;
                }
            }

            return value;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float Snap(float value, float step)
        {
            if (step == 0)
            {
                return value;
            }

            return (float)Math.Floor((value + 0.5F * step) / step) * step;
        }

        public static PointF Snap(PointF value, float step)
        {
            return new PointF(Snap(value.X, step), Snap(value.Y, step));
        }

        public static double Distance(PointF a, PointF b)
        {
            var diff = new PointF(b.X - a.X, b.Y - a.Y);
            return Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
        }
    }
}
