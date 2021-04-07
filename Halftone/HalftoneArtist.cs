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

        private bool enabled = true;

        private bool transparentBg = true;

        private Image customPattern;

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

        public int ShapeType
        {
            get => this.shapeType;

            set
            {
                if (this.shapeType != value)
                {
                    this.shapeType = value;
                    this.DoPropertyChanged();
                }
            }
        }

        public int ShapeSizeBy
        {
            get => this.shapeSizeBy;

            set
            {
                if (this.shapeSizeBy != value)
                {
                    this.shapeSizeBy = value;
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

        public bool TransparentBg
        {
            get => this.transparentBg;

            set
            {
                if (this.transparentBg != value)
                {
                    this.transparentBg = value;
                    this.DoPropertyChanged();
                }
            }
        }

        public Image CustomPattern
        {
            get => this.customPattern;

            set
            {
                this.customPattern = value;
                this.DoPropertyChanged();
            }
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
                    new ShapePatternCustom(this.CustomPattern) :
                    ShapePatternFactory.GetPattern((HalftoneShapeType)this.shapeType);
            
            using (var graphics = Graphics.FromImage(result))
            {
                if (!this.transparentBg)
                {
                    if (shapeSizing == HalftoneShapeSizeBy.BrightnessInv)
                    {
                        graphics.Clear(Color.White);
                    }
                    else
                    {
                        graphics.Clear(Color.Black);
                    }
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

                            pattern.Draw(graphics, rect, color);
                        }
                    }

                    var part = Math.Max(1, (grid.CellCount - 1) / 10);

                    if (grid.Position % part == 0)
                    {
                        progress?.Invoke((float)grid.Position / grid.CellCount);
                    }
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
