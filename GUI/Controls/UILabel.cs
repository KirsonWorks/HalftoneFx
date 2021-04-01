namespace GUI.Controls
{
    using GUI.Helpers;
    using System;
    using System.Drawing;

    public class UILabel : UIControl
    {
        private SizeF textSize = SizeF.Empty;

        public UILabel() : base()
        {
            this.textSize = SizeF.Empty;
            this.AutoSize = true;
        }

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

        public bool WordWrap { get; set; }

        public PointF TextAlign { get; set; } = UIAlign.None;

        protected override SizeF GetPreferedSize()
        {
            return this.textSize;
        }

        protected override void DoRender(Graphics graphics)
        {
            var textColor = this.Enabled ? this.Style.Colors.Text : this.Style.Colors.TextDisabled;
            graphics.DrawText(this.ScreenRect, this.Style.Fonts.Default, textColor, this.TextAlign, this.AutoSize, this.WordWrap, this.Caption);
        }
    }
}
