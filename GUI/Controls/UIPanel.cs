﻿namespace GUI.Controls
{
    using GUI.Helpers;

    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIPanel : UIControl
    {
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
