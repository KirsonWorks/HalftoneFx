namespace Halftone
{
    using System;
    using System.Drawing;

    public class ShapePatternTest : IShapePattern
    {
        private int[] pattern = new int[16] { 0, 10, 2, 8, 5, 15, 7, 13, 1, 11, 3, 9, 4, 14, 6, 12 };

        public void Draw(Graphics graphics, Rectangle rect, Color color)
        {
            var brightness = (int)Math.Ceiling(color.GetBrightness() * 16);

            using (var brush = new SolidBrush(color))
            {
                for (var i = 0; i < brightness; i++)
                {
                    var p = pattern[i];
                    var size = (float)rect.Width / 4;

                    var x = p % 4 * size;
                    var y = p / 4 * size;

                    graphics.FillRectangle(brush, new RectangleF(rect.X + x, rect.Y + y, size, size));
                }
            }
        }
    }
}
