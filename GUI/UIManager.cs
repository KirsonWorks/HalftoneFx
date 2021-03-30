namespace GUI
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIManager : UIControl
    {
        private SmoothingMode smoothModeStored;

        public UIManager() : base()
        {
            this.Name = "ui-manager";
        }

        public event EventHandler<UINotificationEventArgs> OnNotification = delegate { };

        public bool AntiAliasing { get; set; } = true;

        public Point CursorPosition { get; protected set; }

        public UILayoutOptions LayoutOptions { get; set; } = UILayoutOptions.Default;

        public virtual void Refresh()
        {
        }

        public void HandleMouseDown(UIMouseEventArgs e)
        {
            HandleMouseDown(this, e);
        }

        public void HandleMouseMove(UIMouseEventArgs e)
        {
            this.CursorPosition = Point.Round(e.Location);
            HandleMouseMove(this, e);
        }

        public void HandleMouseUp(UIMouseEventArgs e)
        {
            HandleMouseUp(this, e);
        }

        public void HandleMouseWheel(UIMouseEventArgs e)
        {
            HandleMouseWheel(this, e);
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            
        }

        protected override void DoRender(Graphics graphics)
        {
            this.smoothModeStored = graphics.SmoothingMode;
            graphics.SmoothingMode = this.AntiAliasing ? SmoothingMode.AntiAlias : this.smoothModeStored;
        }

        protected override void DoRenderOverlay(Graphics graphics)
        {
            graphics.SmoothingMode = this.smoothModeStored;
        }

        protected override void Notification(UINode sender, UINotification notification)
        {
            this.OnNotification?.Invoke(sender, new UINotificationEventArgs { What = notification });
        }
    }

    public class UINotificationEventArgs : EventArgs
    {
        public UINotification What { get; set; }
    }
}
