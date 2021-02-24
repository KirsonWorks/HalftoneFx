namespace Halftone
{
    using System.Drawing;

    public interface IShapePattern
    {
        void Draw(Graphics graphics, Rectangle rect, Color color);
    }
}
