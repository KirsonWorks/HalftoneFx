namespace Halftone
{
    using System.Drawing;

    public class ShapePatternSquare : IShapePattern
    {
        public void Draw(Graphics graphics, Rectangle rect, Color color)
        {
            using (var brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, rect);
            }
        }
    }
}
