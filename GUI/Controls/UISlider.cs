namespace GUI.Controls
{
    using GUI.Helpers;
    using GUI.BaseControls;
 
    using System.Drawing;

    public class UISlider : UIRangeControl
    {
        public UISlider() : base()
        {
            this.Vertical = false;
            this.TextType = UIRangeTextType.Value;
        }

        protected override void DoRender(Graphics graphics)
        {
            var color = this.Colors.Frame;

            if (this.Enabled)
            {
                if (this.IsPressed)
                {
                    color = this.Colors.FrameActive;
                }
                else if (this.IsHovered)
                {
                    color = this.Colors.FrameHovered;
                }
            }
            else
            {
                color = this.Colors.FrameDisabled;
            }

            var rect = this.ScreenRect;
            graphics.DrawFrame(rect, color, this.Colors.Border, this.Style.Rounding);

            var grabRect = RectangleF.Empty;
            var grabHalfSize = this.Style.SliderGrabSize / 2;
            var grabX = (rect.Right - grabHalfSize) - (rect.X + grabHalfSize);
            var grabY = (rect.Bottom - grabHalfSize) - (rect.Y + grabHalfSize);

            if (this.Vertical)
            {
                grabRect = new RectangleF(rect.X, rect.Y + grabY * this.Percent, rect.Width, this.Style.SliderGrabSize);
            }
            else
            {
                grabRect = new RectangleF(rect.X + grabX * this.Percent, rect.Y, this.Style.SliderGrabSize, rect.Height);
            }

            var grabColor = this.IsPressed ? this.Colors.SliderGrabActive : this.Colors.SliderGrab;
            graphics.DrawRect(grabRect, grabColor, this.Style.Rounding, this.Style.InnerShrink);

            base.DoRender(graphics); // Draw text.
            graphics.DrawBorderVolume(rect, this.Colors.BorderVolume, this.Style.Rounding);
        }
    }
}
