namespace HalftoneFx
{
    using GUI;
    using GUI.Controls;
    using GUI.BaseControls;

    using HalftoneFx.Editor;

    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using HalftoneFx.Helpers;

    public partial class MainForm : Form
    {
        private readonly UIWinForms ui = new UIWinForms();

        private readonly UIPictureBox pictureBox = new UIPictureBox();

        private readonly UIStatusBar statusBar = new UIStatusBar();

        private readonly HalftoneImage image = new HalftoneImage { ThumbnailSize = 300 };

        private readonly UILabel labelSize;

        private readonly UILabel labelZoom;

        private readonly UIProgressBar progress;

        public MainForm()
        {
            this.InitializeComponent();
            this.ui.Container = this;
            this.ui.OnNotification += this.OnUINotification;

            this.pictureBox.Name = "picture-box";
            this.pictureBox.Parent = this.ui;
            this.pictureBox.Size = this.ClientSize;
            this.pictureBox.OnZoomChanged += OnPictureBoxZoomChanged;

            this.statusBar = this.ui.NewStatusBar("status-bar");

            var builder = new UILayoutBuilder(this.ui, UILayoutStyle.Default);

            // Like a bullshit.
            builder.BeginPanel(20, 45)
                   .Label("PICTURE").TextColor(Color.Gold)
                   .Button("LOAD").Hint("Load picture from a file").Click(this.LoadPictureFromFile)
                   .SameLine()
                   .Button("SAVE").Hint("Save picture to a file").Click(this.SavePicture)
                   .CheckBox("SMOOTHING").Hint("On/Off Smoothing filter").Changed(this.OnSmoothingChanged)
                   .CheckBox("GRAYSCALE").Hint("On/Off Grayscale filter").Changed(this.OnGrayscaleChanged)
                   .CheckBox("NEGATIVE").Hint("On/Off Negative filter").Changed(this.OnNegativeChanged)
                   .Label("BRIGHTNESS")
                   .Wide(90)
                   .SliderInt(0, -150, 100, 1, UIRangeTextFlags.PlusSign).Hint("Brightness filter").Changing(this.OnBrightnessChanging)
                   .Label("CONTRAST")
                   .Wide(90)
                   .SliderInt(0, -50, 100, 1, UIRangeTextFlags.PlusSign).Hint("Contrast filter").Changing(this.OnContrastChanging)
                   .Label("QUANTIZATION")
                   .Wide(90)
                   .SliderInt(1, 1, 255, 1).Hint("Quantization filter").Changing(this.OnQuantizationChanging)
                   .Label("DOWNSAMPLING")
                   .Wide(90)
                   .SliderInt(1, 1, 16, 1).TextFormat("x{0}").Hint("Downsampling").Changing(this.OnDownsampleChanging)
                   .Label("SIZE: 0x0").Ref(ref labelSize)
                   .Label("ZOOM: 100%").Hint("Click for reset zoom or fit to screen").Ref(ref labelZoom)
                   .Click((s, e) => this.pictureBox.ResetZoom())
                   .Wide(90)
                   .Progress(0.0f, 1.0f, 0.1f).Ref(ref progress)
                   .EndPanel();

            builder.BeginPanel(140, 45)
                   .CheckBox("HALFTONE", this.image.HalftoneEnabled).TextColor(Color.Gold).Changed(this.OnHalftoneEnabledChanged)
                   .Label("GRID TYPE")
                   .Wide(90)
                   .Slider(0, 0, 2).Caption("Square").Changing(this.OnGridTypeChanging)
                   .Label("PATTERN")
                   .Wide(90)
                   .Slider(0, 0, 2).Caption("Square").Changing(this.OnPatternTypeChanging)
                   .Label("SIZE BY")
                   .Wide(90)
                   .Slider(0, 0, 3).Caption("None").Changing(this.OnShapeSizingChanging)
                   .Label("CELL SIZE")
                   .Wide(90)
                   .SliderInt(this.image.CellSize, 4, 64, 1).TextFormat("{0}px").Changing(this.OnCellSizeChanging)
                   .Label("CELL SCALE")
                   .Wide(90)
                   .SliderFloat(this.image.CellScale, 0.5f, 3.0f, 0.05f).Changing(this.OnCellScaleChanging)
                   .CheckBox("TRANSP. BG", this.image.TransparentBg).Changed(this.OnTransparentBgChanged)
                   .EndPanel();

            this.statusBar.BringToFront();

            this.image.OnImageAvailable += this.OnImageAvailable;
            this.image.OnThumbnailAvailable += this.OnThumbnailAvailable;

            this.image.OnProgress += (s, e) =>
            {
                this.progress.Value = e.Percent;
                this.Invalidate();
            };

            this.LoadPicture(Properties.Resources.Logo);
            this.pictureBox.Zoom(0.7f);
        }

        private void LoadPicture(Image picture)
        {
            // this.ui.Reset(true);

            this.image.Image = this.pictureBox.Image = picture;
            this.pictureBox.FitToScreen();

            this.labelSize.Caption = $"SIZE: {picture.Width}x{picture.Height}";
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
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.pictureBox.Image.SaveAs(this.saveFileDialog.FileName);
                }
                catch
                {
                    MessageBox.Show("Can't save the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void OnUINotification(object sender, UINotificationEventArgs e)
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

        private void OnImageAvailable(object sender, GenerateDoneEventArgs e)
        {
            this.pictureBox.Image = e.Image;
            this.statusBar.Caption = "Done.";
            this.Invalidate();
        }

        private void OnThumbnailAvailable(object sender, GenerateDoneEventArgs e)
        {
            this.pictureBox.Image = e.Image;
            this.Invalidate();
        }

        private void OnPictureBoxZoomChanged(object sender, EventArgs e)
        {
            this.labelZoom.Caption = $"ZOOM: {this.pictureBox.Scale:P0}";
        }

        private void OnBrightnessChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.Brightness = (int)slider.Value;
        }

        private void OnContrastChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.Contrast = (int)slider.Value;
        }

        private void OnQuantizationChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.Quantization = (int)slider.Value;
        }

        private void OnNegativeChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.Negative = checkbox.Checked;
        }

        private void OnGrayscaleChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.Grayscale = checkbox.Checked;
        }

        private void OnSmoothingChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.Smoothing = checkbox.Checked;
        }

        private void OnDownsampleChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.DownsamplingLevel = (int)slider.Value;
        }

        private void OnHalftoneEnabledChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.HalftoneEnabled = checkbox.Checked;
        }

        private void OnCellSizeChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.CellSize = (int)slider.Value;
        }

        private void OnCellScaleChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.CellScale = slider.Value;
        }

        private void OnGridTypeChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            var types = new string[3] { "Square", "Hexagon", "Noise" };
            var value = (int)slider.Value;
            slider.Caption = types[value];
            this.image.GridType = value;
        }

        private void OnPatternTypeChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            var types = new string[3] { "Square", "Circle", "Dithering4x4" };
            var value = (int)slider.Value;
            slider.Caption = types[value];
            this.image.PatternType = value;
        }

        private void OnShapeSizingChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            var types = new string[4] { "None", "Brightness", "Brightness Inv", "Alpha Channel" };
            var value = (int)slider.Value;
            slider.Caption = types[value];
            this.image.ShapeSizing = value;
        }

        private void OnTransparentBgChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.TransparentBg = checkbox.Checked;
        }
    }
}
