namespace HalftoneFx.Views
{
    using KWUI;
    using KWUI.BaseControls;
    using KWUI.Controls;

    using HalftoneFx.UI;
    using HalftoneFx.Helpers;
    using HalftoneFx.Presenters;
    
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public class PictureOptionsView : UILayoutContainer<UIWindow>, IView<WorkspacePresenter>
    {
        private UILabel labelSize;

        private UILabel labelZoom;

        private UICheckBox checkBoxSmoothing;

        private UICheckBox checkBoxNegative;

        private UISlider sliderBrightness;

        private UISlider sliderContrast;

        private UISlider sliderSaturation;

        private UISlider sliderQuantization;

        private UISlider sliderPalette;

        private UISlider sliderDitherMethod;

        private UISlider sliderDitherAmount;

        private UIProgressBar progress;

        public PictureOptionsView(UILayoutBuilder builder)
            : base(builder)
        {
            this.Container.Caption = "PICTURE";
            this.Container.CustomColor("WindowCaption", Color.Gold);
            this.Container.FeatureOff(UIWindowFeatures.ClosingBox);
            this.Container.FeatureOn(UIWindowFeatures.ExpandingBox);
            this.Container.Show();
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

        public void ValueForProgress(float value) =>
            this.progress.Value = value;

        public void ValueForSmoothing(bool value) =>
            this.checkBoxSmoothing.Checked = value;

        public void ValueForNegative(bool value) =>
            this.checkBoxNegative.Checked = value;

        public void ValueForBrightness(int value) =>
            this.sliderBrightness.Value = value;

        public void ValueForContrast(int value) =>
            this.sliderContrast.Value = value;

        public void ValueForSaturation(int value) =>
            this.sliderSaturation.Value = value;

        public void ValueForQuantization(int value) =>
            this.sliderQuantization.Value = value;

        public void ValueForPalette(int value)
        {
            this.sliderPalette.Value = value;
            this.sliderDitherMethod.Enabled = this.sliderDitherAmount.Enabled = value > 0;
        }

        public void ValueForDitherMethod(int value) =>
            this.sliderDitherMethod.Value = value;

        public void ValueForDitherAmount(int value) =>
            this.sliderDitherAmount.Value = value;

        public void LookupForPalette(IEnumerable<string> names)
        {
            var nameList = new List<string> { "None" };
            nameList.AddRange(names);
            this.sliderPalette.Lookup = nameList.ToArray();
        }

        public void SetUp()
        {
            this.sliderPalette.SetRange(this.Presenter.PaletteRange);
            this.sliderBrightness.SetRange(this.Presenter.BrightnessRange);
            this.sliderContrast.SetRange(this.Presenter.ContrastRange);
            this.sliderSaturation.SetRange(this.Presenter.SaturationRange);
            this.sliderQuantization.SetRange(this.Presenter.QuantizationRange);
            this.sliderDitherMethod.SetRange(this.Presenter.DitherMethodRange);
            this.sliderDitherAmount.SetRange(this.Presenter.DitherAmountRange);

            this.ValueForSmoothing(this.Presenter.Smoothing);
            this.ValueForNegative(this.Presenter.Negative);
            this.ValueForBrightness(this.Presenter.Brightness);
            this.ValueForContrast(this.Presenter.Contrast);
            this.ValueForSaturation(this.Presenter.Saturation);
            this.ValueForQuantization(this.Presenter.Quantization);
            this.ValueForDitherMethod(this.Presenter.DitherMethod);
            this.ValueForDitherAmount(this.Presenter.DitherAmount);

            this.ValueForPalette(this.Presenter.PaletteIndex);
            this.LookupForPalette(this.Presenter.Palettes.GetNames());
        }

        protected override void BuildLayout(UILayoutBuilder builder)
        {
            builder
                .Button("LOAD")
                .Click(this.OnLoadClick)

                .SameLine()
                .Button("SAVE")
                .Click(this.OnSaveClick)

                .CheckBox("SMOOTHING")
                .Ref(ref checkBoxSmoothing)
                .Changed(this.OnSmoothingChanged)

                .CheckBox("NEGATIVE")
                .Ref(ref checkBoxNegative)
                .Changed(this.OnNegativeChanged)

                .Label("BRIGHTNESS")
                .SliderInt(0, 0, 0, 1, flags: UIRangeTextFlags.PlusSign)
                .Ref(ref sliderBrightness)
                .Changing(this.OnBrightnessChanging)

                .Label("CONTRAST")
                .SliderInt(0, 0, 0, 1, flags: UIRangeTextFlags.PlusSign)
                .Ref(ref sliderContrast)
                .Changing(this.OnContrastChanging)

                .Label("SATURATION")
                .SliderInt(100, 0, 100, 1)
                .TextFormat("{0}%")
                .Ref(ref sliderSaturation)

                .Changing(this.OnSaturationChanging)

                .Label("QUANTIZATION")
                .SliderInt(0, 0, 0, 1)
                .Ref(ref sliderQuantization)
                .Changing(this.OnQuantizationChanging)

                .Label("PALETTE")
                .Slider(0, new[] { "None" })
                .Ref(ref sliderPalette)
                .Changing(this.OnPaletteChanging)

                .Label("DITHER METHOD")
                .Slider(0, new [] { "None", "Bayer 2x2", "Bayer 4x4", "Bayer 8x8" })
                .Ref(ref sliderDitherMethod)
                .Changing(this.OnDitherMethodChanging)

                .Label("DITHER AMOUNT")
                .SliderInt(0, 0, 100, 1)
                .Ref(ref sliderDitherAmount)
                .Changing(this.OnDitherAmountChanging)

                .Label("SIZE: 0x0")
                .Ref(ref labelSize)

                .Label("ZOOM: 100%")
                .Ref(ref labelZoom)

                .Progress(0.0f, 1.0f, 0.1f)
                .Ref(ref progress);
        }

        private void OnSmoothingChanged(object sender, EventArgs e) =>
            this.Presenter.Smoothing = this.checkBoxSmoothing.Checked;
        
        private void OnNegativeChanged(object sender, EventArgs e) =>
            this.Presenter.Negative = this.checkBoxNegative.Checked;

        private void OnBrightnessChanging(object sender, EventArgs e) => 
            this.Presenter.Brightness = (int)this.sliderBrightness.Value;

        private void OnContrastChanging(object sender, EventArgs e) =>
            this.Presenter.Contrast = (int)this.sliderContrast.Value;

        private void OnSaturationChanging(object sender, EventArgs e) =>
            this.Presenter.Saturation = (int)this.sliderSaturation.Value;

        private void OnQuantizationChanging(object sender, EventArgs e) =>
            this.Presenter.Quantization = (int)this.sliderQuantization.Value;
        private void OnPaletteChanging(object sender, EventArgs e)
        {
            var value = (int)this.sliderPalette.Value;
            this.sliderDitherMethod.Enabled = this.sliderDitherAmount.Enabled = value > 0;
            this.Presenter.PaletteIndex = value;
        }

        private void OnDitherMethodChanging(object sender, EventArgs e) => 
            this.Presenter.DitherMethod = (int)this.sliderDitherMethod.Value;

        private void OnDitherAmountChanging(object sender, EventArgs e) =>
            this.Presenter.DitherAmount = (int)this.sliderDitherAmount.Value;

        private void OnLoadClick(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = Properties.Resources.ImageFilesOpenFilter;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Presenter.LoadPictureFromFile(dialog.FileName);
                }
            }
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = Properties.Resources.ImageFilesSaveFilter;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Presenter.SavePictureToFile(dialog.FileName);
                }
            }
        }
    }
}
