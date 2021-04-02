namespace KWUI.Helpers
{
    using System;
    using System.Drawing;

    public static class RectFHelper
    {
        public static RectangleF Inflate(this RectangleF rect, float value)
        {
            var newRect = rect;
            newRect.Inflate(value, value);
            return newRect;
        }

        public static RectangleF Inside(this RectangleF rect, RectangleF other)
        {
            var diffLeft = rect.Left - other.Left;
            var diffTop = rect.Top - other.Top;
            var diffRight = rect.Right - other.Right;
            var diffBottom = rect.Bottom - other.Bottom;

            rect.X -= +Math.Min(0, diffLeft) + Math.Max(0, diffRight);
            rect.Y -= +Math.Min(0, diffTop) + Math.Max(0, diffBottom);

            return rect;
        }
    }
}
