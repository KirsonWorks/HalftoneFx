namespace ImageFilter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImageFilterComplex: IImageFilter
    {
        private readonly Dictionary<int, IImageFilter> filters;

        public event EventHandler OnValueChanged = delegate { };

        public ImageFilterComplex()
        {
            this.filters = new Dictionary<int, IImageFilter>();
        }

        public IImageFilter Add(int id, IImageFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (!this.filters.ContainsKey(id))
            {
                this.filters.Add(id, filter);
                return filter;
            }

            return this.filters[id];
        }

        public void SetValue(int id, int value)
        {
            var filter = this.GetFilter(id);

            if (filter != null && filter.Value != value)
            {
                filter.Value = value;
                this.OnValueChanged(this, EventArgs.Empty);
            }
        }

        public int GetValue(int id)
        {
            return this.GetFilter(id).Value;
        }

        public int this[int id]
        {
            get => this.GetValue(id);
            set => this.SetValue(id, value);
        }

        public bool HasEffect()
        {
            return this.filters.Values.Count(f => f.HasEffect()) > 0;
        }

        public void RGB(ref byte r, ref byte g, ref byte b)
        {
            foreach (var filter in this.filters.Values.Where(f => f.HasEffect()))
            {
                filter.RGB(ref r, ref g, ref b);
            }
        }

        private ImageFilterBase GetFilter(int id)
        {
            if (this.filters.ContainsKey(id) && this.filters[id] is ImageFilterBase filter)
            {
                return filter;
            }

            throw new Exception("Jesus, Mary and Joseph!");
        }
    }
}
