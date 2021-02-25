namespace Halftone
{
    using System.Drawing;

    public class ShapePatternSquare : IShapePattern
    {
        public bool AntialiasingRequired() => false;

        public void Draw(Graphics graphics, RectangleF rect, Color color)
        {
            using (var brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, rect);
            }
        }
    }
}
