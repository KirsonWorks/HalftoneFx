namespace GUI.Controls
{
    using GUI.Helpers;
    
    using System;
    using System.Drawing;

    public class UIImage : UIControl
    {
        private Image image;

        private string path;

        public event EventHandler OnImageChanged = delegate { };

        public UIImage() : base()
        {
            this.AutoSize = true;
            this.Size = new SizeF(20, 20);
        }

        public Image Image
        {
            get => this.image;

            set
            {   
                this.image = value;

                if (this.AutoSize && this.image != null)
                {
                    this.Size = this.image.Size;
                }

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

        protected override SizeF GetFittedSize()
        {
            if (this.HasImage())
            {
                return this.Image.Size;
            }

            return base.GetFittedSize();
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

            if (this.AutoSize)
            {
                graphics.DrawImage(this.Image, this.ScreenPosition);
            }
            else
            {
                var cropRect = new RectangleF(PointF.Empty, this.Size);

                if (this.Stretch)
                {
                    cropRect = new RectangleF(PointF.Empty, this.Image.Size);
                }
                else if (this.Center)
                {
                    cropRect.X = (this.Image.Width - this.Width) / 2;
                    cropRect.Y = (this.Image.Height - this.Height) / 2;
                }

                graphics.DrawImage(this.Image, this.ScreenRect, cropRect, GraphicsUnit.Pixel);
            }
        }
    }
}
