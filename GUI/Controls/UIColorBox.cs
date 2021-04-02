namespace KWUI.Controls
{
    using KWUI.BaseControls;
    using KWUI.Helpers;

    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIColorBox : UIButtonControl
    {
        private static readonly int checkerSize = 4;

        private static Image checkeredTexture = null;

        public UIColorBox()
            : base()
        {
            if (checkeredTexture == null)
            {
                checkeredTexture = new Bitmap(checkerSize * 2, checkerSize * 2);
                checkeredTexture.GenerateCheckerTexture(checkerSize, Color.White, Color.Silver);
            }
        }

        public Color Color { get; set; } = Color.Empty;

        protected override void DoRender(Graphics graphics)
        {
            var rect = this.ScreenRect;
            var radius = this.Style.Rounding;

            using (var background = new TextureBrush(checkeredTexture))
            {
                graphics.DrawRect(rect, background, radius);
            }

            if (!Color.IsEmpty)
            {
                var r = Rectangle.Round(rect);
                var rgb = Color.FromArgb(255, this.Color);

                using (var gradient = new LinearGradientBrush(r, rgb, this.Color, LinearGradientMode.Vertical))
                {
                    graphics.DrawRect(rect, gradient, radius);
                }
            }

            var stateColor = this.GetStateColor();

            if (!stateColor.IsEmpty && stateColor.A > 0)
            {
                graphics.DrawRect(rect, Color.FromArgb(128, stateColor), radius);
            }

            graphics.DrawBorder(rect, this.Style.Colors.Border, radius);
            graphics.DrawBorderVolume(rect, this.Style.Colors.BorderVolume, radius);
        }

        private Color GetStateColor()
        {
            if (this.Enabled)
            {
                if (this.IsPressed)
                {
                    return this.Colors.FrameActive;
                }
                else if (this.IsHovered)
                {
                    return this.Colors.FrameHovered;
                }
            }
            else
            {
                return this.Colors.FrameDisabled;
            }

            return Color.Empty;
        }
    }
}
