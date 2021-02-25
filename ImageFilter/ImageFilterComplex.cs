namespace ImageFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImageFilterComplex: IImageFilter
    {
        private readonly Dictionary<string, IImageFilter> filters;

        private byte maxKernelSize = 0;

        public event EventHandler OnPropertyChanged = delegate { };

        public ImageFilterComplex()
        {
            this.filters = new Dictionary<string, IImageFilter>();
        }

        public void Add(string name, IImageFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (!this.filters.ContainsKey(name))
            {
                this.filters.Add(name, filter);
                this.maxKernelSize = Math.Max(this.maxKernelSize, filter.GetKernelSize());
            }
        }

        public void SetValue(string name, int value)
        {
            var filter = this.GetFilter(name);

            if (filter != null && filter.Value != value)
            {
                filter.Value = value;
                this.OnPropertyChanged(this, EventArgs.Empty);
            }
        }

        public int GetValue(string name)
        {
            return this.GetFilter(name).Value;
        }

        public int this[string name]
        {
            get => this.GetValue(name);
            set => this.SetValue(name, value);
        }

        public bool HasEffect()
        {
            return this.filters.Values.Any(f => f.HasEffect());
        }

        public byte GetKernelSize() => this.maxKernelSize;

        public void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            foreach (var filter in this.filters.Values.Where(f => f.HasEffect()))
            {
                filter.RGB(ref r, ref g, ref b, kernel, x, y);
            }
        }

        private ImageFilterBase GetFilter(string name)
        {
            if (this.filters.ContainsKey(name) && this.filters[name] is ImageFilterBase filter)
            {
                return filter;
            }

            throw new Exception("Jesus, Mary and Joseph!");
        }
    }
}
