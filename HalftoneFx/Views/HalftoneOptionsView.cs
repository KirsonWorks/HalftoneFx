namespace HalftoneFx.Views
{
    using GUI;
    using GUI.Controls;

    using HalftoneFx.Presenters;
    using HalftoneFx.UI;

    using System;
    using System.Drawing;

    public class HalftoneOptionsView : UILayoutPanel, IView<WorkspacePresenter>
    {
        private UIImage customPattern;

        public HalftoneOptionsView(UILayoutBuilder builder)
            : base(builder)
        {
        }

        public WorkspacePresenter Presenter { get; set; }

        public void SetUp()
        {
            
        }

        protected override void BuildLayout(UILayoutBuilder builder)
        {
            builder
                .CheckBox("HALFTONE", true)
                .TextColor(Color.Gold)
                .Hint("")
                .Changed(this.OnHalftoneEnabledChanged)

                .Label("GRID TYPE")
                .Slider(0, 0, 1/*(int)HalftoneGridType.Max - 1*/)
                .Caption("Square")
                .Changing(this.OnGridTypeChanging)

                .Label("SHAPE TYPE")
                .Slider(0, 0, 1/*(int)HalftoneShapeType.Max - 1*/)
                .Caption("Square")
                .Changing(this.OnShapeTypeChanging)

                .Label("CUSTOM")
                .Button("LOAD")
                .Click(this.OnLoadPatternClick)

                .SameLine()
                .Button("CLEAR")
                .Click(this.OnClearPatternClick)

                .Image(90, 90, Properties.Resources.Imageholder, true).Ref(ref customPattern)

                .Label("SIZE BY")
                .Slider(0, 0, 1/*(int)HalftoneShapeSizing.Max - 1*/)
                .Caption("None")
                .Changing(this.OnShapeSizingChanging)

                .Label("CELL SIZE")
                .SliderInt(8/*this.image.CellSize*/, 2, 64, 1)
                .TextFormat("{0}px")
                .Changing(this.OnCellSizeChanging)

                .Label("CELL SCALE")
                .SliderFloat(/*this.image.CellScale*/1, 0.5f, 3.0f, 0.05f)
                .Changing(this.OnCellScaleChanging)

                .Label("FOREGROUND")
                .Add<UIColorBox>()

                .Label("BACKGROUND")
                .Add<UIColorBox>();
        }

        private void OnCellScaleChanging(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCellSizeChanging(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnShapeSizingChanging(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnClearPatternClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnLoadPatternClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnShapeTypeChanging(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnGridTypeChanging(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnHalftoneEnabledChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
