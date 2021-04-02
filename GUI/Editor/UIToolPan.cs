namespace KWUI.Editor
{
    using KWUI;
    using KWUI.Helpers;
    using System;
    using System.Drawing;

    public class UIToolPan
    {
        public bool Active { get; set; }

        public UIControl Control { get; set; }

        public void Start(PointF location)
        {
            if (!this.Active && this.Control != null)
            {
                this.Active = true;
                this.Control.StarDrag(location);
            }
        }

        public void Move(PointF location)
        {
            if (this.Active && this.Control != null)
            {
                this.Control.Drag(location);
            }
        }

        public void Reset(PointF location)
        {
            this.Control?.SetPositionToCenterFrom(location);
        }
    }
}
