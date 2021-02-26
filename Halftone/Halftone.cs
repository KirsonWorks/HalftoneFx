namespace Halftone
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Threading;

    public enum HalftoneShapeSizing
    {
        None,
        Brightness,
        BrightnessInverted,
        AlphaChannel,
    }

    public class Halftone
    {
        private int gridType = 0;

        private int patternType = 0;
        
        private int shapeSizing = 0;

        private int cellSize = 8;

        private float cellScale = 1.0f;

        private bool enabled = true;

        private bool transparentBg = true;

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

        public int ShapeSizing
        {
            get => this.shapeSizing;

            set
            {
                if (this.shapeSizing != value)
                {
                    this.shapeSizing = value;
                    this.DoPropertyChanged();
                }
            }
        }

        public Bitmap Generate(Bitmap image, Action<float> progress, CancellationToken token)
        {
            if (!this.Enabled)
            {
                return new Bitmap(image);
            }

            var width = image.Width;
            var height = image.Height;
            int shapeSize = (int)(this.CellSize * this.CellScale);
            var half = this.cellSize / 2;
            var result = new Bitmap(width, height);
            var shapeSizing = (HalftoneShapeSizing)this.ShapeSizing;
            var pattern = ShapePatternFactory.GetPattern((ShapePatternType)this.patternType);
            var grid = GridPatternFactory.GetPattern((GridPatternType)this.gridType, width, height, this.cellSize);
            
            using (var graphics = Graphics.FromImage(result))
            {
                if (!this.transparentBg)
                {
                    if (shapeSizing == HalftoneShapeSizing.BrightnessInverted)
                    {
                        graphics.Clear(Color.White);
                    }
                    else
                    {
                        graphics.Clear(Color.Black);
                    }
                }

                if (pattern.AntialiasingRequired())
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                }
                else
                {
                    graphics.SmoothingMode = SmoothingMode.HighSpeed;
                }
                
                while (grid.MoveNext())
                {
                    token.ThrowIfCancellationRequested();

                    var cell = grid.Current;
                    var xPixel = Math.Min(cell.X + half, width - 1);
                    var yPixel = Math.Min(cell.Y + half, height - 1);
                    var color = image.GetPixel(xPixel, yPixel);

                    if (color.A == 0)
                    {
                        continue;
                    }

                    var scale = 1.0f;

                    switch (shapeSizing)
                    {
                        case HalftoneShapeSizing.Brightness:
                            scale = color.GetBrightness();
                            break;

                        case HalftoneShapeSizing.BrightnessInverted:
                            scale = 1.0f - color.GetBrightness();
                            break;

                        case HalftoneShapeSizing.AlphaChannel:
                            scale = (float)color.A / 255.0f;
                            break;
                    }

                    var sz = shapeSize * scale;
                    var offset = ((float)cellSize / 2) - (sz / 2);
                    var rect = new RectangleF(cell.X + offset, cell.Y + offset, sz, sz);

                    pattern.Draw(graphics, rect, color);

                    var part = Math.Max(1, grid.CellCount / 10);

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
