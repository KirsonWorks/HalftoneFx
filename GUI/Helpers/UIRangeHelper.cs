namespace GUI.Controls
{
    using GUI.BaseControls;

    public static class UIRangeHelper
    {
        public static void Setup(this UIRangeControl range, float min, float max, float step, float value)
        {
            range.Min = min;
            range.Max = max;
            range.Step = step;
            range.Value = value;
        }
    }
}
