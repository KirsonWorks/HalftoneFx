namespace HalftoneFx.Views
{
    using GUI;
    using GUI.BaseControls;
    using GUI.Controls;

    using HalftoneFx.Presenters;
    using HalftoneFx.UI;

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class PictureOptionsView : UILayoutPanel, IView<WorkspacePresenter>
    {
        private UILabel labelSize;

        private UILabel labelZoom;

        private UICheckBox checkBoxSmoothing;

        private UICheckBox checkBoxGrayscale;

        private UICheckBox checkBoxNegative;

        private UISlider sliderBrightness;

        private UISlider sliderContrast;

        private UISlider sliderQuantization;

        private UISlider sliderDithering;

        private UIProgressBar progress;

        public PictureOptionsView(UILayoutBuilder builder)
            : base(builder)
        {
        }

        public WorkspacePresenter Presenter { get; set; }
        
        public void ValueForSize(SizeF value)
        {
            this.labelSize.Caption = $"SIZE: {value.Width}x{value.Height}";
        }

        public void ValueForZoom(float value)
        {
            this.labelZoom.Caption = $"ZOOM: {value:P0}";
        }

        public void ValueForProgress(float value) => this.progress.Value = value;

        public void ValueForSmoothing(bool value) => this.checkBoxSmoothing.Checked = value;

        public void ValueForGrayscale(bool value) => this.checkBoxGrayscale.Checked = value;

        public void ValueForNegative(bool value) => this.checkBoxNegative.Checked = value;

        public void ValueForBrightness(int value) => this.sliderBrightness.Value = value;

        public void ValueForContrast(int value) => this.sliderContrast.Value = value;

        public void ValueForQuantization(int value) => this.sliderQuantization.Value = value;

        public void ValueForDithering(int value) => this.sliderDithering.Value = value;

        public void SetUp()
        {
            this.sliderBrightness.Min = this.Presenter.BrightnessMin;
            this.sliderBrightness.Max = this.Presenter.BrightnessMax;
            this.sliderContrast.Min = this.Presenter.ContrastMin;
            this.sliderContrast.Max = this.Presenter.ContrastMax;
            this.sliderQuantization.Min = this.Presenter.QuantizationMin;
            this.sliderQuantization.Max = this.Presenter.QuantizationMax;
            this.sliderDithering.Min = this.Presenter.DitheringMin;
            this.sliderDithering.Max = this.Presenter.DitheringMax;

            this.ValueForSmoothing(this.Presenter.Smoothing);
            this.ValueForGrayscale(this.Presenter.Grayscale);
            this.ValueForNegative(this.Presenter.Negative);
            this.ValueForBrightness(this.Presenter.Brightness);
            this.ValueForContrast(this.Presenter.Contrast);
            this.ValueForQuantization(this.Presenter.Quantization);
            this.ValueForDithering(this.Presenter.Dithering);
        }

        protected override void BuildLayout(UILayoutBuilder builder)
        {
            builder
                .Label("PICTURE")
                .TextColor(Color.Gold)

                .Button("LOAD")
                .Hint("Load picture from a file")
                .Click(this.OnLoadClick)

                .SameLine()
                .Button("SAVE")
                .Hint("Save picture to a file")
                .Click(this.OnSaveClick)

                .CheckBox("SMOOTHING").Ref(ref checkBoxSmoothing)
                .Hint("On/Off Smoothing filter")
                .Changed(this.OnSmoothingChanged)

                .CheckBox("GRAYSCALE").Ref(ref checkBoxGrayscale)
                .Hint("On/Off Grayscale filter")
                .Changed(this.OnGrayscaleChanged)

                .CheckBox("NEGATIVE").Ref(ref checkBoxNegative)
                .Hint("On/Off Negative filter")
                .Changed(this.OnNegativeChanged)

                .Label("BRIGHTNESS")
                .SliderInt(0, 0, 0, 1, UIRangeTextFlags.PlusSign).Ref(ref sliderBrightness)
                .Hint("Brightness filter")
                .Changing(this.OnBrightnessChanging)

                .Label("CONTRAST")
                .SliderInt(0, 0, 0, 1, UIRangeTextFlags.PlusSign).Ref(ref sliderContrast)
                .Hint("Contrast filter")
                .Changing(this.OnContrastChanging)

                .Label("QUANTIZATION")
                .SliderInt(0, 0, 0, 1).Ref(ref sliderQuantization)
                .Hint("Quantization filter")
                .Changing(this.OnQuantizationChanging)

                .Label("DITHERING")
                .Slider(0, 0, 0).Ref(ref sliderDithering)
                .Caption("None")
                .Hint("Dithering effect")
                .Changing(this.OnDitheringChanging)

                .Label("SIZE: 0x0").Ref(ref labelSize)
                .Label("ZOOM: 100%").Ref(ref labelZoom)
                .Progress(0.0f, 1.0f, 0.1f).Ref(ref progress);
        }

        private void OnSmoothingChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.Presenter.Smoothing = checkbox.Checked;
        }

        private void OnGrayscaleChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.Presenter.Grayscale = checkbox.Checked;
        }
        
        private void OnNegativeChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.Presenter.Negative = checkbox.Checked;
        }

        private void OnBrightnessChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.Presenter.Brightness = (int)slider.Value;
        }

        private void OnContrastChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.Presenter.Contrast = (int)slider.Value;
        }

        private void OnQuantizationChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.Presenter.Quantization = (int)slider.Value;
        }

        private void OnDitheringChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            var value = (int)slider.Value;
            var dimension = 1 << value;
            slider.Caption = dimension > 1 ? $"{dimension}x{dimension}" : "None";
            this.Presenter.Dithering = value;
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Presenter.SavePictureToFile(saveDialog.FileName);
                }
            }
        }

        private void OnLoadClick(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Presenter.LoadPictureFromFile(openDialog.FileName);
                }
            }
        }
    }
}
