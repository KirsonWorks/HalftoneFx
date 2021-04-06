namespace HalftoneFx.UI
{
    using KWUI;
    using KWUI.Controls;
    
    using System.Drawing;

    public class UIColorPicker : UIPanel
    {
        public UIColorPicker()
            : base()
        {
            this.AutoSize = true;
        }

        public Color Color { get; set; }

        protected void BuildLayout()
        {
            /*
            var builder = new UILayoutBuilder(this);

            builder
                .Label("RED")
                .SliderInt(this.Color.R, 0, 255, 1)

                .Label("GREEN")
                .SliderInt(this.Color.G, 0, 255, 1)

                .Label("BLUE")
                .SliderInt(this.Color.B, 0, 255, 1)

                .Label("ALPHA")
                .SliderInt(this.Color.A, 0, 255, 1);
            */
        }
    }
}
