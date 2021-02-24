namespace Halftone
{
    using System;
    using System.Drawing;

    public class ShapePatternDithering4x4 : IShapePattern
    {
        private const int TableCols = 4;

        private const int TableSize = TableCols * TableCols;

        private int[] table = new int[TableSize] { 0, 10, 2, 8, 5, 15, 7, 13, 1, 11, 3, 9, 4, 14, 6, 12 };

        public void Draw(Graphics graphics, Rectangle rect, Color color)
        {
            var brightness = (int)Math.Round(color.GetBrightness() * TableSize);
           
            using (var brush = new SolidBrush(color))
            {
                for (var i = 0; i < brightness; i++)
                {
                    var pos = table[i];
                    var size = (float)rect.Width / TableCols;
                    var x = (pos % TableCols) * size;
                    var y = (pos / TableCols) * size;

                    graphics.FillRectangle(brush, new RectangleF(rect.X + x, rect.Y + y, size, size));
                }
            }
        }
    }
}
