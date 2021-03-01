using System;
using System.Threading;

namespace ImageFilter
{
    public class ImageFilterDithering : ImageFilterNoKernel
    {
        private readonly Mutex mutex = new Mutex();

        private int dimension = 0;

        private byte[] table;

        public ImageFilterDithering()
        {
            this.MaxValue = 3;
            this.table = this.GenerateTable(0);
        }

        public override bool HasEffect() => this.Value > 0;

        public override void Prepare()
        {
            this.table = this.GenerateTable(this.Value);
        }

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            var i = (x % this.dimension) + (y % this.dimension * this.dimension);

            if (i < this.table.Length)
            {
                var val = this.table[i];

                r = this.StepByte(r, val);
                g = this.StepByte(g, val);
                b = this.StepByte(b, val);
            }
        }

        private byte[] GenerateTable(int n)
        {
            this.dimension = 1 << n;
            var size = this.dimension * this.dimension;
            var table = new byte[size];

            for (var y = 0; y < this.dimension; y++)
            {
                for (var x = 0; x < this.dimension; x++)
                {
                    int value = 0;
                    int mask = n - 1;
                    int xc = x ^ y;
                    int yc = y;

                    for (int bit = 0; bit < 2 * n; --mask)
                    {
                        value |= ((yc >> mask) & 1) << bit++;
                        value |= ((xc >> mask) & 1) << bit++;
                    }

                    table[x + (y * this.dimension)] = (byte)((value + 1) * 256 / size - 1);
                }
            }

            return table;
        }
    }
}
