namespace Halftone
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Threading;

    public enum HalftoneShapeSizeBy
    {
        None,
        Brightness,
        BrightnessInv,
        Alpha,
        Max
    }

    public class HalftoneArtist
    {
        private int gridType = 0;

        private int shapeType = 0;
        
        private int shapeSizeBy = 0;

        private int cellSize = 4;

        private float cellScale = 1.0f;

        private bool enabled = false;

        private Color fgColor = Color.FromArgb(0, Color.White);

        private Color bgColor = Color.FromArgb(0, Color.Black);

        private Image customPattern;

        public event EventHandler OnPropertyChanged = delegate { };

        public int GridType
        {
            get => this.gridType;
            set => this.DoPropertyChanged(ref gridType, value);
        }

        public int ShapeType
        {
            get => this.shapeType;
            set => this.DoPropertyChanged(ref shapeType, value);
        }

        public int ShapeSizeBy
        {
            get => this.shapeSizeBy;
            set => this.DoPropertyChanged(ref shapeSizeBy, value);
        }

        public int CellSize
        {
            get => this.cellSize;
            set => this.DoPropertyChanged(ref cellSize, value);
        }

        public float CellScale
        {
            get => this.cellScale;
            set => this.DoPropertyChanged(ref cellScale, Math.Max(0, value));
        }

        public bool Enabled
        {
            get => this.enabled;
            set => this.DoPropertyChanged(ref enabled, value);
        }

        public Color ForegroundColor
        {
            get => this.fgColor;
            set => this.DoPropertyChanged(ref fgColor, value);
        }

        public Color BackgroundColor
        {
            get => this.bgColor;
            set => this.DoPropertyChanged(ref bgColor, value);
        }

        public Image CustomPattern
        {
            get => this.customPattern;
            set => this.DoPropertyChanged(ref customPattern, value);
        }

        public Bitmap Generate(Bitmap image, Action<float> progress, CancellationToken token)
        {
            if (!this.Enabled)
            {
                progress?.Invoke(0.0f);
                return new Bitmap(image);
            }

            var width = image.Width;
            var height = image.Height;
            var result = new Bitmap(width, height);
            var halfSize =  (int)Math.Ceiling((float)this.CellSize / 2);
            int shapeSize = (int)(this.CellSize * this.CellScale);
            var shapeSizing = (HalftoneShapeSizeBy)this.ShapeSizeBy;
            var grid = GridPatternFactory.GetPattern((HalftoneGridType)this.gridType, width, height, this.cellSize);

            IShapePattern pattern = this.customPattern != null ?
                    new ShapePatternCustom(this.customPattern) :
                    ShapePatternFactory.GetPattern((HalftoneShapeType)this.shapeType);

            using (var graphics = Graphics.FromImage(result))
            {
                if (!this.bgColor.IsEmpty && this.bgColor.A > 0)
                {
                    graphics.Clear(this.bgColor);
                }

                graphics.SmoothingMode = pattern.AntialiasingRequired() ?
                                            SmoothingMode.AntiAlias :
                                            SmoothingMode.HighSpeed;

                while (grid.MoveNext())
                {
                    token.ThrowIfCancellationRequested();

                    var cell = grid.Current;
                    var xPixel = Math.Min(cell.X + halfSize, width - 1);
                    var yPixel = Math.Min(cell.Y + halfSize, height - 1);
                    var color = image.GetPixel(xPixel, yPixel);
                    
                    if (color.A != 0)
                    {
                        var scale = 1.0f;

                        switch (shapeSizing)
                        {
                            case HalftoneShapeSizeBy.Brightness:
                                scale = color.GetBrightness();
                                break;

                            case HalftoneShapeSizeBy.BrightnessInv:
                                scale = 1.0f - color.GetBrightness();
                                break;

                            case HalftoneShapeSizeBy.Alpha:
                                scale = (float)color.A / 255.0f;
                                break;
                        }

                        if (scale > float.Epsilon)
                        {
                            var size = shapeSize * scale;
                            var offset = ((float)cellSize - size) / 2;
                            var rect = new RectangleF(cell.X + offset, cell.Y + offset, size, size);

                            if (!this.fgColor.IsEmpty && this.fgColor.A > 0)
                            {
                                color = this.fgColor;
                            }

                            pattern.Draw(graphics, rect, color);
                        }
                    }

                    var part = Math.Max(1, (grid.CellCount - 1) / 10);

                    if (grid.Position % part == 0)
                    {
                        progress?.Invoke((float)(grid.Position + 1) / grid.CellCount);
                    }
                }
            }

            return result;
        }

        private void DoPropertyChanged<T>(ref T obj, T value)
        {
            if (obj == null || !obj.Equals(value))
            {
                obj = value;
                this.OnPropertyChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
