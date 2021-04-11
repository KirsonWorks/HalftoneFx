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

        private readonly ColorPalettes palettes;

        private Image pictureForSave;

        public WorkspacePresenter(IWorkspaceView view)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.view.Presenter = this;

            this.palettes = new ColorPalettes();
            this.palettes.OnAdded += OnPalettesChanged;
            this.palettes.OnRemoved += OnPalettesChanged;

            this.image = new HalftoneImage(this.palettes);
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

        public ColorPalettes Palettes => this.palettes;

        public Range<int> PaletteRange => new Range<int>(0, this.palettes.Count);

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

        public int Dithering
        {
            get => this.image.Dithering.Value;
            set => this.image.Dithering.Value = value;
        }

        public Range<int> DitheringRange => this.image.Dithering.GetRange();

        public bool HalftoneEnabled
        {
            get => this.image.HalftoneEnabled;
            set => this.image.HalftoneEnabled = value; 
        }

        public int GridType
        {
            get => this.image.GridType;
            set => this.image.GridType = value;
        }

        public int ShapeType
        {
            get => this.image.ShapeType;
            set => this.image.ShapeType = value;
        }

        public int ShapeSizeBy
        {
            get => this.image.ShapeSizeBy;
            set => this.image.ShapeSizeBy = value;
        }

        public int CellSize
        {
            get => this.image.CellSize;
            set => this.image.CellSize = value;
        }

        public float CellScale
        {
            get => this.image.CellScale;
            set => this.image.CellScale = value;
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
                this.image.CustomPattern = new Bitmap(pattern);
            }
            catch
            {
                this.view.Error($"Can't load pattern from the file {path}");
            }
        }

        public void ClearPattern()
        {
            this.view.SetPattern(null);
            this.image.CustomPattern = null;
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
            GC.Collect();
        }

        private void OnThumbnailAvailable(object sender, GenerateDoneEventArgs e)
        {
            this.view.UpdatePicture(e.Image);
        }

        private void OnPalettesChanged(object sender, EventArgs e)
        {
            this.image.Palette.MaxValue = this.palettes.Count;
            this.view.UpdatePalettes();
        }
    }
}
