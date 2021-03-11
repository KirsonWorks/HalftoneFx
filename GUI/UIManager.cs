namespace GUI
{
    using GUI.Helpers;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class UIManager : UIControl
    {
        private readonly Stopwatch fpsInterval;

        private long fpsCounter = 0;

        private SmoothingMode smoothModeStored;

        public UIManager() : base()
        {
            this.Name = "ui-manager";
            this.HandleEvents = false;
            this.fpsInterval = Stopwatch.StartNew();
        }

        public event EventHandler<UINotificationEventArgs> OnNotification = delegate { };
       
        public long FPS { get; private set; }

        public bool AntiAliasing { get; set; } = true;

        public Point CursorPosition { get; protected set; }

        public UILayoutOptions LayoutOptions { get; set; } = UILayoutOptions.Default;

        protected override void DoRender(Graphics graphics)
        {
            this.smoothModeStored = graphics.SmoothingMode;
            graphics.SmoothingMode = this.AntiAliasing ? SmoothingMode.AntiAlias : this.smoothModeStored;
        }

        protected override void DoRenderOverlay(Graphics graphics)
        {
            graphics.SmoothingMode = this.smoothModeStored;

            if (this.fpsInterval.ElapsedMilliseconds >= 1000)
            {
                this.fpsInterval.Restart();
                this.FPS = this.fpsCounter;
                this.fpsCounter = 0;
            }

            this.fpsCounter++;
#if DEBUG
            graphics.DrawText(new RectangleF(10, 10, 30, 25), this.Style.Fonts.Default, this.Style.Colors.Text, this.FPS.ToString());
#endif
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
