namespace KWUI.Controls
{
    using KWUI.Helpers;
    using KWUI.BaseControls;

    using System.Drawing;

    public class UIButton : UIButtonControl
    {
        private SizeF textSize = new SizeF(1, 1);

        public UIButton()
            : base()
        {
            this.Size = new SizeF(60, 24);
            this.AutoSize = true;
        }

        public override string Caption
        { 
            get => base.Caption;
            
            set
            {
                if (base.Caption != value)
                {
                    base.Caption = value;
                    this.textSize = GraphicsHelper.StringSize(value, this.Style.Fonts.Default);
                    this.UpdatePreferredSize();
                }
            }
        }

        public bool Flat { get; set; }

        protected override SizeF GetPreferredSize()
        {
            return this.textSize + new SizeF().OneValue(this.Style.Padding * 2);
        }


        protected override void DoRender(Graphics graphics)
        {
            var rect = this.ScreenRect;
            var color = this.GetStateColor(
                this.Flat ? Color.Transparent : this.Colors.Button,
                this.Colors.ButtonActive,
                this.Colors.ButtonHovered,
                this.Colors.ButtonChecked,
                this.Colors.ButtonDisabled);

            graphics.DrawFrame(rect, color, this.Flat ? Color.Transparent : this.Style.Colors.Border, this.Style.Rounding);
            graphics.DrawText(rect, this.Style.Fonts.Default, this.GetFgColor(), UIAlign.Center, false, true, this.Caption);

            if (!this.Flat)
            {
                graphics.DrawBorderVolume(rect, this.Style.Colors.BorderVolume, this.Style.Rounding);
            }
        }
    }
}
