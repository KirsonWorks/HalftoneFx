using System.Drawing;

namespace Halftone
{
    public class ShapePatternCircle : IShapePattern
    {
        public void Draw(Graphics graphics, Rectangle rect, Color color)
        {
            using (var brush = new SolidBrush(color))
            {
                graphics.FillEllipse(brush, rect);
            }
        }
    }
}
