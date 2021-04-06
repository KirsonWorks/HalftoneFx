namespace KWUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIManager : UIControl
    {
        private readonly Stack<UIControl> modals = new Stack<UIControl>();

        private SmoothingMode smoothModeStored;

        private UIControl topLevelControl;

        public UIManager() : base()
        {
            this.Name = "ui-manager";
            this.topLevelControl = this;
            this.HandleMouseEvents = false;
        }

        public event EventHandler<UINotificationEventArgs> OnNotification = delegate { };

        public bool AntiAliasing { get; set; } = true;

        public UIControl TopLevelControl => this.topLevelControl;

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
            switch (notification)
            {
                case UINotification.BeginModal:
                    this.modals.Push(this.TopLevelControl);
                    this.topLevelControl = sender as UIControl;
                    break;

                case UINotification.EndModal:
                    if (this.modals.Count > 0)
                    {
                        this.topLevelControl = this.modals.Pop();
                    }

                    break;
            }

            this.OnNotification?.Invoke(sender, new UINotificationEventArgs { What = notification });
        }
    }

    public class UINotificationEventArgs : EventArgs
    {
        public UINotification What { get; set; }
    }
}
