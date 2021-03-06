﻿namespace Halftone
{
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ShapePatternCustom : IShapePattern
    {
        private readonly Image pattern;

        private readonly ImageAttributes attribs;

        public ShapePatternCustom(Image pattern)
        {
            this.pattern = new Bitmap(pattern);
            this.attribs = new ImageAttributes();
        }

        public bool AntialiasingRequired() => false;

        public void Draw(Graphics graphics, RectangleF rect, Color color)
        {
            if (this.pattern != null)
            {
                var matrix = new ColorMatrix
                {
                    Matrix00 = color.R / 255.0f,
                    Matrix11 = color.G / 255.0f,
                    Matrix22 = color.B / 255.0f,
                    Matrix33 = color.A / 255.0f
                };

                this.attribs.SetColorMatrix(matrix);

                graphics.DrawImage(this.pattern, Rectangle.Round(rect), 0, 0, pattern.Width, pattern.Height, GraphicsUnit.Pixel, this.attribs);
            }
        }
    }
}
