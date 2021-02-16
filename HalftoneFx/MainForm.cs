namespace HalftoneFx
{
    using GUI;
    using GUI.Controls;
    using GUI.Helpers;
    using HalftoneFx.GFX;
    using HalftoneFx.Helpers;
    using HalftoneFx.UI;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public static class Filter
    {
        public const int Grayscale = 1;

        public const int Negative = 2;

        public const int Brightness = 3;

        public const int Contrast = 4;

        public const int Quantization = 5;
    }

    public partial class MainForm : Form
    {
        private readonly UIWinForms ui = new UIWinForms();

        private readonly UIPictureBox pictureBox = new UIPictureBox();

        private readonly ImageFilterComplex filter = new ImageFilterComplex();

        private readonly ImageHalftone halftone = new ImageHalftone();

        private Image original;

        private Image preview;

        public MainForm()
        {
            this.InitializeComponent();
            this.ui.Container = this;

            this.pictureBox.Parent = this.ui;
            this.pictureBox.Size = this.ClientSize;
            this.pictureBox.OnZoomChanged += PictureBoxZoomChanged;

            this.filter.Add(Filter.Grayscale, new ImageFilterGrayscale());
            this.filter.Add(Filter.Negative, new ImageFilterNegative());
            this.filter.Add(Filter.Brightness, new ImageFilterBrightness());
            this.filter.Add(Filter.Contrast, new ImageFilterContrast());
            this.filter.Add(Filter.Quantization, new ImageFilterQuantization());
            this.filter.OnValueChanged += this.FilterValueChanged;

            var builder = new UILayoutBuilder(this.ui, UILayoutStyle.Default);

            builder.BeginPanel(45, 45)
                   .Label("PICTURE").TextColor(Color.Gold)
                   .Button("LOAD").Click(this.LoadPictureFromFile).SameLine()
                   .Button("SAVE").Click(this.SavePicture)
                   .CheckBox("GRAYSCALE").Changed(this.GrayscaleChanged)
                   .CheckBox("NEGATIVE").Changed(this.NegativeChanged)
                   .Label("BRIGHTNESS").Stretch(90)
                   .SliderInt(0, -150, 150, 1).Changing(this.BrightChanging).Changed(this.FilterChanged)
                   .Label("CONTRAST").Stretch(90)
                   .SliderInt(0, -50, 100, 1).Changing(this.ContrastChanging).Changed(this.FilterChanged)
                   .Label("QUANTIZATION").Stretch(90)
                   .Slider(1, 1, 255, 1).Changing(this.QuantizationChanging).Changed(this.FilterChanged)
                   .Label("SIZE: 0x0").Name("label-size")
                   .Label("ZOOM: 100%").Name("label-zoom").Click((s, e) => this.pictureBox.ResetZoom())
                   .EndPanel();
        }

        private void PictureBoxZoomChanged(object sender, EventArgs e)
        {
            this.ui.Find<UILabel>("label-zoom").Caption = $"ZOOM: {this.pictureBox.Scale:P0}";
        }

        private void LoadPicture(Bitmap picture)
        {
            var pic = new Bitmap(picture);
            this.preview = pic.Preview(250);
            this.pictureBox.Image = pic;
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
                    if (this.original != null)
                    {
                        this.original.Dispose();
                    }

                    this.original = Image.FromFile(this.openPictureDialog.FileName);
                    this.LoadPicture((Bitmap)this.original);
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

        private void BrightChanging(object sender, EventArgs e)
        {
            if (sender is UISlider slider)
            {
                this.filter[Filter.Brightness] = (int)slider.Value;
            }
        }

        private void ContrastChanging(object sender, EventArgs e)
        {
            if (sender is UISlider slider)
            {
                this.filter[Filter.Contrast] = (int)slider.Value;
            }
        }

        private void QuantizationChanging(object sender, EventArgs e)
        {
            if (sender is UISlider slider)
            {
                this.filter[Filter.Quantization] = (int)slider.Value;
            }
        }

        private void NegativeChanged(object sender, EventArgs e)
        {
            if (sender is UICheckBox checkbox)
            {
                this.filter[Filter.Negative] = Convert.ToInt32(checkbox.Checked);
                this.FilterChanged(sender, e);
            }
        }

        private void GrayscaleChanged(object sender, EventArgs e)
        {
            if (sender is UICheckBox checkBox)
            {
                this.filter[Filter.Grayscale] = Convert.ToInt32(checkBox.Checked);
                this.FilterChanged(sender, e);
            }
        }

        private void FilterValueChanged(object sender, EventArgs e)
        {
            this.pictureBox.Image = ImageFilterPass.GetFiltered((Bitmap)this.preview, this.filter);
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            this.pictureBox.Image = ImageFilterPass.GetFiltered((Bitmap)this.original, this.filter);
        }
    }
}
