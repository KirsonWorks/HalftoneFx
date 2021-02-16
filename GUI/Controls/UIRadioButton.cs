namespace GUI.Controls
{
    using GUI.Helpers;
    using GUI.BaseControls;

    using System;
    using System.Drawing;

    public class UIRadioButton : UIOptionButtonControl
    {
        protected override void DoRender(Graphics graphics)
        {
            var markSize = new RectangleF(this.ScreenPosition, this.GetCheckMarkSize());
            var rounding = (int)Math.Floor(markSize.Width / 2) - 1;

            graphics.DrawFrame(markSize, this.GetColor(), this.Style.Colors.Border, rounding);

            if (this.Checked)
            {
                graphics.DrawRect(markSize, this.Style.Colors.CheckMark, rounding, this.Style.InnerShrink);
            }

            graphics.DrawBorderVolume(markSize, this.Style.Colors.BorderVolume, rounding);

            base.DoRender(graphics);
        }
    }
}
