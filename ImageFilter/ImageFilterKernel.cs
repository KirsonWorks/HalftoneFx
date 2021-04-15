namespace ImageFilter
{
    using System;

    public class ImageFilterKernel : ImageFilterBase, IImageFilter
    {
        private byte kernelSize = 3;

        private float[] matrix = new float[] { 0, 0, 0, 0, 1, 0, 0, 0, 0 };

        public ImageFilterKernel(float[] matrix, float factor, byte bias)
        {
            this.MaxValue = 1;
            this.Matrix = matrix;
            this.Factor = factor;
            this.Bias = bias;
        }

        public float[] Matrix
        {
            get => this.matrix;

            set
            {
                kernelSize = (byte)Math.Sqrt(value.Length);
                this.matrix = value;
            }
        }

        public float Factor { get; set; } = 1.0f;

        public byte Bias { get; set; }

        public byte GetKernelSize() => this.kernelSize;

        public bool HasEffect() => this.Value != 0;

        public void Prepare()
        {
        }

        public void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            var red = 0;
            var green = 0;
            var blue = 0;

            // todo offset center

            for (var i = 0; i < this.matrix.Length; i++)
            {
                red += (int)(kernel[i * 3] * this.matrix[i]);
                green += (int)(kernel[i * 3 + 1] * this.matrix[i]);
                blue += (int)(kernel[i * 3 + 2] * this.matrix[i]);
            }

            r = this.ClampByte((int)(this.Factor * red + this.Bias));
            g = this.ClampByte((int)(this.Factor * green + this.Bias));
            b = this.ClampByte((int)(this.Factor * blue + this.Bias));
        }

        public IImageFilter Clone()
        {
            return (IImageFilter)this.MemberwiseClone();
        }
    }
}
