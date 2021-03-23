namespace HalftoneFx.Helpers
{
    using HalftoneFx.Models;
    
    using ImageFilter;
    
    public static class IImageFilterHelper
    {
        public static Range<int> GetRange(this IImageFilter filter)
        {
            return new Range<int>
            {
                MinValue = filter.MinValue,
                MaxValue = filter.MaxValue,
            };
        }
    }
}
