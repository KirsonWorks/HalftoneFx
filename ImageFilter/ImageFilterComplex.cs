namespace ImageFilter
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class ImageFilterComplex: IImageFilter
    {
        private readonly Dictionary<string, IImageFilter> filters;

        private IList<IImageFilter> activeFilters = new List<IImageFilter>();

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

        public byte GetKernelSize()
        {
            return this.filters.Values.Where(x => x.HasEffect()).Max(x => x.GetKernelSize());
        }

        public void Prepare()
        {
            this.activeFilters = this.filters.Values.Where(f => f.HasEffect()).ToList();

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
