namespace KWUI
{
    using KWUI.Helpers;

    using System;
    using System.Linq;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public partial class UIControl : UINode
    {
        private static UIControl activePopupControl = null;

        private SizeF size;

        private bool visible = true;

        private bool enabled = true;

        private bool autoSize = false;

        public UIControl()
            : base()
        {
        }

        public event EventHandler OnResize = delegate { };

        public event EventHandler OnVisibleChanged = delegate { };

        public event EventHandler OnEnabledChanged = delegate { };

        public virtual string Caption { get; set; }

        public virtual string HintText { get; set; }

        public bool ClipContent { get; set; }

        public UIControl PopupControl { get; set; }

        public UIAnchors Anchors { get; set; } = UIAnchors.Left | UIAnchors.Top;

        public PointF ScreenPosition { get; private set; } = PointF.Empty;
        
        public float Left
        {
            get => this.LocalPosition.X;
            set => this.SetPosition(new PointF(value, this.LocalPosition.Y));
        }

        public float Top
        {
            get => this.LocalPosition.Y;
            set => this.SetPosition(new PointF(this.LocalPosition.X, value));
        }

        public virtual bool AutoSize
        {
            get => this.autoSize;

            set
            {
                if (this.autoSize != value)
                {
                    this.autoSize = value;
                    this.UpdatePreferredSize();
                }
            }
        }

        public SizeF Size
        {
            get => this.size;

            set
            {
                if (!this.AutoSize)
                {
                    this.UpdateSize(value);
                }
            }
        }

        public float Width
        {
            get => this.Size.Width;
            set => this.Size = new SizeF(value, this.size.Height);
        }

        public float Height
        {
            get => this.Size.Height;
            set => this.Size = new SizeF(this.size.Width, value);
        }

        public bool Visible
        {
            get => this.visible;

            set
            {
                if (this.visible != value)
                {
                    this.visible = value;
                    this.OnVisibleChanged(this, EventArgs.Empty);
                    this.DoChangeVisibility();
                }
            }
        }

        public bool Enabled
        {
            get => this.enabled;

            set
            {
                if (this.enabled != value)
                {
                    this.enabled = value;
                    this.OnEnabledChanged(this, EventArgs.Empty);
                    this.DoChangeEnabled();
                }
            }
        }

        public PointF ScreenPositionCenter
        {
            get
            {
                var halfSize = this.Size;
                halfSize.Width /= 2;
                halfSize.Height /= 2;
                return this.ScreenPosition + halfSize;
            }
        }

        public RectangleF BoundsRect
        {
            get => new RectangleF(this.LocalPosition, this.Size);
        }

        public RectangleF ScreenRect
        {
            get => new RectangleF(this.ScreenPosition, this.Size);
        }

        public virtual RectangleF ClientRect
        {
            get => new RectangleF(PointF.Empty, this.Size);
        }
        
        protected PointF LocalPosition { get; private set; } = PointF.Empty;
        
        protected virtual RectangleF ClipRect
        {
            get
            {
                var rect = this.ClientRect;
                rect.Location += this.ScreenPosition.ToSize();
                rect.Inflate(1, 1);
                return rect;
            }
        }
       
        protected bool CanRender
        {
            get
            {
                if (this.Parent is UIControl parent)
                {
                    return parent.ScreenRect.IntersectsWith(this.ScreenRect);
                }

                return true;
            }
        }

        public virtual void Show()
        {
            this.Visible = true;
        }

        public virtual void Hide()
        {
            this.Visible = false;

            if (activePopupControl == this)
            {
                activePopupControl = null;
            }
        }

        public void BringToFront()
        {
            if (this.Parent != null)
            {
                var count = this.Parent.GetChildrenCount();
                this.Parent.MoveNode(this, count - 1);
            }
        }

        public void SendToBack()
        {
            if (this.Parent != null)
            {
                this.Parent.MoveNode(this, 0);
            }
        }

        public void Popup(PointF global)
        {
            activePopupControl?.Hide();
            activePopupControl = this;

            this.BringToFront();
            this.SetGlobalPosition(global);
            this.Show();
        }

        public UIControl SetSize(SizeF value)
        {
            this.Size = value;
            return this;
        }

        public UIControl SetSize(float width, float height)
        {
            return this.SetSize(new SizeF(width, height));
        }

        public UIControl SetBounds(PointF location, SizeF size)
        {
            return this.SetPosition(location)
                       .SetSize(size);
        }

        public UIControl SetBounds(float x, float y, float width, float height)
        {
            return this.SetPosition(x, y)
                       .SetSize(width, height);
        }
        
        public UIControl SetPosition(float x, float y)
        {
            return this.SetPosition(new PointF(x, y));
        }

        public UIControl SetPosition(PointF value)
        {
            this.LocalPosition = value;
            this.ScreenPosition = value;

            if (this.Parent is UIControl parent)
            {
                while (parent != null)
                {
                    var pos = parent.LocalPosition + parent.ClientRect.Location.ToSize();
                    this.ScreenPosition += pos.ToSize();
                    parent = parent.Parent as UIControl;
                }
            }

            foreach (var child in this.GetChildren<UIControl>())
            {
                child.SetPosition(child.LocalPosition);
            }

            this.DoChangePosition();
            return this;
        }

        public UIControl SetGlobalPosition(PointF value)
        {
            if (this.Parent is UIControl parent)
            {
                var pos = parent.ScreenPosition;
                this.SetPosition(value.X - pos.X, value.Y - pos.Y);
            }
            else
            {
                this.SetPosition(value);
            }

            return this;
        }

        public UIControl SetPositionToCenterFrom(PointF pos)
        {
            var size = this.Size;
            return this.SetPosition(pos.X - (size.Width / 2), pos.Y - (size.Height / 2));
        }

        public void Render(Graphics graphics)
        {
            if (this.Visible && this.CanRender)
            {
                this.Style.Colors.PushColors(this.customColors);
                this.DoRender(graphics);
                this.Style.Colors.PopColors();
                
                if (this.ClipContent)
                {
                    using (var clipPath = this.GetClipPath(graphics, this.ClipRect))
                    {
                        graphics.SetClip(clipPath, CombineMode.Intersect);
                    }
                }

                foreach (var child in this.GetChildren<UIControl>())
                {
                    child.Render(graphics);
                }

                if (this.ClipContent)
                {
                    graphics.ResetClip();
                }

                this.DoRenderOverlay(graphics);
            }
        }

        protected virtual GraphicsPath GetClipPath(Graphics graphics, RectangleF rect)
        {
            return graphics.GetRectPath(rect, 0);
        }

        protected virtual SizeF GetPreferredSize()
        {
            return SizeF.Empty;
        }

        protected void UpdateSize(SizeF value)
        {
            if (this.size != value)
            {
                var prevSize = this.size;
                this.size = value;
                this.DoResize(prevSize);
            }
        }

        protected void UpdatePreferredSize()
        {
            if (this.AutoSize)
            {
                this.UpdateSize(this.GetPreferredSize());
            }
        }

        protected void ComputeAnchorableSize(SizeF prevSize, SizeF delta)
        {
            if (this.Anchors == (UIAnchors.Left | UIAnchors.Top))
            {
                return;
            }

            var rect = this.BoundsRect;
            var left = rect.X;
            var top = rect.Y;
            var width = rect.Width;
            var height = rect.Height;

            prevSize = prevSize.Max(new SizeF().OneValue(1));

            switch (this.Anchors & (UIAnchors.Left | UIAnchors.Right))
            {
                case UIAnchors.Left | UIAnchors.Right:
                    width += delta.Width;
                    break;

                case UIAnchors.Right:
                    left += delta.Width;
                    break;

                case UIAnchors.None:
                    left += (left + width / 2) / prevSize.Width * delta.Width;
                    break;
            }

            switch (this.Anchors & (UIAnchors.Top | UIAnchors.Bottom))
            {
                case UIAnchors.Top | UIAnchors.Bottom:
                    height += delta.Height;
                    break;

                case UIAnchors.Bottom:
                    top += delta.Height;
                    break;

                case UIAnchors.None:
                    top += (top + height / 2) / prevSize.Height * delta.Height;
                    break;
            }

            this.SetBounds(left, top, width, height);
        }

        protected override void DoParentChanged()
        {
            if (this.ScreenPosition.IsEmpty)
            {
                this.SetPosition(0, 0);
            }
        }

        protected virtual void DoResize(SizeF prevSize)
        {
            this.OnResize(this, EventArgs.Empty);

            var delta = this.size - prevSize;

            foreach (var child in this.GetChildren<UIControl>())
            {
                child.ComputeAnchorableSize(prevSize, delta);
                child.DoParentResize();
            }
        }

        protected virtual void DoRender(Graphics graphics)
        {
        }

        protected virtual void DoRenderOverlay(Graphics graphics)
        {
        }

        protected virtual void DoChangeVisibility()
        {
        }

        protected virtual void DoChangeEnabled()
        {
        }

        protected virtual void DoParentResize()
        {
        }

        protected virtual void DoChangePosition()
        {
        }
    }
}