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
    }
}
