﻿namespace KWUI.BaseControls
{
    using KWUI.Helpers;

    using System.Drawing;

    public abstract class UIOptionButtonControl : UIButtonControl
    {
        private SizeF textSize = new SizeF(1, 1);

        public UIOptionButtonControl() 
            : base()
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
                    this.UpdatePreferredSize();
                }
            }
        }

        public override bool ToggleMode
        {
            get => base.ToggleMode;
            set => base.ToggleMode = true;
        }

        protected override SizeF GetPreferredSize()
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


        protected Color GetBgColor()
        {
            return this.GetStateColor(
                this.Colors.Frame,
                this.Colors.FrameActive,
                this.Colors.FrameHovered,
                this.Colors.Frame,
                this.Colors.FrameDisabled);
        }

        protected override void DoRender(Graphics graphics)
        {
            if (this.textSize.Width > 0)
            {
                var sr = this.ScreenRect;
                var cmw = this.GetCheckMarkSize().Width + this.Style.Spacing;

                sr.X += cmw;
                sr.Width -= cmw;

                graphics.DrawText(sr, this.Style.Fonts.Default, this.GetFgColor(), this.TextAlign, this.AutoSize, this.WordWrap, this.Caption);
            }
        }
    }
}
