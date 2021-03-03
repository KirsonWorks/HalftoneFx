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
                graphics.DrawCheckMark(markRect, this.Colors.CheckMark);
            }

            graphics.DrawBorderVolume(markRect, this.Colors.BorderVolume, this.Style.Rounding);
            base.DoRender(graphics);
        }
    }
}
