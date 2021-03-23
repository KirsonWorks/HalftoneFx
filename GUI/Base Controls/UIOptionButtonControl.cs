namespace GUI.BaseControls
{
    using GUI.Helpers;

    using System.Drawing;

    public abstract class UIOptionButtonControl : UIButtonControl
    {
        private SizeF textSize = new SizeF(1, 1);

        public UIOptionButtonControl() : base()
        {
            this.SetSize(100, 20);
            this.ToggleMode = true;
            this.AutoSize = true;
        }

        public bool WordWrap { get; set; }

        public PointF TextAlign { get; set; } = UIAlign.LeftTop;

        public override string Caption 
        { 
            get => base.Caption;

            set
            {
                if (base.Caption != value)
                {
                    base.Caption = value;
                    this.textSize = GraphicsHelper.StringSize(value, this.Style.Fonts.Default);
                }
            }
        }

        public override bool ToggleMode
        {
            get => base.ToggleMode;
            set => base.ToggleMode = true;
        }

        protected override SizeF GetFittedSize()
        {
            var minSize = this.GetCheckMarkSize();

            if (this.textSize.Width > 0)
            {
                minSize.Width += this.textSize.Width + this.Style.Spacing;
            }

            return minSize;
        }

        protected virtual SizeF GetCheckMarkSize()
        {
            return new SizeF().OneValue(this.Style.ToggleSize);
        }

        protected Color GetColor()
        {
            if (this.Enabled)
            {
                if (this.IsPressed)
                {
                    return this.Colors.FrameActive;
                }
                else if (this.IsHovered)
                {
                    return this.Colors.FrameHovered;
                }
            }
            else
            {
                return this.Colors.FrameDisabled;
            }

            return this.Colors.Frame;
        }

        protected override void DoRender(Graphics graphics)
        {
            if (this.textSize.Width > 0)
            {
                var sr = this.ScreenRect;
                var cmw = this.GetCheckMarkSize().Width + this.Style.Spacing;

                sr.X += cmw;
                sr.Width -= cmw;

                graphics.DrawText(sr, this.Style.Fonts.Default, this.GetTextColor(), this.TextAlign, this.AutoSize, this.WordWrap, this.Caption);
            }
        }
    }
}
