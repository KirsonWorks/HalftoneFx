namespace GUI.Controls
{
    using GUI.Helpers;
    using GUI.BaseControls;

    using System.Drawing;

    public class UICheckBox : UIOptionButtonControl
    {
        protected override void DoRender(Graphics graphics)
        {
            var markRect = new RectangleF(this.ScreenPosition, this.GetCheckMarkSize());
            graphics.DrawFrame(markRect, this.GetColor(), this.Colors.Border, this.Style.Rounding);

            if (this.Checked)
            {
                graphics.DrawRect(markRect, this.Colors.CheckMark, this.Style.Rounding, this.Style.InnerShrink);
            }

            graphics.DrawBorderVolume(markRect, this.Colors.BorderVolume, this.Style.Rounding);
            base.DoRender(graphics);
        }
    }
}
