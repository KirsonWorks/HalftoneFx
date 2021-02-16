namespace GUI.Controls
{
    using System;
    using System.Drawing;

    [Serializable]
    public class UIHorizontalLine : UIControl
    {
        public UIHorizontalLine() : base()
        {
            this.Size = new SizeF(50, 1F);
        }

        protected override void DoRender(Graphics graphics)
        {
            //var a = this.GetAlignPosition(this.Size);
            //var b = new PointF(a.X + this.Size.Width, a.Y);
            //graphics.DrawLine(new Pen(this.Style.Colors.Line, this.Size.Height), a, b);
        }
    }
}
