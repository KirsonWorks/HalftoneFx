namespace HalftoneFx
{
    using GUI;
    using GUI.Controls;
    using GUI.Helpers;

    using Halftone;

    using HalftoneFx.UI;
    using HalftoneFx.Helpers;

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private readonly UIWinForms ui = new UIWinForms();

        private readonly UIPictureBox pictureBox = new UIPictureBox();

        private readonly HalftoneFacade halftone = new HalftoneFacade();

        private Image original, preview;

        public MainForm()
        {
            this.InitializeComponent();
            this.ui.Container = this;

            this.pictureBox.Parent = this.ui;
            this.pictureBox.Size = this.ClientSize;
            this.pictureBox.OnZoomChanged += PictureBoxZoomChanged;

            this.halftone.OnPropertyChanged += HalftoneOnPropertyChanged;
            this.halftone.OnImageAvailable += (s, e) => this.pictureBox.Image = e.Image;

            var builder = new UILayoutBuilder(this.ui, UILayoutStyle.Default);

            builder.BeginPanel(45, 45)
                   .Label("PICTURE").TextColor(Color.Gold)
                   .Button("LOAD").Click(this.LoadPictureFromFile).SameLine()
                   .Button("SAVE").Click(this.SavePicture)
                   .CheckBox("GRAYSCALE").Changed(this.GrayscaleChanged)
                   .CheckBox("NEGATIVE").Changed(this.NegativeChanged)
                   .Label("BRIGHTNESS").Stretch(90)
                   .SliderInt(0, -150, 100, 1).Changing(this.BrightnessChanging)
                   .Label("CONTRAST").Stretch(90)
                   .SliderInt(0, -50, 100, 1).Changing(this.ContrastChanging)
                   .Label("QUANTIZATION").Stretch(90)
                   .Slider(1, 1, 255, 1).Changing(this.QuantizationChanging)
                   .Label("SIZE: 0x0").Name("label-size")
                   .Label("ZOOM: 100%").Name("label-zoom").Click((s, e) => this.pictureBox.ResetZoom())
                   .EndPanel();
        }

        private void HalftoneOnPropertyChanged(object sender, EventArgs e)
        {
            this.pictureBox.Image = this.halftone.Generate((Bitmap)this.preview);
            this.halftone.GenerateAsync((Bitmap)this.original, 500);
        }

        private void PictureBoxZoomChanged(object sender, EventArgs e)
        {
            this.ui.Find<UILabel>("label-zoom").Caption = $"ZOOM: {this.pictureBox.Scale:P0}";
        }

        private void LoadPicture(Image picture)
        {
            this.original?.Dispose();
            this.original = picture;
            this.preview = picture.Preview(300);

            this.pictureBox.Image = new Bitmap(picture);
            this.pictureBox.FullView();
            this.ui.Reset(true);

            this.ui.Find<UILabel>("label-size").Caption = $"SIZE: {this.pictureBox.ImageSize.ToStringWxH()}";
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

        private void BrightnessChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.halftone.Brightness = (int)slider.Value;
        }

        private void ContrastChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.halftone.Contrast = (int)slider.Value;
        }

        private void QuantizationChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.halftone.Quantization = (int)slider.Value;
        }

        private void NegativeChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.halftone.Negative = checkbox.Checked;
        }

        private void GrayscaleChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.halftone.Grayscale = checkbox.Checked;
        }
    }
}
