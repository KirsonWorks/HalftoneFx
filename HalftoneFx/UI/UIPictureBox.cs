namespace HalftoneFx.UI
{
    using System;
    using System.Drawing;
    
    using GUI;
    using GUI.Controls;

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
                Name = "image",
                Parent = this,
                Stretch = true,
                AutoSize = false,
                HandleEvents = false,
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
            }
        }

        public SizeF ImageSize => this.imageControl.Size;

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

        public void ResetZoom()
        {
            if (this.Scale == 1.0f)
            {
                this.Zoom(this.Size - new Size(25, 25));
            }
            else
            {
                this.Zoom(1.0f);
            }
        }

        public void FullView()
        {
            this.imageControl.Size = this.toolZoom.OriginalSize = this.imageControl.Image.Size;
            this.toolZoom.Reset();
            this.toolPan.Reset(this.ScreenPositionCenter);
            this.OnZoomChanged(this, EventArgs.Empty);
        }

        protected override void DoParentResize(SizeF oldSize)
        {
            if (this.Parent is UIControl parent)
            {
                this.Size = parent.Size;
                this.toolPan.Reset(this.ScreenPositionCenter);
            }
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            if (this.imageControl.HasImage())
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

                base.DoMouseInput(e);
            }
        }
    }
}
