namespace HalftoneFx.Helpers
{
    using KWUI.BaseControls;
    using HalftoneFx.Models;

    using System;

    public static class UIRangeHelper
    {
        public static void SetRange<T>(this UIRangeControl control, Range<T> range)
            where T : IConvertible
        {
            control.Min = range.MinValue.ToSingle(null);
            control.Max = range.MaxValue.ToSingle(null);
        }
    }
}
