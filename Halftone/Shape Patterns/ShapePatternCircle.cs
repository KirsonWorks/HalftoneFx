namespace Halftone
{
    using System.Drawing;

    public class ShapePatternCircle : IShapePattern
    {
        public bool AntialiasingRequired() => true;

        public void Draw(Graphics graphics, RectangleF rect, Color color)
        {
            using (var brush = new SolidBrush(color))
            {
                graphics.FillEllipse(brush, rect);
            }
        }
    }
}
