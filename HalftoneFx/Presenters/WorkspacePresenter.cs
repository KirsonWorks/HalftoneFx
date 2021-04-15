namespace HalftoneFx.Presenters
{
    using Common;

    using HalftoneFx.Helpers;
    using HalftoneFx.Models;
    using HalftoneFx.Views;

    using System;    
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class WorkspacePresenter
    {
        private readonly IWorkspaceView view;

        private readonly HalftoneImage image;

        private Image pictureForSave;

        public WorkspacePresenter(IWorkspaceView view)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.view.Presenter = this;

            this.Palettes = new ColorPalettes();
            this.Palettes.OnAdded += OnPalettesChanged;
            this.Palettes.OnRemoved += OnPalettesChanged;

            this.image = new HalftoneImage(this.Palettes);
            this.image.OnProgress += OnProgress;
            this.image.OnImageAvailable += this.OnImageAvailable;
            this.image.OnThumbnailAvailable += this.OnThumbnailAvailable;
            
            this.view.SetUp();
            this.LoadPicture(Properties.Resources.Logo);
        }

        public bool Smoothing
        {
            get => this.image.Smoothing.Value == 1;
            set => this.image.Smoothing.Value = Convert.ToInt32(value);
        }

        public bool Negative
        {
            get => this.image.Negative.Value == 1;
            set => this.image.Negative.Value = Convert.ToInt32(value);
        }

        public int PaletteIndex
        {
            get => this.image.PaletteIndex;
            set => this.image.PaletteIndex = value;
        }

        public ColorPalettes Palettes { get; }

        public Range<int> PaletteRange => new Range<int>(0, this.Palettes.Count);

        public int Brightness
        {
            get => this.image.Brightness.Value;
            set => this.image.Brightness.Value = value;
        }

        public Range<int> BrightnessRange => this.image.Brightness.GetRange();

        public int Contrast
        {
            get => this.image.Contrast.Value;
            set => this.image.Contrast.Value = value;
        }

        public Range<int> ContrastRange => this.image.Contrast.GetRange();

        public int Saturation
        {
            get => this.image.Saturation.Value;
            set => this.image.Saturation.Value = value;
        }

        public Range<int> SaturationRange => this.image.Saturation.GetRange();

        public int Quantization
        {
            get => this.image.Quantization.Value;
            set => this.image.Quantization.Value = value;
        }

        public Range<int> QuantizationRange => this.image.Quantization.GetRange();

        public int DitherMethod
        {
            get => this.image.Palette.DitherMethod;
            set => this.image.Palette.DitherMethod = value;
        }

        public Range<int> DitherMethodRange =>
            new Range<int>(0, this.image.Palette.DitherMethodMax);

        public int DitherAmount
        {
            get => (int)(this.image.Palette.DitherAmount * this.DitherAmountRange.MaxValue);
            set => this.image.Palette.DitherAmount = (float)value / this.DitherAmountRange.MaxValue;
        }

        public Range<int> DitherAmountRange = new Range<int>(0, 100);

        public bool HalftoneEnabled
        {
            get => this.image.Halftone.Enabled;
            set => this.image.Halftone.Enabled = value; 
        }

        public int GridType
        {
            get => this.image.Halftone.GridType;
            set => this.image.Halftone.GridType = value;
        }

        public int ShapeType
        {
            get => this.image.Halftone.ShapeType;
            set => this.image.Halftone.ShapeType = value;
        }

        public int ShapeSizeBy
        {
            get => this.image.Halftone.ShapeSizeBy;
            set => this.image.Halftone.ShapeSizeBy = value;
        }

        public int CellSize
        {
            get => this.image.Halftone.CellSize;
            set => this.image.Halftone.CellSize = value;
        }

        public float CellScale
        {
            get => this.image.Halftone.CellScale;
            set => this.image.Halftone.CellScale = value;
        }

        public Color Foreground
        {
            get => this.image.Halftone.ForegroundColor;
            set => this.image.Halftone.ForegroundColor = value;
        }

        public Color Background
        {
            get => this.image.Halftone.BackgroundColor;
            set => this.image.Halftone.BackgroundColor = value;
        }

        public void LoadPicture(Image picture)
        {
            this.view.SetPicture(picture);
            this.image.Image = this.pictureForSave = picture;
        }

        public void LoadPictureFromFile(string path)
        {
            try
            {
                var image = Image.FromFile(path);
                this.LoadPicture(image);
            }
            catch
            {
                this.view.Error($"Can't load the file {path}");
            }
        }

        public void SavePictureToFile(string path)
        {
            try
            {
                this.pictureForSave.SaveAs(path);
            }
            catch
            {
                this.view.Error($"Can't save the file {path}");
            }
        }

        public void LoadPatternFromFile(string path)
        {
            try
            {
                var pattern = Image.FromFile(path)
                    .Resize(64, 64, InterpolationMode.High);

                this.view.SetPattern(pattern);
                this.image.Halftone.CustomPattern = new Bitmap(pattern);
            }
            catch
            {
                this.view.Error($"Can't load pattern from the file {path}");
            }
        }

        public void ClearPattern()
        {
            this.view.SetPattern(null);
            this.image.Halftone.CustomPattern = null;
        }

        private void OnProgress(object sender, ProgressChangedEventArgs e)
        {
            this.view.SetProgress(e.Percent);
        }

        private void OnImageAvailable(object sender, GenerateDoneEventArgs e)
        {
            if (e.Flags.HasFlag(ImageGenerationFlags.Halftoning))
            {
                this.pictureForSave = e.Image;
            }

            this.view.UpdatePicture(e.Image);
        }

        private void OnThumbnailAvailable(object sender, GenerateDoneEventArgs e)
        {
            this.view.UpdatePicture(e.Image);
        }

        private void OnPalettesChanged(object sender, EventArgs e)
        {
            this.image.Palette.MaxValue = this.Palettes.Count;
            this.view.UpdatePalettes();
        }
    }
}
