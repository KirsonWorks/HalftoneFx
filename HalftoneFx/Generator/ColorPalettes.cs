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
            this.Create("CGA",
                0x000000, 0x0000aa, 0x00aa00, 0x00aaaa,
                0xaa0000, 0xaa00aa, 0xaa5500, 0xaaaaaa,
                0x555555, 0x5555ff, 0x55ff55, 0x55ffff,
                0xff5555, 0xff55ff, 0xffff55, 0xffffff);

            this.Create("CGA0",
                0x000000, 0x00aa00, 0xaa0000, 0xaa5500);

            this.Create("CGA1",
                0x000000, 0x00aaaa, 0xaa00aa, 0xaaaaaa);

            this.Create("Game Boy",
                0x081820, 0x346856, 0x88c070, 0xe0f8d0);

            this.Create("Paradise",
                0x058789, 0x503d2e, 0xd54b1a, 0xe3a72f,
                0xf0ecc9);

            this.Create("Retro",
                0x666547, 0xfb2e01, 0x6fcb9f, 0xffe28a,
                0xfffeb3);
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
