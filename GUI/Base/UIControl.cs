namespace GUI
{
    using GUI.Helpers;

    using System;
    using System.Linq;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Collections.Generic;

    public partial class UIControl : UINode
    {
        private static UIStyle style = new UIStyle();

        private static UIControl activePopupControl = null;

        private static UIControl activeControl = null;

        private static UIControl hoverControl = null;

        private static bool isDoubleClick = false;

        private SizeF size;

        private bool visible = true;

        private bool enabled = true;

        private bool autoSize = false;

        private PointF dragOffset;

        private Dictionary<string, Color> customColors = new Dictionary<string, Color>();

        protected UIStyle Style => style;

        protected UIColors Colors => this.Style.Colors;

        protected UIFonts Fonts => this.Style.Fonts;

        public UIControl() : base()
        {
        }

        public event EventHandler OnResize = delegate { };

        public event EventHandler OnVisibleChanged = delegate { };

        public event EventHandler OnEnabledChanged = delegate { };

        public event EventHandler OnMouseClick = delegate { };

        public event EventHandler OnMouseDoubleClick = delegate { };

        public event EventHandler OnMouseOver = delegate { };

        public event EventHandler OnMouseOut = delegate { };

        public event EventHandler<UIMouseEventArgs> OnMouseDown;

        public event EventHandler<UIMouseEventArgs> OnMouseMove;

        public event EventHandler<UIMouseEventArgs> OnMouseUp;

        public event EventHandler<UIMouseEventArgs> OnMouseWheel;

        public virtual string Caption { get; set; }

        public virtual string HintText { get; set; }

        public bool HandleMouseEvents { get; set; } = true;

        public UIAnchors Anchors { get; set; } = UIAnchors.Left | UIAnchors.Top;

        public float ExtraSize { get; set; }

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
                    this.UpdateMinimumSize();
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

        protected PointF LocalPosition { get; private set; }

        public PointF ScreenPosition { get; private set; }

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

        public bool ClipContent { get; set; }

        public UIControl PopupControl { get; set; }

        public bool IsMouseOver => hoverControl == this;

        public UIControl this[string name] => this.Find<UIControl>(name);

        public UIControl SetSize(float width, float height)
        {
            this.Size = new SizeF(width, height);
            return this;
        }

        public UIControl SetBounds(PointF location, SizeF size)
        {
            this.SetPosition(location);
            this.Size = size;
            return this;
        }

        public UIControl SetBounds(float x, float y, float width, float height)
        {
            return this.SetPosition(x, y)
                       .SetSize(width, height);
        }

        public UIControl SetPosition(PointF value)
        {
            this.LocalPosition = value;
            this.ScreenPosition = value;

            if (this.Parent is UIControl control)
            {
                while (control != null)
                {
                    var pos = control.LocalPosition;
                    this.ScreenPosition += pos.ToSize();
                    control = control.Parent as UIControl;
                }
            }

            foreach (var child in this.GetChildren<UIControl>())
            {
                child.SetPosition(child.LocalPosition);
            }

            this.DoChangePosition();
            return this;
        }

        public UIControl SetPosition(float x, float y)
        {
            return this.SetPosition(new PointF(x, y));
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

        public UIControl SetPositionToCenterFrom(float x, float y)
        {
            return this.SetPositionToCenterFrom(new PointF(x, y));
        }

        public RectangleF ClientRect
        {
            get => new RectangleF(this.LocalPosition, this.Size);
        }

        public RectangleF ScreenRect
        {
            get => new RectangleF(this.ScreenPosition - new SizeF(this.ExtraSize, this.ExtraSize),
                                  this.Size + new SizeF(this.ExtraSize * 2, this.ExtraSize * 2));
        }

        public virtual RectangleF HitRect
        {
            get => this.ScreenRect;
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

        public void Render(Graphics graphics)
        {
            if (this.Visible && !this.NeedClipping)
            {
                this.Style.Colors.PushColors(this.customColors);
                this.DoRender(graphics);
                this.Style.Colors.PopColors();
                
                if (this.ClipContent)
                {
                    using (var clipPath = this.GetClipPath(graphics, this.ScreenRect))
                    {
                        graphics.SetClip(clipPath, CombineMode.Intersect);
                    }
                }

                foreach (var child in this.GetChildren<UIControl>())
                {
                    child.Render(graphics);
                }
                
                this.DoRenderOverlay(graphics);

                if (this.ClipContent)
                {
                    graphics.ResetClip();
                }
            }
        }

        public UIControl CustomColor(string name, Color value)
        {
            System.Diagnostics.Debug.Assert(!this.customColors.ContainsKey(name));
            this.customColors.Add(name, value);
            return this;
        }

        protected virtual SizeF GetMinimumSize()
        {
            return SizeF.Empty;
        }

        protected bool NeedClipping
        {
            get
            {
                if (this.Parent is UIControl parent)
                {
                     return !parent.ScreenRect.IntersectsWith(this.ScreenRect);
                }

                return false;
            }
        }

        protected virtual GraphicsPath GetClipPath(Graphics graphics, RectangleF rect)
        {
            return graphics.GetRectPath(rect, 0);
        }

        protected virtual void DoResize(SizeF prevSize)
        {
            this.OnResize(this, EventArgs.Empty);

            var delta = this.size - prevSize;

            foreach (var child in this.GetChildren<UIControl>())
            {
                child.ComputeSize(prevSize, delta);
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

        protected void UpdateMinimumSize()
        {
            if (this.AutoSize)
            {
                this.UpdateSize(this.GetMinimumSize());
            }
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

        protected void ComputeSize(SizeF prevSize, SizeF delta)
        {
            if (this.Anchors == (UIAnchors.Left | UIAnchors.Top))
            {
                return;
            }

            var clientRect = this.ClientRect;
            var left = clientRect.X;
            var top = clientRect.Y;
            var width = clientRect.Width;
            var height = clientRect.Height;

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

        public static void HandleMouseDown(UIControl control, UIMouseEventArgs e)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            activeControl = control.GetControlAt(e.Location);

            if (activePopupControl != null &&
                activePopupControl != activeControl &&
                !activePopupControl.IsParentOf(activeControl))
            {
                activePopupControl.Hide();
            }

            if (activeControl != null)
            {
                activeControl.DoMouseInput(e);
                isDoubleClick = e.Clicks > 1;
            }
        }

        public static void HandleMouseMove(UIControl control, UIMouseEventArgs e)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var overControl = control.GetControlAt(e.Location);

            if (overControl != hoverControl)
            {
                hoverControl?.DoMouseOverOut(e, false);
                overControl?.DoMouseOverOut(e, true);
                hoverControl = overControl;
            }

            if (activeControl != null)
            {
                activeControl.DoMouseInput(e);

                if (activeControl == overControl)
                {
                    return;
                }
            }

            if (overControl != null)
            {
                overControl.DoMouseInput(e);
            }
        }

        public static void HandleMouseUp(UIControl control, UIMouseEventArgs e)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var overControl = control.GetControlAt(e.Location);

            if (activePopupControl != null &&
                activePopupControl != overControl &&
                !activePopupControl.IsParentOf(overControl))
            {
                activePopupControl.Hide();
            }

            if (activeControl != null)
            {
                if (overControl == activeControl)
                {
                    if (isDoubleClick)
                    {
                        activeControl.DoMouseDoubleClick(e);
                    }
                    else
                    {
                        activeControl.DoMouseClick(e);
                    }
                }

                activeControl.DoMouseInput(e);
            }
        }

        public static void HandleMouseWheel(UIControl control, UIMouseEventArgs e)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var overControl = control.GetControlAt(e.Location);

            if (overControl != null)
            {
                overControl.DoMouseInput(e);
            }
        }

        public UIControl GetControlAt(PointF location)
        {
            if (this.Visible && this.Enabled)
            {
                var children = this.GetChildren<UIControl>().Reverse().Where(c => c.Visible);

                foreach (var child in children)
                {
                    var control = child.GetControlAt(location);

                    if (control != null)
                    {
                        return control;
                    }
                }

                if (this.HandleMouseEvents && this.HitTest(location))
                {
                    return this;
                }
            }

            return null;
        }

        public UIControl GetControlAt(float x, float y)
        {
            return this.GetControlAt(new PointF(x, y));
        }

        public bool HitTest(PointF location)
        {
            return this.HitRect.Contains(location);
        }

        public bool HitTest(float x, float y)
        {
            return this.HitTest(new PointF(x, y));
        }

        public void StarDrag(PointF location)
        {
            var pos = this.ScreenPosition;
            this.dragOffset = new PointF(location.X - pos.X, location.Y - pos.Y);
        }

        public void Drag(PointF location)
        {
            var relPos = this.LocalPosition;
            var absPos = this.ScreenPosition;
            var diff = new PointF(location.X - absPos.X, location.Y - absPos.Y);
            this.SetPosition(new PointF(relPos.X + diff.X - this.dragOffset.X, relPos.Y + diff.Y - this.dragOffset.Y));
        }

        protected virtual void DoMouseClick(UIMouseEventArgs e)
        {
            this.OnMouseClick(this, e);
        }

        protected virtual void DoMouseDoubleClick(UIMouseEventArgs e)
        {
            this.OnMouseDoubleClick(this, e);
        }

        protected virtual void DoMouseInput(UIMouseEventArgs e)
        {
            if (this.PopupControl != null &&
                e.EventType == UIMouseEventType.Up &&
                e.Button == UIMouseButtons.Right)
            {
                this.PopupControl.Popup(e.Location);
            }

            var events = new Dictionary<UIMouseEventType, EventHandler<UIMouseEventArgs>>
            {
                { UIMouseEventType.Down, this.OnMouseDown },
                { UIMouseEventType.Move, this.OnMouseMove },
                { UIMouseEventType.Up, this.OnMouseUp },
                { UIMouseEventType.Wheel, this.OnMouseWheel }
            };

            events[e.EventType]?.Invoke(this, e);

            if (this.Parent is UIControl parent && parent.HandleMouseEvents)
            {
                parent.DoMouseInput(e);
            }
        }

        protected virtual void DoMouseOverOut(UIMouseEventArgs e, bool isOver)
        {
            if (isOver)
            {
                this.OnMouseOver(this, e);
                this.NotifyRoot(UINotification.MouseOver);
            }
            else
            {
                this.OnMouseOut(this, e);
                this.NotifyRoot(UINotification.MouseOut);
            }
        }
    }
}