namespace ImageFilter
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class ImageFilterPalette : ImageFilterNoKernel
    {
        private float ditherAmount = 0;

        private int ditherMethod = 0;

        private BayerMatrix matrix = new BayerMatrix(0);

        private Color[] palette = new[] { Color.Black, Color.White };

        public ImageFilterPalette()
            : base()
        {
            this.MaxValue = 1;
        }

        public int DitherMethod
        {
            get => this.ditherMethod;

            set
            {
                value = Math.Min(value, this.DitherMethodMax);

                if (this.ditherMethod != value)
                {
                    this.ditherMethod = value;

                    if (this.DitherAmount > float.Epsilon)
                    {
                        this.DoValueChanged();
                    }

                    this.matrix = new BayerMatrix(value);
                }
            }
        }

        public int DitherMethodMax => 3;

        public float DitherAmount
        {
            get => this.ditherAmount;

            set
            {
                value = this.Clamp(value, 0.0f, 1.0f);

                if (Math.Abs(this.ditherAmount - value) > float.Epsilon)
                {
                    this.ditherAmount = value;
                    this.DoValueChanged();
                }
            }
        }

        public void SetPalette(IEnumerable<Color> palette)
        {
            if (palette == null)
            {
                throw new ArgumentNullException(nameof(palette));
            }

            this.palette = palette.ToArray();
        }

        public override bool HasEffect() => this.Value > 0;

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            var index = 0;
            int maxDistance = int.MaxValue;

            if (this.DitherMethod > 0 && this.ditherAmount > float.Epsilon)
            {
                var d = (byte)(this.matrix[x, y] * this.ditherAmount);
                r = this.ClampByte(r + d);
                g = this.ClampByte(g + d);
                b = this.ClampByte(b + d);
            }

            for (var i = 0; i < this.palette.Length; i++)
            {
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

            var color = this.palette[index];
            r = color.R;
            g = color.G;
            b = color.B;
        }

        public override IImageFilter Clone()
        {
            var clone = (ImageFilterPalette)base.Clone();
            clone.matrix = (BayerMatrix)this.matrix.Clone();
            return clone;
        }
    }
}
