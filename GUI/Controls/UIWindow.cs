namespace GUI.Controls
{
    using GUI.Helpers;

    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIWindow : UIControl
    {
        private bool canMove = false; 

        public event EventHandler OnCloseClick = delegate { };

        public UIWindow()
            : base()
        {
            this.Visible = false;
            this.ClipContent = true;
            this.Size = new SizeF(180, 180);
        }

        public override RectangleF ClientRect
        {
            get
            {
                var rect = base.ClientRect;
                var size = this.Style.WindowTitleSize;
                rect.Y += size;
                rect.Height -= size;
                return rect;
            }

        }

        public void Close()
        {
        }

        protected override SizeF GetPreferedSize()
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

            // Window frame.
            var rect = this.ScreenRect;
            graphics.DrawRect(rect, this.Colors.WindowShadow, this.Style.WindowRounding, -this.Style.WindowShadowSize);
            graphics.DrawFrame(rect, this.Colors.Window, this.Colors.Border, this.Style.WindowRounding);
            graphics.DrawBorderVolume(rect, this.Colors.BorderVolume, this.Style.WindowRounding);

            // Title bar.
            rect.Height = this.Style.WindowTitleSize;
            graphics.DrawRect(rect, this.Style.Colors.WindowTitle, this.Style.WindowRounding);

            // Title text.
            rect.Inflate(-this.Style.Padding, 0);
            graphics.DrawText(rect, this.Style.Fonts.Default, this.Style.Colors.Text, UIAlign.LeftMiddle, false, false, this.Caption);
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
    }
}