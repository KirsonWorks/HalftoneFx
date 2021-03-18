namespace GUI.Helpers
{
    using System;
    using System.Drawing;

    public static class PointFHelper
    {
        public static PointF Add(this PointF point, PointF other)
        {
            point.X += other.X;
            point.Y += other.Y;
            return point;
        }

        public static PointF Add(this PointF point, float x, float y)
        {
            return point.Add(new PointF(x, y));
        }

        public static PointF Min(this PointF point, PointF other)
        {
            return new PointF(Math.Min(point.X, other.X), Math.Min(point.Y, other.Y));
        }

        public static PointF Max(this PointF point, PointF other)
        {
            return new PointF(Math.Max(point.X, other.X), Math.Max(point.Y, other.Y));
        }

        public static SizeF ToSize(this PointF point)
        {
            return new SizeF(point.X, point.Y);
        }
    }
}
