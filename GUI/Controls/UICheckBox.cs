namespace KWUI.Controls
{
    using KWUI.Helpers;
    using KWUI.BaseControls;

    using System.Drawing;

    public class UICheckBox : UIOptionButtonControl
    {
        protected override void DoRender(Graphics graphics)
        {
            var markRect = new RectangleF(this.ScreenPosition, this.GetCheckMarkSize());
            graphics.DrawFrame(markRect, this.GetBgColor(), this.Colors.Border, this.Style.Rounding);

            if (this.Checked)
            {
                graphics.DrawSolidShape(markRect.Inflate(-this.Style.InnerShrink), this.Colors.CheckMark, this.Shapes.CheckMark);
            }

            graphics.DrawBorderVolume(markRect, this.Colors.BorderVolume, this.Style.Rounding);
            base.DoRender(graphics);
        }
    }
}
