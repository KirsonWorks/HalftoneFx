namespace GUI.Controls
{
    using GUI.Helpers;

    using System;
    using System.Drawing;

    public class UIWindow : UIControl
    {
        public string Description { get; set; }

        public event EventHandler OnCloseClick = delegate {};

        public UIWindow() : base()
        {

            this.Visible = true;
            this.Size = new SizeF(200, 200);
        }

        protected override void DoResize(SizeF deltaSize)
        {

        }

        protected override void DoRender(Graphics graphics)
        {
            graphics.DrawFrame(this.ScreenRect, this.Colors.Window, this.Colors.Border, this.Style.WindowRounding);
        }
    }
}