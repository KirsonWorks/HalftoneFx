namespace GUI
{
    using System;
    using System.Drawing;

    public enum UINotification
    {
        EnterTree,
        ExitTree,
        ParentChanged,
        MouseOver,
        MouseOut,
    }

    [Flags]
    public enum UIAnchors
    {
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
    }

    public struct UIAlign
    {
        public static PointF None = PointF.Empty;

        public static PointF LeftTop => new PointF(0, 0);

        public static PointF LeftMiddle => new PointF(0, 0.5F);

        public static PointF LeftBottom => new PointF(0, 1);

        public static PointF MiddleTop => new PointF(0.5F, 0);

        public static PointF Center => new PointF(0.5F, 0.5F);

        public static PointF MiddleBottom => new PointF(0.5F, 1);

        public static PointF RightTop => new PointF(1, 0);

        public static PointF RightMiddle => new PointF(1, 0.5F);

        public static PointF RightBottom => new PointF(1, 1);
    }
}
