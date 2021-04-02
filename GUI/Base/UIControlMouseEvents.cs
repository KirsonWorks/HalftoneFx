namespace KWUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public partial class UIControl : UINode
    {
        private static UIControl activeControl = null;

        private static UIControl hoverControl = null;

        private static bool isDoubleClick = false;
        
        private PointF dragOffset;

        public event EventHandler OnMouseClick = delegate { };

        public event EventHandler OnMouseDoubleClick = delegate { };

        public event EventHandler OnMouseOver = delegate { };

        public event EventHandler OnMouseOut = delegate { };

        public event EventHandler<UIMouseEventArgs> OnMouseDown;

        public event EventHandler<UIMouseEventArgs> OnMouseMove;

        public event EventHandler<UIMouseEventArgs> OnMouseUp;

        public event EventHandler<UIMouseEventArgs> OnMouseWheel;

        public bool HandleMouseEvents { get; set; } = true;

        public bool IsMouseOver => hoverControl == this;

        public UIControl GetControlAt(PointF location)
        {
            if (this.Visible && this.Enabled && this.HitTest(location))
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

                if (this.HandleMouseEvents)
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
            return this.ScreenRect.Contains(location);
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
    }
}
