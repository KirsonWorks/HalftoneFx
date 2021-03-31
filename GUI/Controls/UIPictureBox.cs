namespace GUI.Controls
{
    using GUI;
    using GUI.Editor;

    using System;
    using System.Drawing;

    public class UIPictureBox : UIControl
    {
        private readonly UIImage imageControl;

        private readonly UIToolPan toolPan = new UIToolPan();

        private readonly UIToolZoom toolZoom = new UIToolZoom();

        public event EventHandler OnZoomChanged = delegate { };

        public UIPictureBox() : base()
        {
            this.imageControl = new UIImage
            {
                Name = UIFactory.Name(typeof(UIImage)),
                Parent = this,
                Stretch = true,
                AutoSize = false,
                HandleMouseEvents = false,
                BorderColor = Color.LightSlateGray,
            };

            this.toolPan.Control = this.imageControl;
            this.toolZoom.Control = this.imageControl;
        }

        public int ImageBorderSize { get; set; } = 4;

        public Color ImageBorderColor
        {
            get => this.imageControl.BorderColor;
            set => this.imageControl.BorderColor = value;
        }

        public Image Image
        { 
            get => this.imageControl.Image;
            
            set 
            {
                this.imageControl.Image = value;
                this.toolZoom.OriginalSize = (value != null) ? value.Size : System.Drawing.Size.Empty;
            }
        }

        public float Scale => this.toolZoom.Scale;

        public float ScaleMin => this.toolZoom.ScaleMin;

        public float ScaleMax => this.toolZoom.ScaleMax;

        public void Zoom(float scale)
        {
            this.toolZoom.Zoom(scale);
            this.toolPan.Reset(this.ScreenPositionCenter);
            this.OnZoomChanged(this, EventArgs.Empty);
        }

        public void Zoom(SizeF size)
        {
            this.toolZoom.Zoom(size);
            this.toolPan.Reset(this.ScreenPositionCenter);
            this.OnZoomChanged(this, EventArgs.Empty);
        }

        public void ZoomIn()
        {
            this.toolZoom.Zoom(1, this.ScreenPositionCenter);
            this.OnZoomChanged(this, EventArgs.Empty);
        }

        public void ZoomOut()
        {
            this.toolZoom.Zoom(-1, this.ScreenPositionCenter);
            this.OnZoomChanged(this, EventArgs.Empty);
        }

        public void FullView()
        {
            this.Zoom(1.0f);
        }

        public void FitToScreen()
        {
            this.Zoom(this.Size - new Size(30, 30));
        }

        public void OptimalView()
        {
            if (this.Image.Width > this.Width || this.Image.Height > this.Height)
            {
                this.FitToScreen();
            }
            else
            {
                this.FullView();
            }
        }

        public void ResetZoom()
        {
            if (this.Scale == 1.0f)
            {
                this.FitToScreen();
            }
            else
            {
                this.FullView();
            }
        }

        protected override void DoParentResize()
        {
            this.toolPan.Reset(this.ScreenPositionCenter);
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            if (this.imageControl.HasImage())
            {
                if (e.Button == UIMouseButtons.Left || e.EventType == UIMouseEventType.Wheel)
                {
                    switch (e.EventType)
                    {
                        case UIMouseEventType.Down:
                            this.toolPan.Start(e.Location);
                            this.imageControl.BorderSize = this.ImageBorderSize;
                            break;

                        case UIMouseEventType.Move:
                            this.toolPan.Move(e.Location);

                            break;

                        case UIMouseEventType.Up:
                            this.toolPan.Active = false;
                            this.imageControl.BorderSize = 0;
                            break;

                        case UIMouseEventType.Wheel:
                            this.toolZoom.Zoom(e.Delta, e.Location);
                            this.OnZoomChanged(this, EventArgs.Empty);
                            break;
                    }
                }

                base.DoMouseInput(e);
            }
        }
    }
}
