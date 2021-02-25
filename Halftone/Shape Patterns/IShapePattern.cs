namespace Halftone
{
    using System.Drawing;

    public interface IShapePattern
    {
        bool AntialiasingRequired();

        void Draw(Graphics graphics, RectangleF rect, Color color);
    }
}
