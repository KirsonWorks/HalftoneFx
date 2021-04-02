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
        Draggable = 8
    }

    public class UIWindow : UIControl
    {
        private readonly UIToolButton buttonClose;

        private readonly UIToolButton buttonExpand;

        private UIWindowFeatures features;

        private SizeF clientSize;

        private bool dragging = false;

        private bool expanded = true;

        public event EventHandler OnClose = delegate { };

        public UIWindow()
            : base()
        {
            this.Visible = false;
            this.ClipContent = true;
            this.Size = new SizeF(180, 180);

            this.buttonClose = this.NewNode<UIToolButton>("");
            this.buttonClose.SetSize(16, 16);
            
            this.buttonClose.Shape = new PointF[]
                {
                    new PointF(0.1f, 0.1f),
                    new PointF(0.3f, 0.1f),
                    new PointF(0.5f, 0.4f),
                    new PointF(0.7f, 0.1f),
                    new PointF(0.9f, 0.1f),
                    new PointF(0.6f, 0.5f),
                    new PointF(0.9f, 0.9f),
                    new PointF(0.7f, 0.9f),
                    new PointF(0.5f, 0.6f),
                    new PointF(0.3f, 0.9f),
                    new PointF(0.1f, 0.9f),
                    new PointF(0.4f, 0.5f),
                };

            this.buttonClose.OnMouseClick += (s, e) => this.Close();

            this.buttonExpand = this.NewNode<UIToolButton>("");
            this.buttonExpand.SetSize(16, 16);
            this.buttonExpand.ToggleMode = true;
            this.buttonExpand.Shape = this.Shapes.CheckMark;
            this.buttonExpand.OnChanged += (s, e) => this.Expanded = !this.buttonExpand.Checked;

            this.Features = UIWindowFeatures.TitleBar | UIWindowFeatures.ClosingBox | UIWindowFeatures.ExpandingBox | UIWindowFeatures.Draggable;
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
                if (this.Features.HasFlag(UIWindowFeatures.TitleBar))
                {
                    if (this.expanded != value)
                    {
                        this.expanded = value;
                        this.ExpandCollapse(value);
                    }
                }
            }
        }

        private void ExpandCollapse(bool value)
        {
            if (!value)
            {
                this.clientSize = this.ClientRect.Size;
                this.Height = this.Style.WindowTitleSize;
                return;
            }

            this.Height = this.Style.WindowTitleSize + this.clientSize.Height;
        }

        public override RectangleF ClientRect
        {
            get
            {
                var rect = base.ClientRect;

                if (this.features.HasFlag(UIWindowFeatures.TitleBar))
                {
                    var size = this.Style.WindowTitleSize;
                    rect.Y += size;
                    rect.Height -= size;
                }

                return rect;
            }
        }

        public void Close()
        {
            this.Hide();
            this.OnClose(this, EventArgs.Empty);
        }

        protected override GraphicsPath GetClipPath(Graphics graphics, RectangleF rect)
        {
            return graphics.GetClipPath(rect, this.Style.Rounding);
        }

        protected override void DoRender(Graphics graphics)
        {
            /*
            if (this.Parent is UIControl parent)
            {
                using (var brush = new SolidBrush(Color.FromArgb(180, Color.Black)))
                {
                    graphics.FillRectangle(brush, parent.ScreenRect);
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
                rect.Height = this.Style.WindowTitleSize;
                graphics.DrawRect(rect, this.Style.Colors.WindowTitle, this.Style.WindowRounding);

                // Title text.
                rect.Inflate(-this.Style.Padding, 0);
                graphics.DrawText(rect, this.Style.Fonts.Default, this.Style.Colors.Text, UIAlign.LeftMiddle, false, false, this.Caption);

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
                        dragArea.Height = this.Style.WindowTitleSize;
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

            var margin = new SizeF(this.Style.Padding, -this.Style.WindowTitleSize + 4);
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