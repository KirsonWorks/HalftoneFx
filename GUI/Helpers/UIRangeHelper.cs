namespace KWUI.Controls
{
    using KWUI.BaseControls;

    public static class UIRangeHelper
    {
        public static void Setup(this UIRangeControl range, float min, float max, float step)
        {
            range.Min = min;
            range.Max = max;
            range.Step = step;
        }

        public static void Setup(this UIRangeControl range, float min, float max, float step, float value)
        {
            range.Setup(min, max, step);
            range.Value = value;
        }
    }
}
