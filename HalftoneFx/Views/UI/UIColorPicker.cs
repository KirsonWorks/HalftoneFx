namespace HalftoneFx.UI
{
    using KWUI;
    using KWUI.Controls;
    
    using System.Drawing;

    public class UIColorPicker<T> : UILayoutContainer<T>
        where T: UIControl
    {
        private Color backup = Color.Empty;

        private UIColorBox colorBox;

        private UISlider sliderR;

        private UISlider sliderG;

        private UISlider sliderB;

        private UISlider sliderA;

        public UIColorPicker(UILayoutBuilder builder)
            : base(builder)
        {
        }

        public Color Color
        { 
            get => this.colorBox.Color;

            set
            {
                if (this.backup.IsEmpty)
                {
                    this.backup = value;
                }

                this.sliderR.Value = value.R;
                this.sliderG.Value = value.G;
                this.sliderB.Value = value.B;
                this.sliderA.Value = value.A;
                this.colorBox.Color = value;
            }
        }

        protected override void BuildLayout(UILayoutBuilder builder)
        {
            builder
                .ColorBox()
                .Ref(ref colorBox)

                .SliderInt(0, 255, 1)
                .Ref(ref sliderR)
                .Changing((s, e) => this.colorBox.R = (byte)sliderR.Value)
                .SameLine()
                .Label("R")

                .SliderInt(0, 255, 1)
                .Ref(ref sliderG)
                .Changing((s, e) => this.colorBox.G = (byte)sliderG.Value)
                .SameLine()
                .Label("G")

                .SliderInt(0, 255, 1)
                .Ref(ref sliderB)
                .Changing((s, e) => this.colorBox.B = (byte)sliderB.Value)
                .SameLine()
                .Label("B")

                .SliderInt(0, 255, 1)
                .Ref(ref sliderA)
                .Changing((s, e) => this.colorBox.A = (byte)sliderA.Value)
                .SameLine()
                .Label("A")

                .Button("APPLY")
                .Click((s, e) => this.Container.Hide())
                .SameLine()
                .Button("CANCEL")
                .Click((s, e) =>
                {
                    this.Color = this.backup;
                    this.Container.Hide();
                });
        }
    }
}
