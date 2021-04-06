namespace KWUI.Helpers
{
    using System;
    using System.Drawing;

    public static class PointFHelper
    {
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
