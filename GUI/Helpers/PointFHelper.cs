namespace GUI.Helpers
{
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

        public static SizeF ToSize(this PointF point)
        {
            return new SizeF(point.X, point.Y);
        }
    }
}
