namespace GUI.Extension
{
    using GUI.Common;

    public static class UIControlExtension
    {
        public static double Area(this UIControl control)
        {
            var sz = control.Size;
            return sz.Width * sz.Height;
        }

        public static double Distance(this UIControl control, UIControl other)
        {
            return UIMath.Distance(control.ScreenPositionCenter, other.ScreenPositionCenter);
        }
    }
}
