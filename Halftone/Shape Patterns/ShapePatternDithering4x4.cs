namespace Halftone
{
    using System;
    using System.Drawing;

    public class ShapePatternDithering4x4 : IShapePattern
    {
        private const int Columns = 4;

        private const int TableSize = Columns * Columns;

        private readonly int[] table = new int[TableSize] { 0, 10, 2, 8, 5, 15, 7, 13, 1, 11, 3, 9, 4, 14, 6, 12 };

        public bool AntialiasingRequired() => false;

        public void Draw(Graphics graphics, RectangleF rect, Color color)
        {
            var shade = (int)Math.Round(color.GetBrightness() * TableSize);
           
            using (var brush = new SolidBrush(color))
            {
                for (var i = 0; i < shade; i++)
                {
                    var pos = table[i];
                    var size = (float)rect.Width / Columns;
                    var x = (pos % Columns) * size;
                    var y = (pos / Columns) * size;

                    graphics.FillRectangle(brush, new RectangleF(rect.X + x, rect.Y + y, size, size));
                }
            }
        }
    }
}
