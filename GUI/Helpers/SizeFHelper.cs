namespace GUI.Helpers
{
    using System;
    using System.Drawing;

    public static class SizeFHelper
    {
        public static SizeF OneValue(this SizeF size, float value)
        {
            size.Width = size.Height = value;
            return size;
        }

        public static SizeF Max(this SizeF size, SizeF other)
        {
            return new SizeF(
                Math.Max(size.Width, other.Width),
                Math.Max(size.Height, other.Height));
        }

        public static float Aspect(this SizeF size)
        {
            try
            {
                return size.Width / size.Height;
            }
            catch
            {
                return float.NaN;
            }
        }
    }
}
