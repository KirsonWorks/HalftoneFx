namespace GUI.Controls
{
    using GUI.Helpers;
    using GUI.BaseControls;

    using System.Drawing;

    public class UIButton : UIButtonControl
    {
        private SizeF textSize = new SizeF(1, 1);

        public UIButton() : base()
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
                    this.UpdateMinimumSize();
                }
            }
        }

        protected Color GetBgColor()
        {
            if (this.Enabled)
            {
                if (this.Checked)
                {
                    return this.Colors.ButtonChecked;
                }
                else if (this.IsPressed)
                {
                    return this.Colors.ButtonActive;
                }
                else if (this.IsHovered)
                {
                    return this.Colors.ButtonHovered;
                }
            }
            else
            {
                return this.Colors.ButtonDisabled;
            }

            return this.Colors.Button;
        }

        protected override SizeF GetMinimumSize()
        {
            return this.textSize + new SizeF().OneValue(this.Style.Padding * 2);
        }

        protected override void DoRender(Graphics graphics)
        {
            graphics.DrawFrame(this.ScreenRect, this.GetBgColor(), this.Style.Colors.Border, this.Style.Rounding);
            graphics.DrawText(this.ScreenRect, this.Style.Fonts.Default, this.GetTextColor(), UIAlign.Center, false, true, this.Caption);
            graphics.DrawBorderVolume(this.ScreenRect, this.Style.Colors.BorderVolume, this.Style.Rounding);
        }
    }
}
