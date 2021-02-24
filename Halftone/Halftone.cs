namespace Halftone
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class Halftone
    {
        private int gridType = 0;

        private int patternType = 0;

        private int cellSize = 10;

        private float cellScale = 1.0f;

        private bool enabled = true;

        public event EventHandler OnPropertyChanged = delegate { };

        public int GridType
        {
            get => this.gridType;

            set
            {
                if (this.gridType != value)
                {
                    this.gridType = value;
                    this.DoPropertyChanged();
                }
            }
        }

        public int PatternType
        {
            get => this.patternType;

            set
            {
                if (this.patternType != value)
                {
                    this.patternType = value;
                    this.DoPropertyChanged();
                }
            }
        }

        public int CellSize
        {
            get => this.cellSize;

            set
            {
                if (this.cellSize != value)
                {
                    this.cellSize = value;
                    this.DoPropertyChanged();
                }
            }
        }

        public float CellScale
        {
            get => this.cellScale;
            
            set
            {
                var val = Math.Max(0, value);

                if (this.cellScale != val)
                {
                    this.cellScale = val;
                    this.DoPropertyChanged();
                }
            }
        }

        public bool Enabled
        {
            get => this.enabled;

            set
            {
                if (this.enabled != value)
                {
                    this.enabled = value;
                    this.DoPropertyChanged();
                }
            }
        }

        public Bitmap Generate(Bitmap image)
        {
            if (!this.Enabled)
            {
                return new Bitmap(image);
            }

            var width = image.Width;
            var height = image.Height;
            int size = (int)(this.CellSize * this.CellScale);
            int offset = this.CellSize - size;
            var half = this.cellSize / 2;
            var result = new Bitmap(width, height);
            var pattern = ShapePatternFactory.GetPattern((ShapePatternType)this.patternType);
            var grid = GridPatternFactory.GetPattern((GridPatternType)this.gridType, width, height, this.cellSize);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed; // need option.
                
                while (grid.MoveNext())
                {
                    var cell = grid.Current;
                    var xPixel = Math.Min(cell.X + half, width - 1);
                    var yPixel = Math.Min(cell.Y + half, height - 1);
                    var color = image.GetPixel(xPixel, yPixel);

                    if (color.A == 0)
                    {
                        continue;
                    }

                    var rect = new Rectangle(cell.X + offset, cell.Y + offset, size, size);
                    pattern.Draw(graphics, rect, color);
                }
            }

            return result;
        }

        private void DoPropertyChanged()
        {
            this.OnPropertyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
