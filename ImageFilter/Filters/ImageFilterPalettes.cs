using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageFilter
{
    public class ImageFilterPalettes : ImageFilterNoKernel
    {
        private readonly List<Color[]> palettes = new List<Color[]>();

        private Color[] palette;

        public ImageFilterPalettes()
            : base()
        {
        }

        public void AddPalette(Color[] palette)
        {
            this.palettes.Add(palette);
            this.MaxValue = this.palettes.Count;
        }

        public void AddPalette(params int[] colors)
        {
            var palette = colors
                .Select(c => Color.FromArgb(c))
                .ToArray();

            this.AddPalette(palette);
        }

        public override bool HasEffect() => this.Value > 0;

        public override void Prepare()
        {
            this.palette = this.palettes.Count > 0 ?
                this.palettes[this.Value - 1] :
                new[] { Color.White, Color.Black };
        }

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            var index = 0;
            int maxDistance = int.MaxValue;

            for (var i = 0; i < palette.Length; i++)
            {
                var c = palette[i];

                var diffR = ((c.R - r) * 19595) >> 16;
                var diffG = ((c.G - g) * 38470) >> 16;
                var diffB = ((c.B - b) * 7471) >> 16;

                var distance = diffR * diffR + diffG * diffG + diffB * diffB;

                if (distance == 0)
                {
                    index = i;
                    break;
                }

                if (distance < maxDistance)
                {
                    index = i;
                    maxDistance = distance;
                }
            }

            var color = palette[index];
            r = color.R;
            g = color.G;
            b = color.B;
        }
    }
}
