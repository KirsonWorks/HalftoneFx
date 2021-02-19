namespace HalftoneFx
{
    using GUI;
    using GUI.Controls;

    using HalftoneFx.Editor;
    using HalftoneFx.Helpers;

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private readonly UIWinForms ui = new UIWinForms();

        private readonly UIPictureBox pictureBox = new UIPictureBox();

        private readonly UIStatusBar statusBar = new UIStatusBar();

        private readonly HalftoneGenerator generator = new HalftoneGenerator();

        private readonly UILabel labelSize;

        private readonly UILabel labelZoom;

        private readonly UIProgressBar progress;

        private Image original, preview;

        public MainForm()
        {
            this.InitializeComponent();
            this.ui.Container = this;
            this.ui.OnNotification += this.OnNotification;

            this.pictureBox.Name = "picture-box";
            this.pictureBox.Parent = this.ui;
            this.pictureBox.Size = this.ClientSize;
            this.pictureBox.OnZoomChanged += PictureBoxZoomChanged;

            this.statusBar = this.ui.NewStatusBar("status-bar");

            this.generator.OnPropertyChanged += OnGeneratorPropertyChanged;
            this.generator.OnImageAvailable += (s, e) => this.pictureBox.Image = e.Image;
            this.generator.OnProgressChanged += (s, e) =>
            { 
                this.progress.Value = e.Percent; 
                this.Invalidate();
            };

            var builder = new UILayoutBuilder(this.ui, UILayoutStyle.Default);

            // Like a bullshit.
            builder.BeginPanel(45, 45)
                   .Label("PICTURE").TextColor(Color.Gold)
                   .Button("LOAD").Hint("Load picture from a file").Click(this.LoadPictureFromFile)
                   .SameLine()
                   .Button("SAVE").Hint("Save picture to a file").Click(this.SavePicture)
                   .CheckBox("GRAYSCALE").Hint("Enable/Disable Grayscale filter").Changed(this.GrayscaleChanged)
                   .CheckBox("NEGATIVE").Hint("Enable/Disable Negative filter").Changed(this.NegativeChanged)
                   .Label("BRIGHTNESS")
                   .Wide(90)
                   .SliderInt(0, -150, 100, 1).Hint("Brightness filter").Changing(this.BrightnessChanging)
                   .Label("CONTRAST")
                   .Wide(90)
                   .SliderInt(0, -50, 100, 1).Hint("Contrast filter").Changing(this.ContrastChanging)
                   .Label("QUANTIZATION")
                   .Wide(90)
                   .Slider(1, 1, 255, 1).Hint("Quantization filter").Changing(this.QuantizationChanging)
                   .Label("SIZE: 0x0").Ref(ref labelSize)
                   .Label("ZOOM: 100%").Hint("Click for reset zoom or fit to screen").Ref(ref labelZoom)
                   .Click((s, e) => this.pictureBox.ResetZoom())
                   .Wide(90)
                   .Progress(0.0f, 1.0f, 0.1f).Ref(ref progress)
                   .EndPanel();

            builder.BeginPanel(45, 360)
                   .Label("HALFTONE").TextColor(Color.Gold)
                   .Label("SIZE").Stretch(90)
                   .SliderInt(200, 25, 300, 1).Changing(this.HalftoneSizeChanging)
                   .EndPanel();

            this.LoadPicture(Properties.Resources.Logo);
            this.pictureBox.Zoom(0.7f);
        }

        private void LoadPicture(Image picture)
        {
            this.original?.Dispose();
            this.original = picture;
            this.preview = picture.Preview(300);

            this.pictureBox.Image = new Bitmap(picture);
            this.pictureBox.FullView();

            this.labelSize.Caption = $"SIZE: {picture.Width}x{picture.Height}";
            this.ui.Reset(true);
        }

        private void LoadPictureFromFile(object sender, EventArgs e)
        {
            if (this.openPictureDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var image = Image.FromFile(this.openPictureDialog.FileName);
                    this.LoadPicture(image);
                }
                catch
                {
                    MessageBox.Show("Can't load the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void SavePicture(object sender, EventArgs e)
        {
            
        }

        private void OnNotification(object sender, UINotificationEventArgs e)
        {
            switch (e.What)
            {
                case UINotification.MouseOver:
                    if (sender is UIControl control)
                    {
                        this.statusBar.Caption = control.HintText;
                    }

                    break;

                case UINotification.MouseOut:
                    this.statusBar.Caption = string.Empty;
                    break;
            }
        }

        private void OnGeneratorPropertyChanged(object sender, EventArgs e)
        {
            this.pictureBox.Image = this.generator.Generate((Bitmap)this.preview, true);
            this.generator.GenerateAsync((Bitmap)this.original, 500);
        }

        private void PictureBoxZoomChanged(object sender, EventArgs e)
        {
            this.labelZoom.Caption = $"ZOOM: {this.pictureBox.Scale:P0}";
        }

        private void BrightnessChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.generator.Brightness = (int)slider.Value;
        }

        private void ContrastChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.generator.Contrast = (int)slider.Value;
        }

        private void QuantizationChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.generator.Quantization = (int)slider.Value;
        }

        private void NegativeChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.generator.Negative = checkbox.Checked;
        }

        private void GrayscaleChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.generator.Grayscale = checkbox.Checked;
        }

        private void HalftoneSizeChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.generator.HalftoneSize = (int)slider.Value;
        }
    }
}
