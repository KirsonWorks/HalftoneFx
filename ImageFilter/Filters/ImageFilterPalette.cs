namespace ImageFilter
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class ImageFilterPalette : ImageFilterNoKernel
    {
        private bool isPaletteChanged;

        private IEnumerable<Color> newPalette;

        private Color[] palette = new[] { Color.Black, Color.White };

        public ImageFilterPalette()
            : base()
        {
            this.MaxValue = 1;
        }

        public void SetPalette(IEnumerable<Color> palette)
        {
            this.newPalette = palette ?? throw new ArgumentNullException(nameof(palette));
            this.isPaletteChanged = true;
        }

        public override bool HasEffect() => this.Value > 0;

        public override void Prepare()
        {
            if (this.isPaletteChanged)
            {
                this.palette = this.newPalette.ToArray();
                this.isPaletteChanged = false;
            }
        }

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            var index = 0;
            int maxDistance = int.MaxValue;

            for (var i = 0; i < this.palette.Length; i++)
            {
                if (this.isPaletteChanged)
                {
                    return;
                }

                var c = this.palette[i];
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

            //if (index < this.palette.Length) // I don't want to use the lockers for profit.
            {
                var color = this.palette[index];
                r = color.R;
                g = color.G;
                b = color.B;
            }
        }
    }
}
