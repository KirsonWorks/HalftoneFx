namespace KWUI.Controls
{
    using KWUI.BaseControls;
    using KWUI.Helpers;

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    [Flags]
    public enum UIWindowFeatures
    {
        None = 0,
        TitleBar = 1,
        ClosingBox = 2,
        ExpandingBox = 4,
        Draggable = 8,
    }

    public class UIWindow : UIControl
    {
        private readonly UIToolButton buttonClose;

        private readonly UIToolButton buttonExpand;

        private UIWindowFeatures features;

        private float height;

        private bool dragging = false;

        private bool expanded = true;

        public event EventHandler OnClose = delegate { };

        public UIWindow()
            : base()
        {
            this.Visible = false;
            this.ClipContent = true;
            this.Size = new SizeF(180, 180);

            var size = this.Style.WindowBarSize - this.Style.InnerShrink * 2;
            this.buttonClose = this.NewToolButton(size: size, shape: this.Shapes.Cross);
            this.buttonClose.OnMouseClick += (s, e) => this.Close();

            this.buttonExpand = this.NewToolButton(size: size, shape: this.Shapes.Upperscore);
            this.buttonExpand.OnMouseClick += (s, e) => this.Expanded = !this.Expanded;

            this.Features = UIWindowFeatures.TitleBar | UIWindowFeatures.ClosingBox;
        }

        public UIWindowFeatures Features
        {
            get => this.features;

            set
            {
                this.features = value;
                this.TitleBarPerform();
            }
        }

        public bool Expanded
        {
            get => this.expanded;

            set
            {
                if (this.Features.HasFlag(UIWindowFeatures.ExpandingBox))
                {
                    if (this.expanded != value)
                    {
                        this.expanded = value;
                        this.ExpandCollapse(value);
                    }
                }
            }
        }

        private void ExpandCollapse(bool expanded)
        {
            if (!expanded)
            {
                this.height = this.Height;
                this.Height = 0;
                this.buttonExpand.Shape = this.Shapes.Square;
                return;
            }

            this.Height = height;
            this.buttonExpand.Shape = this.Shapes.Upperscore;
        }

        public override RectangleF ClientRect
        {
            get
            {
                var rect = base.ClientRect;

                if (this.features.HasFlag(UIWindowFeatures.TitleBar))
                {
                    var height = this.Style.WindowBarSize;
                    rect.Y += height;
                    rect.Height -= height;
                }

                return rect;
            }
        }

        public void Close()
        {
            if (this.Visible)
            {
                this.Hide();
                this.OnClose(this, EventArgs.Empty);
            }
        }

        protected override GraphicsPath GetClipPath(Graphics graphics, RectangleF rect)
        {
            return graphics.GetClipPath(rect, this.Style.Rounding);
        }

        protected override void DoRender(Graphics graphics)
        { 
            /*
             * For modal window.
             * 
            if (this.Parent is UIControl parent)
            {
                using (var brush = new SolidBrush(Color.FromArgb(180, Color.Black)))
                {
                    graphics.FillRectangle(brush, parent.ScreenClientRect);
                }
            }
            */

            // Window frame.
            var rect = this.ScreenRect;
            graphics.DrawRect(rect, this.Colors.WindowShadow, this.Style.WindowRounding, -this.Style.WindowShadowSize);
            graphics.DrawFrame(rect, this.Colors.Window, this.Colors.Border, this.Style.WindowRounding);
            graphics.DrawBorderVolume(rect, this.Colors.BorderVolume, this.Style.WindowRounding);

            if (this.Features.HasFlag(UIWindowFeatures.TitleBar))
            {
                // Title bar.
                rect.Height = this.Style.WindowBarSize;
                graphics.DrawRect(rect, this.Colors.WindowBar, this.Style.WindowRounding);

                // Title bar caption.
                rect.Inflate(-this.Style.Padding, 0);
                graphics.DrawText(rect, this.Fonts.Default, this.Colors.WindowCaption, UIAlign.LeftMiddle, false, false, this.Caption);

                this.buttonClose.Render(graphics);
                this.buttonExpand.Render(graphics);
            }
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            switch (e.EventType)
            {
                case UIMouseEventType.Down:
                    this.BringToFront();

                    var dragArea = this.ScreenRect;

                    if (!this.Features.HasFlag(UIWindowFeatures.Draggable))
                    {
                        dragArea.Height = this.Style.WindowBarSize;
                    }

                    if (IsMouseOver && dragArea.Contains(e.Location))
                    {
                        this.StarDrag(e.Location);
                        this.dragging = true;
                    }

                    break;

                case UIMouseEventType.Move:
                    if (this.dragging)
                    {
                        this.Drag(e.Location);
                    }

                    break;

                case UIMouseEventType.Up:
                    this.dragging = false;
                    break;
            }

            base.DoMouseInput(e);
        }

        private void TitleBarPerform()
        {
            var features = this.features;
            var buttons = new List<UIButtonControl> { this.buttonExpand, this.buttonClose };

            if (!features.HasFlag(UIWindowFeatures.TitleBar))
            {
                features &= ~UIWindowFeatures.ClosingBox;
                features &= ~UIWindowFeatures.ExpandingBox;
            }

            if (!features.HasFlag(UIWindowFeatures.ClosingBox))
            {
                buttons[1].Visible = false;
                buttons.RemoveAt(1);
            }

            if (!features.HasFlag(UIWindowFeatures.ExpandingBox))
            {
                buttons[0].Visible = false;
                buttons.RemoveAt(0);
            }

            var margin = new SizeF(this.Style.Padding, -this.Style.WindowBarSize + this.Style.InnerShrink);
            for (var i = buttons.Count - 1; i >= 0; i--)
            {
                var button = buttons[i];
                button.Visible = true;
                button.SetLayoutPreset(UILayoutPreset.TopRight, margin);
                margin.Width += button.Width + this.Style.Spacing;
            }
        }
    }
}