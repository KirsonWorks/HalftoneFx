namespace GUI.Helpers
{
    using System.Drawing;

    public static class SizeFHelper
    {
        public static SizeF OneValue(this SizeF size, float value)
        {
            size.Width = size.Height = value;
            return size;
        }

        public static string ToStringWxH(this SizeF size)
        {
            return $"{size.Width}x{size.Height}";
        }

        public static float Aspect(this SizeF size)
        {
            if (size.Height > 0)
            {
                return size.Width / size.Height;
            }
            
            if (size.Width > 0)
            {
                return size.Height / size.Width;
            }

            return 1.0f;
        }
    }
}
