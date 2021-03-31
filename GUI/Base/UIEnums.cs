namespace GUI
{
    using System;
    using System.Drawing;

    public enum UINotification
    {
        AddNode,
        RemoveNode,
        ParentChanged,
        MouseOver,
        MouseOut,
    }

    [Flags]
    public enum UIAnchors
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
        All = Left | Top | Right | Bottom,
    }

    public struct UIAlign
    {
        public static PointF None = PointF.Empty;

        public static PointF LeftTop => new PointF(0, 0);

        public static PointF LeftMiddle => new PointF(0, 0.5f);

        public static PointF LeftBottom => new PointF(0, 1);

        public static PointF MiddleTop => new PointF(0.5f, 0);

        public static PointF Center => new PointF(0.5f, 0.5f);

        public static PointF MiddleBottom => new PointF(0.5f, 1);

        public static PointF RightTop => new PointF(1, 0);

        public static PointF RightMiddle => new PointF(1, 0.5f);

        public static PointF RightBottom => new PointF(1, 1);
    }
}
