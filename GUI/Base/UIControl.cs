﻿namespace GUI
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

        private static UIControl hoveredControl = null;

        private static bool isDoubleClick = false;

        private SizeF size;

        private bool visible = true;

        private bool enabled = true;

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

        public virtual bool AutoSize { get; set; }

        public bool HandleEvents { get; set; } = true;

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

        public SizeF Size
        {
            get
            {
                if (this.AutoSize)
                {
                    var maxSize = this.GetFittedSize();

                    foreach (var child in this.GetChildren<UIControl>())
                    {
                        var ps = child.LocalPosition + child.Size;
                        maxSize = new SizeF(Math.Max(ps.X, maxSize.Width), Math.Max(ps.Y, maxSize.Height));
                    }

                    return maxSize;
                }

                return this.size;
            }

            set
            {
                if (this.size != value)
                {
                    var deltaSize = value - this.size;
                    this.size = value;
                    this.DoResize(deltaSize);

                    if (this.Parent is UIControl parent && parent.AutoSize)
                    {
                        parent.DoResize(deltaSize);
                    }
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

        public UIControl SetCenterPos(PointF pos)
        {
            var size = this.Size;
            return this.SetPosition(pos.X - (size.Width / 2), pos.Y - (size.Height / 2));
        }

        public UIControl SetCenterPos(float x, float y)
        {
            return this.SetCenterPos(new PointF(x, y));
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
                    this.ScreenPosition += new SizeF(pos.X, pos.Y);
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

        public RectangleF ScreenRect
        {
            get => new RectangleF(this.ScreenPosition - new SizeF(this.ExtraSize, this.ExtraSize),
                                  this.Size + new SizeF(this.ExtraSize * 2, this.ExtraSize * 2));
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

        public void Popup(PointF location)
        {
            activePopupControl?.Hide();
            activePopupControl = this;

            this.BringToFront();
            this.SetPosition(location);
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

        protected virtual SizeF GetFittedSize()
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

        protected virtual void DoResize(SizeF deltaSize)
        {
            this.OnResize(this, EventArgs.Empty);

            foreach (var child in this.GetChildren<UIControl>())
            {
                child.DoParentResize(deltaSize);
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

        protected virtual void DoParentResize(SizeF deltaSize)
        {
        }

        protected virtual void DoChangePosition()
        {
        }

        public static bool HandleMouseDown(UIControl control, UIMouseEventArgs e)
        {
            if (control == null)
            {
                return false;
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
                return true;
            }

            return false;
        }

        public static bool HandleMouseMove(UIControl control, UIMouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (control == null)
            {
                return false;
            }

            var overControl = control.GetControlAt(e.Location);

            if (overControl != hoveredControl)
            {
                hoveredControl?.DoMouseOverOut(e, false);
                overControl?.DoMouseOverOut(e, true);
                hoveredControl = overControl;
            }

            if (activeControl != null)
            {
                activeControl.DoMouseInput(e);

                if (activeControl == overControl)
                {
                    return true;
                }
            }

            if (overControl != null)
            {
                overControl.DoMouseInput(e);
                return true;
            }

            return false;
        }

        public static bool HandleMouseUp(UIControl control, UIMouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (control == null)
            {
                return false;
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
                return true;
            }

            return false;
        }

        public static bool HandleMouseWheel(UIControl control, UIMouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (control == null)
            {
                return false;
            }

            var overControl = control.GetControlAt(e.Location);

            if (overControl != null)
            {
                overControl.DoMouseInput(e);
                return true;
            }

            return false;
        }

        public UIControl GetControlAt(PointF location)
        {
            if (this.HitTest(location))
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

                if (this.HandleEvents)
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
            return this.Visible && this.Enabled && this.ScreenRect.Contains(location);
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