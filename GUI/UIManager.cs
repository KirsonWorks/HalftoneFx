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
            this.HandleMouseEvents = false;
        }

        public event EventHandler<UINotificationEventArgs> OnNotification = delegate { };

        public bool AntiAliasing { get; set; } = true;

        public UILayoutOptions LayoutOptions { get; set; } = UILayoutOptions.Default;

        public virtual void Refresh()
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
