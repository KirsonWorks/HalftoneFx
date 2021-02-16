namespace HalftoneFx.GFX
{
    using System;
    using System.Drawing;

    public class ImageHalftone
    {
        public Image Generate(Bitmap image)
        {
            var columns = 200;
            var width = image.Width;
            var height = image.Height;
            
            var result = new Bitmap(width, height);
            
            using (var graphics = Graphics.FromImage(result))
            {
                var cellSize = (int)Math.Ceiling((double)width / columns);
                var rows = (int)Math.Ceiling((double)height / cellSize);

                for (var i = 0; i < columns * rows; i++)
                {
                    var x = (i % columns) * cellSize;
                    var y = (i / columns) * cellSize;

                    var xPixel = Math.Min(x + cellSize / 2, width - 1);
                    var yPixel = Math.Min(y + cellSize / 2, height - 1);
                    var color = image.GetPixel(xPixel, yPixel);

                    using (var brush = new SolidBrush(color))
                    {
                        graphics.FillRectangle(brush, new Rectangle(x, y, cellSize, cellSize));
                    }
                }
            }

            return result;
        }
    }
}
