namespace HalftoneFx.Models
{
    public struct Range<T>
    {
        public Range(T min, T max)
        {
            this.MinValue = min;
            this.MaxValue = max;
        }

        public T MinValue { get; set; }

        public T MaxValue { get; set; }
    }
}
