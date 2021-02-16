namespace GUI.Editor
{
    using GUI;
    using GUI.Common;

    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Collections.Generic;

    public class UIEditor
    {
        private const int DragDeadZone = 3;

        private bool? dragging;

        private PointF dragStart;

        private readonly List<UIControl> selected = new List<UIControl>();

        public bool Enabled { get; set; } = true;

        public List<UIControl> Collection { get; set; }

        public byte SnapStep { get; set; } = 4;

        public event EventHandler OnSelect = delegate {};

        public object[] Selected
        {
            get
            {
                return this.selected.ToArray();
            }
        }

        public bool Select(PointF location)
        {
            if (this.Collection != null)
            {
                foreach (var item in this.Collection)
                {
                    var subItem = item.GetControlAt(location);

                    if (subItem != null)
                    {
                        return this.Select(subItem);
                    }
                }
            }

            return false;
        }

        public bool Select(UIControl element)
        {
            if (element != null)
            {
                var index = this.selected.IndexOf(element);

                if (index == -1)
                {
                    this.selected.Add(element);
                }
            }

            this.OnSelect(this, EventArgs.Empty);
            return this.selected.Count > 0;
        }

        public void StartDrag(PointF location, bool clear)
        {
            if (!this.Enabled)
            {
                return;
            }

            if (clear)
            {
                this.selected.Clear();
            }

            if (this.Select(location))
            {
                this.dragging = false;
                this.dragStart = location;
                this.selected.ForEach(e => e.StarDrag(location));
            }
        }

        public void Drag(PointF location)
        {
            if (this.Enabled && this.dragging != null && this.selected.Count > 0)
            {
                if (this.dragging == false)
                {
                    this.dragging = UIMath.Distance(location, this.dragStart) >= DragDeadZone;
                }
                else
                {
                    this.selected.ForEach(e => e.Drag(UIMath.Snap(location, this.SnapStep)));
                }
            }
        }

        public void EndDrag()
        {
            this.dragging = null;
        }

        public void Render(Graphics g)
        {
            if (!this.Enabled)
            {
                return;
            }

            this.Collection?.ForEach(x => x.Render(g));

            if (this.selected.Count > 0)
            {
                using (var pen = new Pen(Color.Gray))
                {
                    foreach (var e in this.selected)
                    {
                        pen.DashStyle = DashStyle.Dash;
                        g.DrawRectangle(pen, Rectangle.Round(e.ScreenRect));
                    }
                }
            }
        }
    }
}