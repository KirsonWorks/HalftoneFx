namespace GUI.Controls
{
    using GUI.Helpers;

    using System;
    using System.Drawing;
    using System.Collections.Generic;

    public class UIPopupMenu : UIPanel
    {
        private readonly List<UIPopupMenuItem> items = new List<UIPopupMenuItem>();

        private bool pressed;

        private SizeF itemSize;

        private SizeF fittedSize;

        private int selectedIndex;

        public UIPopupMenu()
            : base()
        {
            this.Visible = false;
            this.AutoSize = true;
        }

        public UIPopupMenuItem AddItem(string text, Image icon, Action click, bool closeAfterClick = true)
        {
            var item = new UIPopupMenuItem
            {
                Text = text,
                Icon = icon,
                Click = click,
                CloseAfterClick = closeAfterClick,
            };

            this.items.Add(item);
            return item;
        }

        protected override SizeF GetFittedSize() => this.fittedSize;

        protected override void DoChangeVisibility()
        {
            if (this.Visible)
            {
                this.pressed = false;
                this.selectedIndex = -1;
                this.AdjustSize();
            }
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            switch (e.EventType)
            {
                case UIMouseEventType.Down:
                    this.pressed = e.Button == UIMouseButtons.Left;
                    break;

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

                case UIMouseEventType.Up:
                    if (this.pressed &&
                        this.selectedIndex > -1 &&
                        this.GetItemRect(this.selectedIndex).Contains(e.Location))
                    {
                        var item = this.items[this.selectedIndex];
                        item?.Click?.Invoke();

                        if (item.CloseAfterClick)
                        {
                            this.Hide();
                        }
                    }

                    this.pressed = false;
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
                var bgColor = this.Style.Colors.Button;

                if (this.selectedIndex == i)
                {
                    bgColor = pressed ? this.Style.Colors.ButtonActive : this.Style.Colors.ButtonHovered;
                }

                graphics.DrawRect(rect, bgColor, this.Style.Rounding);
                rect = rect.Inflate(-this.Style.InnerShrink);

                if (item.Icon != null)
                {
                    graphics.DrawImage(item.Icon, rect.X, rect.Y + (rect.Height - item.Icon.Height) / 2);
                    rect.X += item.Icon.Width + this.Style.Padding;
                }
                
                graphics.DrawText(rect, this.Style.Fonts.Default, this.Style.Colors.Text, UIAlign.LeftMiddle, false, false, item.Text);
            }
        }
        
        private RectangleF GetItemRect(int index)
        {
            var pos = this.ScreenPosition;
            var padding = this.Style.Padding;
            var spacing = this.Style.Spacing;

            pos = new PointF(pos.X + padding, 
                             pos.Y + padding + (index * (this.itemSize.Height + spacing)));
            
            return new RectangleF(pos, this.itemSize);
        }

        private void AdjustSize()
        {
            this.itemSize = SizeF.Empty;
            var padding = this.Style.Padding;
            var spacing = this.Style.Spacing;

            foreach (var item in this.items)
            {
                var textSize = GraphicsHelper.StringSize(item.Text, this.Style.Fonts.Default);
                this.itemSize = itemSize.Max(textSize);

                if (item.Icon != null)
                {
                    var iconSize = item.Icon.Size;
                    this.itemSize = this.itemSize.Max(new SizeF(iconSize.Width + padding + textSize.Width, iconSize.Height));
                }
            }

            itemSize.Width += padding * 2;
            itemSize.Height += padding * 2;

            this.fittedSize = new SizeF(padding * 2 + itemSize.Width,
                padding * 2 + ((itemSize.Height + spacing) * this.items.Count - spacing));
        }
    }

    public class UIPopupMenuItem
    {
        public string Text { get; set; }
        
        public Image Icon { get; set; }

        public bool CloseAfterClick { get; set; } = true;

        public Action Click;
    }
}
