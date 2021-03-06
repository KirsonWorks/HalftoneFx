﻿namespace HalftoneFx.Views
{
    using KWUI;
    using KWUI.Controls;

    using HalftoneFx.Presenters;
    using HalftoneFx.UI;

    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public class HalftoneOptionsView : UILayoutContainer<UIWindow>, IView<WorkspacePresenter>
    {
        private UICheckBox checkBoxEnabled;

        private UISlider sliderGridType;

        private UISlider sliderShapeType;

        private UISlider sliderShapeSizeBy;

        private UISlider sliderCellSize;

        private UISlider sliderCellScale;

        private UIImage customPattern;

        private UIColorBox colorBoxForeground;

        private UIColorBox colorBoxBackground;

        public HalftoneOptionsView(UILayoutBuilder builder)
            : base(builder)
        {
            this.Container.Caption = "HALFTONE";
            this.Container.CustomColor("WindowCaption", Color.Gold);
            this.Container.FeatureOff(UIWindowFeatures.ClosingBox);
            this.Container.FeatureOn(UIWindowFeatures.ExpandingBox);
            this.Container.Show();
        }

        public WorkspacePresenter Presenter { get; set; }

        public void ValueForEnabled(bool value) =>
            this.checkBoxEnabled.Checked = value;

        public void ValueForGridType(int value) =>
            this.sliderGridType.Value = value;

        public void ValueForShapeType(int value) =>
            this.sliderShapeType.Value = value;

        public void ValueForCustomPattern(Image image) =>
            this.customPattern.Image = image ?? Properties.Resources.Imageholder;

        public void ValueForShapeSizeBy(int value) =>
            this.sliderShapeSizeBy.Value = value;

        public void ValueForCellSize(int value) =>
            this.sliderCellSize.Value = value;

        public void ValueForCellScale(float value) =>
            this.sliderCellScale.Value = value;

        public void ValueForFgColor(Color value) =>
            this.colorBoxForeground.Color = value;

        public void ValueForBgColor(Color value) =>
            this.colorBoxBackground.Color = value;

        public void SetUp()
        {
            this.ValueForEnabled(this.Presenter.HalftoneEnabled);
            this.ValueForGridType(this.Presenter.GridType);
            this.ValueForShapeType(this.Presenter.ShapeType);
            this.ValueForShapeSizeBy(this.Presenter.ShapeSizeBy);
            this.ValueForCellSize(this.Presenter.CellSize);
            this.ValueForCellScale(this.Presenter.CellScale);
            this.ValueForFgColor(this.Presenter.Foreground);
            this.ValueForBgColor(this.Presenter.Background);
        }

        protected override void BuildLayout(UILayoutBuilder builder)
        {
            builder
                .CheckBox("ENABLED")
                .Ref(ref checkBoxEnabled)
                .Hint("")
                .Changed(this.OnHalftoneEnabledChanged)

                .Label("GRID TYPE")
                .Slider(0, new[] { "Square", "Hexagon", "Checkerboard", "Lines", "Columns", "Noise" })
                .Ref(ref sliderGridType)
                .Changing(this.OnGridTypeChanging)

                .Label("SHAPE TYPE")
                .Slider(0, new[] { "Square", "Circle", "Dithering 4x4" })
                .Ref(ref sliderShapeType)
                .Changing(this.OnShapeTypeChanging)

                .Label("CUSTOM")
                .Button("LOAD")
                .Click(this.OnLoadPatternClick)

                .SameLine()
                .Button("CLEAR")
                .Click(this.OnClearPatternClick)

                .Image(90, 90, Properties.Resources.Imageholder, true)
                .Ref(ref customPattern)

                .Label("SIZE BY")
                .Slider(0, new[] { "None", "Brightness", "Brightness Inv", "Alpha" })
                .Ref(ref sliderShapeSizeBy)
                .Changing(this.OnShapeSizeByChanging)

                .Label("CELL SIZE")
                .SliderInt(4, 2, 64, 1)
                .Ref(ref sliderCellSize)
                .TextFormat("{0}px")
                .Changing(this.OnCellSizeChanging)

                .Label("CELL SCALE")
                .SliderFloat(1, 0.5f, 3.0f, 0.05f)
                .Ref(ref sliderCellScale)
                .Changing(this.OnCellScaleChanging)

                .Label("FOREGROUND")
                .Add<UIColorBox>()
                .Ref(ref colorBoxForeground)
                .Click(this.OnFgColorClick)

                .Label("BACKGROUND")
                .Add<UIColorBox>()
                .Ref(ref colorBoxBackground)
                .Click(this.OnBgColorClick);
        }
        private void OnHalftoneEnabledChanged(object sender, EventArgs e)
        {
            var @checked = this.checkBoxEnabled.Checked;
            this.Presenter.HalftoneEnabled = @checked;

            var dependents = new List<UIControl>();
            dependents.AddRange(this.Container.GetChildren<UILabel>());
            dependents.AddRange(this.Container.GetChildren<UISlider>());
            dependents.AddRange(this.Container.GetChildren<UIButton>());
            dependents.AddRange(this.Container.GetChildren<UIColorBox>());
            dependents.ForEach(control => control.Enabled = @checked);
        }

        private void OnGridTypeChanging(object sender, EventArgs e) =>
            this.Presenter.GridType = (int)this.sliderGridType.Value;

        private void OnShapeTypeChanging(object sender, EventArgs e) =>
            this.Presenter.ShapeType = (int)this.sliderShapeType.Value;

        private void OnShapeSizeByChanging(object sender, EventArgs e) =>
            this.Presenter.ShapeSizeBy = (int)this.sliderShapeSizeBy.Value;

        private void OnCellSizeChanging(object sender, EventArgs e) =>
            this.Presenter.CellSize = (int)this.sliderCellSize.Value;

        private void OnCellScaleChanging(object sender, EventArgs e) =>
            this.Presenter.CellScale = this.sliderCellScale.Value;

        private void OnLoadPatternClick(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = Properties.Resources.ImageFilesOpenFilter;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Presenter.LoadPatternFromFile(dialog.FileName);
                }
            }
        }

        private void OnClearPatternClick(object sender, EventArgs e)
        {
            this.Presenter.ClearPattern();
        }

        private void OnFgColorClick(object sender, EventArgs e)
        {
            if (this.Container.Root is UIManager manager)
            {
                var builder = manager.NewLayoutBuilder();
                var picker = new UIPopupColorPicker(builder, this.colorBoxForeground.Color);
                picker.Popup((e as UIMouseEventArgs).Location);

                picker.OnClose += (s, ev) =>
                {
                    this.colorBoxForeground.Color = picker.Color;
                    this.Presenter.Foreground = picker.Color;
                };
            }
        }

        private void OnBgColorClick(object sender, EventArgs e)
        {
            if (this.Container.Root is UIManager manager)
            {
                var builder = manager.NewLayoutBuilder();
                var picker = new UIPopupColorPicker(builder, this.colorBoxBackground.Color);
                picker.Popup((e as UIMouseEventArgs).Location);

                picker.OnClose += (s, ev) =>
                {
                    this.colorBoxBackground.Color = picker.Color;
                    this.Presenter.Background = picker.Color;
                };
            }
        }
    }
}
