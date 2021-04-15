namespace Common
{
    using System;

    public class BayerMatrix : ICloneable
    {
        private readonly int dimension;

        private byte[] matrix;

        public BayerMatrix(int n)
        {
            n = Math.Max(0, n);
            this.dimension = 1 << n;
            var size = this.dimension * this.dimension;
            this.matrix = new byte[size];

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

                    this.matrix[x + (y * this.dimension)] = (byte)((value + 1) * 255 / size);
                }
            }
        }

        public byte this[int x, int y]
        {
            get
            {
                var index = (x % this.dimension) + (y % this.dimension * this.dimension);
                return (index >= 0 && index < this.matrix.Length) ? this.matrix[index] : (byte)0;
            }
        }

        public object Clone()
        {
            var clone = (BayerMatrix)this.MemberwiseClone();
            this.matrix.CopyTo(clone.matrix, 0);
            return clone;
        }
    }
}
