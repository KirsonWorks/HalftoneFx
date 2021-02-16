namespace GUI.Common
{
    using System;

    public class UIRange<T> where T : IComparable<T>
    {
        private T value;

        private T min;

        private T max;

        private T step;

        public T Min
        {
            get => this.min;

            set
            {
                this.min = value;

                if (this.value.CompareTo(this.min) < 0)
                {
                    this.Value = this.min;
                }
            }
        }

        public T Max
        {
            get => this.max;

            set
            {
                this.max = value;

                if (this.value.CompareTo(this.max) > 0)
                {
                    this.Value = this.max;
                }
            }
        }

        public T Step
        {
            get => this.step;
            set => this.step = value;
        }

        public T Value
        {
            get => this.value;
            set => this.value = UIMath.Clamp(value, this.Min, this.Max);
        }

        public bool IsSameValue(T other)
        {
            return this.value.CompareTo(other) == 0;
        }
    }
}
