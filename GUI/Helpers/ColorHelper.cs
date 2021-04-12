namespace KWUI.Helpers
{
    using System.Drawing;

    public static class ColorHelper
    {
        public static float GetLuminance(this Color color)
        {
            return (0.2126f * color.R + 0.7152f * color.G + 0.0722f * color.B) / 255.0f;
        }

        public static Color Blend(this Color color, Color other, float t)
        {
            var r = color.R + (other.R - color.R) * t;
            var g = color.G + (other.G - color.G) * t;
            var b = color.B + (other.B - color.B) * t;

            return Color.FromArgb((byte)r, (byte)g, (byte)b);
        }
    }
}
