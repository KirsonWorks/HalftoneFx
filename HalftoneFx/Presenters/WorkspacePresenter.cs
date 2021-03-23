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

        private readonly HalftoneImage halftone;

        private Image pictureForSave;

        public WorkspacePresenter(IWorkspaceView view)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.view.Presenter = this;

            this.halftone = new HalftoneImage();
            this.halftone.OnProgress += OnProgress;
            this.halftone.OnImageAvailable += this.OnImageAvailable;
            this.halftone.OnThumbnailAvailable += this.OnThumbnailAvailable;
            
            this.view.SetUp();
            this.LoadPicture(Properties.Resources.Logo);
        }

        public bool Smoothing
        {
            get => this.halftone.Smoothing.Value == 1;
            set => this.halftone.Smoothing.Value = Convert.ToInt32(value);
        }

        public bool Grayscale
        {
            get => this.halftone.Grayscale.Value == 1;
            set => this.halftone.Grayscale.Value = Convert.ToInt32(value);
        }

        public bool Negative
        {
            get => this.halftone.Negative.Value == 1;
            set => this.halftone.Negative.Value = Convert.ToInt32(value);
        }

        public int Brightness
        {
            get => this.halftone.Brightness.Value;
            set => this.halftone.Brightness.Value = value;
        }

        public Range<int> BrightnessRange => this.halftone.Brightness.GetRange();

        public int Contrast
        {
            get => this.halftone.Contrast.Value;
            set => this.halftone.Contrast.Value = value;
        }

        public Range<int> ContrastRange => this.halftone.Contrast.GetRange();

        public int Quantization
        {
            get => this.halftone.Quantization.Value;
            set => this.halftone.Quantization.Value = value;
        }

        public Range<int> QuantizationRange => this.halftone.Quantization.GetRange();

        public int Dithering
        {
            get => this.halftone.Dithering.Value;
            set => this.halftone.Dithering.Value = value;
        }

        public Range<int> DitheringRange => this.halftone.Dithering.GetRange();

        public bool HalftoneEnabled
        {
            get => this.halftone.HalftoneEnabled;
            set => this.halftone.HalftoneEnabled = value; 
        }

        public int GridType
        {
            get => this.halftone.GridType;
            set => this.halftone.GridType = value;
        }

        public int ShapeType
        {
            get => this.halftone.ShapeType;
            set => this.halftone.ShapeType = value;
        }

        public int ShapeSizeBy
        {
            get => this.halftone.ShapeSizeBy;
            set => this.halftone.ShapeSizeBy = value;
        }

        public int CellSize
        {
            get => this.halftone.CellSize;
            set => this.halftone.CellSize = value;
        }

        public float CellScale
        {
            get => this.halftone.CellScale;
            set => this.halftone.CellScale = value;
        }

        public void LoadPicture(Image picture)
        {
            this.halftone.Image = this.pictureForSave = picture;
            this.view.SetPicture(picture);
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
                this.halftone.CustomPattern = new Bitmap(pattern);
            }
            catch
            {
                this.view.Error($"Can't load pattern from the file {path}");
            }
        }

        public void ClearPattern()
        {
            this.view.SetPattern(null);
            this.halftone.CustomPattern = null;
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
    }
}
