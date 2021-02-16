namespace GUI.Render
{
    using System.Drawing;

    public enum UISelectionType
    {
        Rect,
        Circle,
        Angles
    }

    public static class UISelection
    {
        public static void Draw(UISelectionType type, Graphics graphics, Pen pen, Rectangle rect)
        {
            switch (type)
            {
                case UISelectionType.Rect:
                    graphics.DrawRectangle(pen, rect);
                    break;

                case UISelectionType.Circle:
                    graphics.DrawEllipse(pen, rect);
                    break;

                case UISelectionType.Angles:
                    int n = 5;
                    int l = rect.Left - 1;
                    int t = rect.Top - 1;
                    int b = rect.Bottom;
                    int r = rect.Right;

                    var lt = new Point[] { new Point(l, t + n), new Point(l, t), new Point(l + n, t) };
                    var lb = new Point[] { new Point(l, b - n), new Point(l, b), new Point(l + n, b) };
                    var rb = new Point[] { new Point(r - n, b), new Point(r, b), new Point(r, b - n) };
                    var rt = new Point[] { new Point(r - n, t), new Point(r, t), new Point(r, t + n) };

                    graphics.DrawLines(pen, lt);
                    graphics.DrawLines(pen, lb);
                    graphics.DrawLines(pen, rb);
                    graphics.DrawLines(pen, rt);
                    break;
            }
        }
    }
}
