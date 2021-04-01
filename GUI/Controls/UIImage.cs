namespace GUI.Controls
{
    using GUI.Helpers;
    
    using System;
    using System.Drawing;

    public class UIImage : UIControl
    {
        private Image image;

        private string path;

        private Size imageSize;

        public event EventHandler OnImageChanged = delegate { };

        public UIImage() : base()
        {
            this.AutoSize = true;
            this.Size = new SizeF(20, 20);
            this.BorderColor = this.Style.Colors.Border;
        }

        public Image Image
        {
            get => this.image;

            set
            {
                this.image = value;
                this.imageSize = value != null ? value.Size : System.Drawing.Size.Empty;
                this.UpdatePreferredSize();
                this.OnImageChanged(this, EventArgs.Empty);
            }
        }

        public bool Stretch { get; set; }

        public bool Center { get; set; }

        public int BorderSize { get; set; }

        public Color BorderColor { get; set; }

        public string FileName
        {
            get => this.path;

            set
            {
                if (this.path != value)
                {
                    this.path = value;
                    this.Image = Image.FromFile(this.path);
                }
            }
        }

        public bool HasImage()
        {
            return this.Image != null;
        }

        protected override SizeF GetPreferedSize()
        {
            return this.imageSize;
        }

        protected override void DoRender(Graphics graphics)
        {
            if (!this.HasImage())
            {
                return;
            }

            if (this.BorderSize > 0 && !this.BorderColor.IsEmpty)
            {
                graphics.DrawBorder(this.ScreenRect, this.BorderColor, 0, -this.BorderSize);
            }

            var interpolationMode = graphics.InterpolationMode;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;

            if (this.AutoSize)
            {
                graphics.DrawImage(this.Image, Point.Round(this.ScreenPosition));
            }
            else
            {
                var cropRect = new RectangleF(PointF.Empty, this.Size);

                if (this.Stretch)
                {
                    cropRect = new RectangleF(PointF.Empty, this.imageSize);
                }
                else if (this.Center)
                {
                    cropRect.X = (this.imageSize.Width - this.Width) / 2;
                    cropRect.Y = (this.imageSize.Height - this.Height) / 2;
                }

                graphics.DrawImage(this.Image, Rectangle.Round(this.ScreenRect),
                    Rectangle.Round(cropRect), GraphicsUnit.Pixel);
            }

            graphics.InterpolationMode = interpolationMode;
        }
    }
}
