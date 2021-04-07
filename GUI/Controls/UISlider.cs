namespace KWUI.Controls
{
    using KWUI.Helpers;
    using KWUI.BaseControls;
 
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
            var color = this.GetStateColor(
                this.Colors.Frame,
                this.Colors.FrameActive,
                this.Colors.FrameHovered,
                this.Colors.FrameDisabled);

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

            var grabColor = this.GetStateColor(
                this.Colors.SliderGrab,
                this.Colors.SliderGrabActive,
                this.Colors.SliderGrab,
                this.Colors.SliderGrabDisabled
                );

            graphics.DrawRect(grabRect, grabColor, this.Style.Rounding, this.Style.InnerShrink);

            base.DoRender(graphics); // Draw text.
            graphics.DrawBorderVolume(rect, this.Colors.BorderVolume, this.Style.Rounding);
        }

        private Color GetStateColor(Color normal, Color pressed, Color hovered, Color disabled)
        {
            if (this.Enabled)
            {
                if (this.IsPressed)
                {
                    return pressed;
                }
                
                if (this.IsHovered)
                {
                    return hovered;
                }
            }
            else
            {
                return disabled;
            }

            return normal;
        }
    }
}
