namespace Halftone
{
    using System.Drawing;

    public interface IShapePattern
    {
        bool AntialiasingRequired();

        void Draw(Graphics graphics, Rectangle rect, Color color);
    }
}
