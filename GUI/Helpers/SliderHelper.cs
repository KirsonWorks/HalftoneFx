namespace GUI.Controls
{
    public static class SliderHelper
    {
        public static void Setup(this UISlider slider, float min, float max, float step, float value)
        {
            slider.Min = min;
            slider.Max = max;
            slider.Step = step;
            slider.Value = value;
        }
    }
}
