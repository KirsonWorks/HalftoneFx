namespace KWUI.Controls
{
    using KWUI.Helpers;

    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIPanel : UIControl
    {
        public UIPanel()
            : base()
        {
            this.SetSize(100, 100);
        }

        protected override GraphicsPath GetClipPath(Graphics graphics, RectangleF rect)
        {
            return graphics.GetClipPath(rect, this.Style.Rounding);
        }
        
        protected override void DoRender(Graphics graphics)
        {
            graphics.DrawFrame(this.ScreenRect, this.Colors.Container, this.Colors.Border, this.Style.Rounding);
            graphics.DrawBorderVolume(this.ScreenRect, this.Colors.BorderVolume, this.Style.Rounding);
        }
    }
}
