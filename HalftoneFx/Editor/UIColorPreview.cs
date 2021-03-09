namespace HalftoneFx.Editor
{
    using GUI;
    using GUI.Helpers;

    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIColorPreview : UIControl
    {
        private static readonly int checkerSize = 4;

        private static Image checkeredTexture = null;

        public UIColorPreview()
            : base()
        {
            if (checkeredTexture == null)
            {
                checkeredTexture = new Bitmap(checkerSize * 2, checkerSize * 2);
                checkeredTexture.GenerateCheckerTexture(checkerSize, Color.White, Color.Gray);
            }
        }

        public Color Color { get; set; } = Color.Empty;

        protected override void DoRender(Graphics graphics)
        {
            var rect = this.ScreenRect;
            var radius = this.Style.Rounding;

            if (this.Color.IsEmpty)
            {
                graphics.DrawRect(rect, Color.White, radius);

                using (var pen = new Pen(Color.Red, 2.0f))
                {
                    var r = rect.Inflate(-(float)radius / 4);
                    graphics.DrawLine(pen, r.Right, r.Y, r.X, r.Bottom);
                }
            }
            else
            {
                var rgb = Color.FromArgb(255, this.Color);

                using (var background = new TextureBrush(checkeredTexture))
                using (var gradient = new LinearGradientBrush(Rectangle.Round(rect), rgb, this.Color, LinearGradientMode.Vertical))
                {
                    graphics.DrawRect(rect, background, radius);
                    graphics.DrawRect(rect, gradient, radius);
                }
            }

            graphics.DrawBorder(rect, this.Style.Colors.Border, radius);
            graphics.DrawBorderVolume(rect, this.Style.Colors.BorderVolume, radius);
        }
    }
}
