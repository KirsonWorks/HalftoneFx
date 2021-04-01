namespace GUI
{
    using GUI.Helpers;

    using System;
    using System.Drawing;
    using System.Linq;

    public class UILayoutWorkbench
    {
        private static PointF DefaultAlign = new PointF(0.0f, 0.5f);

        private readonly UILayoutOptions options;

        private PointF align = DefaultAlign;

        private bool isSameLine = false;

        private bool stretching = false;

        private PointF local = PointF.Empty;

        private PointF global = PointF.Empty;

        private SizeF cellSize = SizeF.Empty;

        private SizeF customCellSize = SizeF.Empty;

        private PointF minPos = new PointF(float.PositiveInfinity, float.PositiveInfinity);

        private PointF maxPos = PointF.Empty;

        public UILayoutWorkbench(UILayoutOptions options)
        {
            this.options = options;
            this.Translate(this.options.Margin);
        }

        public SizeF OverallSize
        {
            get
            {
                if (maxPos.IsEmpty)
                {
                    return SizeF.Empty;
                }

                var margin = this.options.Margin;
                margin += margin.ToSize();
                return this.maxPos.ToSize() - this.minPos.ToSize() + margin.ToSize();
            }
        }

        public void Align(PointF value)
        {
            this.align = value;
        }

        public void Stretch(float value = 0.0f)
        {
            this.stretching = true;
            this.Wide(value);
        }

        public void Wide(float value)
        {
            this.customCellSize = new SizeF(Math.Max(1.0f, value), this.customCellSize.Height);
        }

        public void Tall(float value)
        {
            this.customCellSize = new SizeF(this.customCellSize.Width, Math.Max(1.0f, value));
        }

        public void Indent(int count)
        {
            this.local.X += Math.Max(0, count) * this.options.Indent;
        }

        public void Translate(PointF location)
        {
            this.global = location;
            this.local = PointF.Empty;
        }

        public void LocalTranslate(PointF location)
        {
            this.local = location;
        }

        public void Offset(SizeF value)
        {
            this.local += value;
        }

        public void SameLine()
        {
            if (!this.isSameLine)
            {
                this.isSameLine = true;
                var offset = this.cellSize + this.options.Spacing;
                this.Offset(new SizeF(offset.Width, -offset.Height));
            }
        }

        public void Set(UIControl control)
        {
            if (control == null)
            {
                return;
            }

            if (this.stretching)
            {
                control.AutoSize = false;
            }

            if (!this.isSameLine)
            {
                this.LocalTranslate(new PointF(0, this.local.Y));
                this.cellSize = SizeF.Empty;
            }

            var cellWidth = this.customCellSize.Width >= 1.0f ? this.customCellSize.Width : this.options.CellWidth;
            var cellHeight = this.customCellSize.Height >= 1.0f ? this.customCellSize.Height : this.options.CellHeight;

            // Trying to set the size from options.
            control.SetSize(cellWidth, cellHeight);

            var innerSize = control.AutoSize ? SizeF.Empty : new SizeF(cellWidth, cellHeight);
            this.cellSize = this.cellSize.Max(control.Size.Max(innerSize));

            var x = this.local.X + (this.cellSize.Width - control.Width) * this.align.X;
            var y = this.local.Y + (this.cellSize.Height - control.Height) * this.align.Y;

            control.SetPosition(this.global.X + x, this.global.Y + y);

            var rect = control.BoundsRect;
            this.minPos = this.minPos.Min(rect.Location);
            this.maxPos = this.maxPos.Max(new PointF(rect.Right, rect.Bottom));

            this.NextLine();
        }

        private void NextLine()
        {
            this.isSameLine = false;
            this.stretching = false;
            this.align = DefaultAlign;
            this.customCellSize = SizeF.Empty;
            this.Offset(new SizeF(0, this.cellSize.Height + this.options.Spacing.Height));
        }

        public void NextLine(UILayoutWorkbench workbench)
        {
            this.Offset(new SizeF(0, -(this.cellSize.Height + this.options.Spacing.Height)));
            this.maxPos = this.maxPos.Max(this.global + this.local.ToSize() + workbench.OverallSize);
            this.cellSize = this.cellSize.Max(workbench.OverallSize);
            this.NextLine();
        }
    }
}
