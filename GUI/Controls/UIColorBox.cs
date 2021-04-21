namespace KWUI.Controls
{
    using KWUI.BaseControls;
    using KWUI.Helpers;

    using System;
    using System.Drawing;

    public class UIColorBox : UIButtonControl
    {
        private static readonly int checkerSize = 4;

        private static Image checkeredTexture = null;

        private Color color = Color.Empty;

        private Color? defaultColor = null;

        public UIColorBox()
            : base()
        {
            if (checkeredTexture == null)
            {
                checkeredTexture = new Bitmap(checkerSize * 2, checkerSize * 2);
                checkeredTexture.GenerateCheckerTexture(checkerSize, Color.White, Color.Silver);
            }
        }

        public Color Color 
        {
            get => this.color;

            set
            {
                if (this.color != value)
                {
                    if (this.defaultColor == null)
                    {
                        this.defaultColor = value;
                    }

                    this.color = value;
                    this.DoChanged();
                }
            }
        }

        public byte R
        {
            get => this.Color.R;
            set => this.Color = Color.FromArgb(this.A, value, this.G, this.B);
        }

        public byte G
        {
            get => this.Color.G;
            set => this.Color = Color.FromArgb(this.A, this.R, value, this.B);
        }

        public byte B
        {
            get => this.Color.B;
            set => this.Color = Color.FromArgb(this.A, this.R, this.G, value);
        }

        public byte A
        {
            get => this.Color.A;
            set => this.Color = Color.FromArgb(value, this.R, this.G, this.B);
        }


        protected override void DoRender(Graphics graphics)
        {
            var rect = this.ScreenRect;
            var radius = this.Style.Rounding;

            using (var background = new TextureBrush(checkeredTexture))
            {
                graphics.DrawRect(rect, background, radius);
            }

            if (!this.Color.IsEmpty)
            {
                using (var gradient = new SolidBrush(this.Color))
                {
                    graphics.DrawRect(rect, gradient, radius);
                }

                var mixed = Color.White.Blend(this.Color, this.Color.A / 255.0f);
                var textColor = mixed.GetLuminance() >= 0.65 ? Color.Black : Color.White;
                graphics.DrawText(rect, this.Fonts.Default, textColor, UIAlign.Center, false, false, $"#{Color.ToArgb():X8}");
            }

            var stateColor = this.GetStateColor();

            if (!stateColor.IsEmpty && stateColor.A > 0)
            {
                graphics.DrawRect(rect, Color.FromArgb(128, stateColor), radius);
            }

            graphics.DrawBorder(rect, this.Style.Colors.Border, radius);
            graphics.DrawBorderVolume(rect, this.Style.Colors.BorderVolume, radius);
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            if (e.EventType == UIMouseEventType.Up && e.Button == UIMouseButtons.Right)
            {
                this.Color = this.defaultColor ?? this.Color;
            }

            base.DoMouseInput(e);
        }

        private Color GetStateColor()
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

            return Color.Empty;
        }
    }
}
