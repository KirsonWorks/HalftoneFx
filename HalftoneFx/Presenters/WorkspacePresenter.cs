namespace HalftoneFx.Presenters
{
    using HalftoneFx.Views;

    using System;
    using System.Drawing;

    public class WorkspacePresenter
    {
        private readonly IWorkspaceView view;

        private readonly HalftoneImage halftone;

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


        public int BrightnessMin => this.halftone.Brightness.MinValue;

        public int BrightnessMax => this.halftone.Brightness.MaxValue;

        public int Contrast
        {
            get => this.halftone.Contrast.Value;
            set => this.halftone.Contrast.Value = value;
        }

        public int ContrastMin => this.halftone.Contrast.MinValue;

        public int ContrastMax => this.halftone.Contrast.MaxValue;

        public int Quantization
        {
            get => this.halftone.Quantization.Value;
            set => this.halftone.Quantization.Value = value;
        }

        public int QuantizationMin => this.halftone.Quantization.MinValue;

        public int QuantizationMax => this.halftone.Quantization.MaxValue;

        public int Dithering
        {
            get => this.halftone.Dithering.Value;
            set => this.halftone.Dithering.Value = value;
        }

        public int DitheringMin => this.halftone.Dithering.MinValue;

        public int DitheringMax => this.halftone.Dithering.MaxValue;

        public void LoadPicture(Image picture)
        {
            this.halftone.Image = picture;
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
                
            }
            catch
            {
                this.view.Error($"Can't save the file {path}");
            }
        }

        private void OnProgress(object sender, ProgressChangedEventArgs e)
        {
            this.view.SetProgress(e.Percent);
        }

        private void OnImageAvailable(object sender, GenerateDoneEventArgs e)
        {
            this.view.UpdatePicture(e.Image);
            GC.Collect();
        }

        private void OnThumbnailAvailable(object sender, GenerateDoneEventArgs e)
        {
            this.view.UpdatePicture(e.Image);
        }
    }
}
