﻿namespace HalftoneFx
{
    using GUI;
    using GUI.Controls;

    using Halftone;
    using HalftoneFx.UI;
    using HalftoneFx.Helpers;

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private readonly HalftoneFacade halftone = new HalftoneFacade();

        private readonly UIWinForms ui = new UIWinForms();

        private readonly UIPictureBox pictureBox = new UIPictureBox();

        private readonly UILabel labelSize;

        private readonly UILabel labelZoom;

        private readonly UIProgressBar progress;

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
            this.halftone.OnProgressChanged += (s, e) => { this.progress.Value = e.Percent; this.Invalidate(); };

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
                   .Label("SIZE: 0x0").Ref(ref labelSize)
                   .Label("ZOOM: 100%").Ref(ref labelZoom).Click((s, e) => this.pictureBox.ResetZoom()).Stretch(90)
                   .Progress(0.0f, 1.0f, 0.1f).Ref(ref progress)
                   .EndPanel();
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
        private void HalftoneOnPropertyChanged(object sender, EventArgs e)
        {
            this.pictureBox.Image = this.halftone.Generate((Bitmap)this.preview);
            this.halftone.GenerateAsync((Bitmap)this.original, 500);
        }

        private void PictureBoxZoomChanged(object sender, EventArgs e)
        {
            this.labelZoom.Caption = $"ZOOM: {this.pictureBox.Scale:P0}";
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
