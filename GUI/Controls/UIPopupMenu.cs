namespace GUI.Controls
{
    using GUI.Helpers;

    using System;
    using System.Drawing;
    using System.Collections.Generic;

    public class UIPopupMenu : UIPanel
    {
        private readonly List<UIPopupMenuItem> items = new List<UIPopupMenuItem>();

        private SizeF fittedSize = SizeF.Empty;

        private int selectedIndex = -1;

        public UIPopupMenu()
            : base()
        {
            this.Visible = false;
            this.AutoSize = true;
        }

        public UIPopupMenuItem AddItem(string caption)
        {
            var item = new UIPopupMenuItem
            {
                Caption = caption
            };

            this.items.Add(item);
            return item;
        }

        protected override SizeF GetFittedSize() => this.fittedSize;

        protected override void DoChangeVisibility()
        {
            if (this.Visible)
            {
                this.AdjustSize();
            }
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            switch (e.EventType)
            {
                case UIMouseEventType.Move:
                    for (var i = 0; i < this.items.Count; i++)
                    {
                        if (this.GetItemRect(i).Contains(e.Location))
                        {
                            this.selectedIndex = i;
                            break;
                        }
                    }

                    break;
            }

            base.DoMouseInput(e);
        }

        protected override void DoRender(Graphics graphics)
        {
            base.DoRender(graphics);

            for (var i = 0; i < this.items.Count; i++)
            {
                var item = this.items[i];
                var rect = this.GetItemRect(i);
                var bgColor = this.selectedIndex == i ? this.Style.Colors.ButtonHovered : this.Style.Colors.Button;
                graphics.DrawRect(rect, bgColor, this.Style.Rounding);
                rect = rect.Inflate(-this.Style.InnerShrink);
                graphics.DrawText(rect, this.Style.Fonts.Default, this.Style.Colors.Text, UIAlign.LeftMiddle, false, false, item.Caption);
            }
        }
        
        private RectangleF GetItemRect(int index)
        {
            var pos = this.ScreenPosition;
            var padding = this.Style.Padding;
            var spacing = this.Style.Spacing;

            var itemHeight = ((this.fittedSize.Height - padding * 2) / this.items.Count);
            var sz = new SizeF(this.fittedSize.Width - padding * 2, itemHeight);
            
            var p = new PointF(pos.X + padding, pos.Y + padding + (index * sz.Height));
            sz.Height -= spacing;
            
            return new RectangleF(p, sz);
        }

        private void AdjustSize()
        {
            var maxTextSize = SizeF.Empty;
            var padding = this.Style.Padding;
            var spacing = this.Style.Spacing;

            foreach (var item in this.items)
            {
                var textSize = GraphicsHelper.StringSize(item.Caption, this.Style.Fonts.Default);
                maxTextSize = maxTextSize.Max(textSize);
            }

            maxTextSize.Width += padding * 3;
            maxTextSize.Height += padding * 2 + spacing;

            this.fittedSize = new SizeF(padding + maxTextSize.Width + padding, 
                padding + (maxTextSize.Height * this.items.Count) + padding);
        }
    }

    public class UIPopupMenuItem
    {
        public string Caption { get; set; }

        public event EventHandler<UIMouseEventArgs> OnClick;
    }
}
