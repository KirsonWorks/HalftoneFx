namespace HalftoneFx
{
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;

    public class ColorPalettes
    {
        private readonly Dictionary<string, Color[]> palettes
            = new Dictionary<string, Color[]>();

        public ColorPalettes()
        {
            this.Create("Apple II",
                0x000000, 0x6c2940, 0x403578, 0xd93cf0,
                0x135740, 0x808080, 0x2697f0, 0xbfb4f8,
                0x404b07, 0xd9680f, 0x808080, 0xeca8bf,
                0x26c30f, 0xbfca87, 0x93d6bf, 0xffffff);

            this.Create("CGA",
                0x000000, 0x0000aa, 0x00aa00, 0x00aaaa,
                0xaa0000, 0xaa00aa, 0xaa5500, 0xaaaaaa,
                0x555555, 0x5555ff, 0x55ff55, 0x55ffff,
                0xff5555, 0xff55ff, 0xffff55, 0xffffff);

            this.Create("CGA0",
                0x000000, 0x00aa00, 0xaa0000, 0xaa5500);

            this.Create("CGA1",
                0x000000, 0x00aaaa, 0xaa00aa, 0xaaaaaa);

            this.Create("Commodore 64",
                0x000000, 0xffffff, 0x883932, 0x67b6bd,
                0x8b3f96, 0x55a049, 0x40318d, 0xbfce72,
                0x8b5429, 0x574200, 0xb86962, 0x505050,
                0x787878, 0x94e089, 0x7869c4, 0x9f9f9f);

            this.Create("Game Boy",
                0x081820, 0x346856, 0x88c070, 0xe0f8d0);           

            this.Create("Classic",
                0x058789, 0x503d2e, 0xd54b1a, 0xe3a72f,
                0xf0ecc9);

            this.Create("HEPT32",
                0x000000, 0x180d2f, 0x353658, 0x686b72,
                0x8b97b6, 0xc5cddb, 0xffffff, 0x5ee9e9,
                0x2890dc, 0x1831a7, 0x053239, 0x005f41,
                0x08b23b, 0x47f641, 0xe8ff75, 0xfbbe82,
                0xde9751, 0xb66831, 0x8a4926, 0x461c14,
                0x1e090d, 0x720d0d, 0x813704, 0xda2424,
                0xef6e10, 0xecab11, 0xece910, 0xf78d8d,
                0xf94e6d, 0xc12458, 0x841252, 0x3d083b);

            this.Create("Retro",
                0x666547, 0xfb2e01, 0x6fcb9f, 0xffe28a,
                0xfffeb3);

            this.Create("Synthwave 9",
                0xf6eddb, 0xec8d75, 0xbd4b64, 0x9e2281,
                0x40265c, 0x1b1e23, 0x244584, 0x50a9cf,
                0x96e6c2);

            this.Create("ZX Spectrum",
                0x000000, 0x0000c0, 0xc00000, 0xc000c0,
                0x00c000, 0x00c0c0, 0xc0c000, 0xc0c0c0,
                0x000000, 0x0000ff, 0xff0000, 0xff00ff,
                0x00ff00, 0x00ffff, 0xffff00, 0xffffff);
        }

        public event EventHandler OnAdded = delegate { };

        public event EventHandler OnRemoved = delegate { };

        public int Count => this.palettes.Count;

        public void Create(string name, params int[] colors)
        {
            var palette = colors.Select(c => Color.FromArgb(c));
            this.Add(name, palette);
        }

        public void Add(string name, IEnumerable<Color> palette)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (palette == null)
            {
                throw new ArgumentNullException(nameof(palette));
            }

            if (this.palettes.ContainsKey(name))
            {
                throw new Exception("Palette with this name exists");
            }

            var array = palette.ToArray();

            if (array.Length > 0)
            {
                this.palettes.Add(name, array);
                this.OnAdded(this, EventArgs.Empty);
            }
        }

        public void Remove(string name)
        {
            this.palettes.Remove(name);
            this.OnRemoved(this, EventArgs.Empty);
        }

        public Color[] GetByIndex(int index)
        {
            if (index >= 0 && index < this.palettes.Count)
            {
                return this.palettes.Values.ElementAt(index);
            }

            throw new IndexOutOfRangeException();
        }

        public Color[] GetByName(string name)
        {
            if (this.palettes.TryGetValue(name, out Color[] palette))
            {
                return palette;
            }

            throw new Exception($"Palette {name} not found");
        }

        public IEnumerable<string> GetNames()
        {
            return this.palettes.Keys;
        }
    }
}
