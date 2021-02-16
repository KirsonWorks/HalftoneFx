namespace GUI.Controls
{
    using GUI.Helpers;
    using GUI.BaseControls;

    using System.Drawing;

    public class UIProgressBar : UIRangeControl
    {
        public UIProgressBar() : base()
        {
            this.ReadOnly = true;
            this.TextType = UIRangeTextType.Percent;
            this.TextFlags = UIRangeTextFlags.Decimal;
        }

        protected override void DoRender(Graphics graphics)
        {
            graphics.DrawFrame(this.ScreenRect, this.Colors.Frame, this.Colors.Border, this.Style.Rounding);

            if (this.Percent > float.Epsilon)
            {
                var rect = this.ScreenRect;

                if (this.Vertical)
                {
                    var h = rect.Height * this.Percent;

                    if (this.Inverted)
                    {
                        rect = new RectangleF(rect.X, rect.Bottom - h, rect.Width, h);
                    }
                    else
                    {
                        rect = new RectangleF(rect.X, rect.Y, rect.Width, h);
                    }
                }
                else
                {
                    var w = rect.Width * this.Percent;

                    if (this.Inverted)
                    {
                        rect = new RectangleF(rect.X + rect.Width - w, rect.Y, w, rect.Height);
                    }
                    else
                    {
                        rect = new RectangleF(rect.X, rect.Y, rect.Width * this.Percent, rect.Height);
                    }
                }

                graphics.DrawRect(rect, this.Colors.ProgressBar, this.Style.Rounding, 0.5F);
            }

            base.DoRender(graphics); // Draw text.
            graphics.DrawBorderVolume(this.ScreenRect, this.Colors.BorderVolume, this.Style.Rounding);
        }
    }
}
