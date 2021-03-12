namespace GUI.Controls
{
    using GUI.Helpers;

    using System.Drawing;

    public class UIStatusBar : UIControl
    {
        public UIStatusBar() : base()
        {
            this.Height = 26;
            this.HandleEvents = false;
        }

        public override string Caption
        {
            get => base.Caption;
            set => base.Caption = value;
        }

        protected override void DoRender(Graphics graphics)
        {
            var sr = this.ScreenRect;
            graphics.DrawFrame(sr, this.Style.Colors.StatusBar, Color.Empty, 0);
            graphics.DrawText(sr.Inflate(-this.Style.Padding), this.Style.Fonts.Default,
                    this.Style.Colors.Text, UIAlign.LeftMiddle, false, false, this.Caption);
        }

        protected override void DoParentResize(SizeF deltaSize)
        {
            this.UpdateSize();
        }

        protected override void DoParentChanged()
        {
            this.UpdateSize();
        }

        private void UpdateSize()
        {
            if (this.Parent is UIControl parent)
            {
                this.SetPosition(0.0f, parent.Height - this.Height);
                this.Width = parent.Width;
            }
        }
    }
}