namespace GUI.Controls
{
    using GUI.Helpers;

    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIWindow : UIControl
    {
        private bool canMove = false;

        public event EventHandler OnCloseClick = delegate {};

        public UIWindow()
            : base()
        {
            this.Visible = false;
            this.Size = new SizeF(100, 100);
            this.ClipContent = true;
        }

        public void Close()
        {
        }

        protected override SizeF GetMinimumSize()
        {
            return new SizeF(100, 100);
        }

        protected override GraphicsPath GetClipPath(Graphics graphics, RectangleF rect)
        {
            return graphics.GetClipPath(rect, this.Style.Rounding);
        }

        protected override void DoRender(Graphics graphics)
        {
            /*
            if (this.Parent is UIControl parent)
            {
                using (var brush = new SolidBrush(Color.FromArgb(180, Color.Black)))
                {
                    graphics.FillRectangle(brush, parent.ScreenRect);
                }
            }
            */
            var rect = this.ScreenRect;
            var clr = Color.FromArgb(100, Color.Black);
            graphics.DrawRect(rect, clr, this.Style.WindowRounding, -3.5f);
            graphics.DrawFrame(rect, this.Colors.Window, this.Colors.Border, this.Style.WindowRounding);
            graphics.DrawBorderVolume(rect, this.Colors.BorderVolume, this.Style.WindowRounding);
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            switch (e.EventType)
            {
                case UIMouseEventType.Down:
                    this.BringToFront();

                    if (IsMouseOver)
                    {
                        this.StarDrag(e.Location);
                        this.canMove = true;
                    }

                    break;

                case UIMouseEventType.Move:
                    if (this.canMove)
                    {
                        this.Drag(e.Location);
                    }

                    break;

                case UIMouseEventType.Up:
                    this.canMove = false;
                    break;
            }

            base.DoMouseInput(e);
        }

        protected override void DoRenderOverlay(Graphics graphics)
        {
            var r = this.ScreenRect;
            r.Height = 24;

            var tr = r;
            tr.X += 10;

            graphics.DrawRect(r, Color.Crimson, this.Style.WindowRounding);
            graphics.DrawText(tr, Style.Fonts.Default, this.Style.Colors.Text, UIAlign.LeftMiddle, false, false, this.Caption);
        }
    }
}