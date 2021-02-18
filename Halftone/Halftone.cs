namespace Halftone
{
    using System;
    using System.Drawing;

    public class Halftone
    {
        private int size;

        public event EventHandler OnPropertyChanged = delegate { };

        public int Size 
        {
            get => this.size;

            set
            {
                if (this.size != value)
                {
                    this.size = value;
                    this.OnPropertyChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Bitmap Generate(Bitmap image)
        {
            if (this.Size <= 0)
            {
                return new Bitmap(image);
            }

            var columns = this.Size;
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

                    var brightness = color.GetBrightness();

                    if (brightness == 0)
                    {
                        continue;
                    }


                    var dotSize = brightness * (cellSize - 1.0f);
                    var offset = (cellSize / 2.0f) - (dotSize / 2.0f);

                    using (var brush = new SolidBrush(color))
                    {
                        graphics.FillEllipse(brush, new RectangleF(x + offset, y + offset, dotSize, dotSize));
                    }
                }
            }

            return result;
        }
    }
}
