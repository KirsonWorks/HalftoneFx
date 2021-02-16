namespace GUI
{
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

        public long FPS { get; private set; }

        public bool AntiAliasing { get; set; } = true;

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
        }
    }
}
