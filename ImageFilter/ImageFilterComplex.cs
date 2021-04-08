namespace ImageFilter
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class ImageFilterComplex : IImageFilter
    {
        private readonly List<IImageFilter> filters;

        private IList<IImageFilter> activeFilters = new List<IImageFilter>();

        public ImageFilterComplex()
        {
            this.filters = new List<IImageFilter>();
        }

        public event EventHandler OnPropertyChanged = delegate { };

        public int Value { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public void Add(IImageFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (filter is ImageFilterBase filterBase)
            {
                filterBase.OnValueChanged += DoFilterValueChanged;
            }

            this.filters.Add(filter);
        }

        public bool HasEffect()
        {
            return this.filters.Any(f => f.HasEffect());
        }

        public byte GetKernelSize()
        {
            return this.filters.Where(x => x.HasEffect()).Max(x => x.GetKernelSize());
        }

        public void Prepare()
        {
            this.activeFilters = this.filters.Where(f => f.HasEffect()).ToList();

            foreach (var filter in this.activeFilters)
            {
                filter.Prepare();
            }
        }

        public void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            foreach (var filter in this.activeFilters)
            {
                filter.RGB(ref r, ref g, ref b, kernel, x, y);
            }
        }

        private void DoFilterValueChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged?.Invoke(sender, e);
        }
    }
}
