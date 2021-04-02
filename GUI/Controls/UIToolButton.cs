namespace KWUI.Controls
{
    using KWUI.BaseControls;
    using KWUI.Helpers;

    using System.Drawing;

    public class UIToolButton : UIButtonControl
    {
        public UIToolButton()
            : base()
        {
            this.SetSize(24, 24);
        }

        public Image Icon { get; set; }

        public PointF[] Shape { get; set; }

        protected override void DoRender(Graphics graphics)
        {
            var rect = this.ScreenRect;
            var color = this.GetStateColor(
                Color.Empty,
                this.Colors.ButtonActive,
                this.Colors.ButtonHovered,
                this.Colors.ButtonChecked,
                this.Colors.ButtonDisabled);

            graphics.DrawFrame(rect, color, Color.Empty, this.Style.Rounding);

            rect = rect.Inflate(-this.Style.InnerShrink);
            
            if (this.Icon != null)
            {
                graphics.DrawImage(this.Icon, rect);
            }

            if (this.Shape != null)
            {
                graphics.DrawSolidShape(rect, this.GetFgColor(), this.Shape);
            }
        }
    }
}
