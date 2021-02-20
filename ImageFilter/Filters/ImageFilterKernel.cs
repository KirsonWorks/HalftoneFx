using System;

namespace ImageFilter
{
    public class ImageFilterKernel : ImageFilterBase, IImageFilter
    {
        private byte kernelSize;

        private float[] matrix;
        
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

        public bool HasEffect() => true;

        public void RGB(ref byte r, ref byte g, ref byte b)
        {
        }

        public void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel)
        {
            var red = 0;
            var green = 0;
            var blue = 0;

            // todo offset

            for (var i = 0; i < this.matrix.Length; i++)
            {
                red += (int)(kernel[i * 3] * this.matrix[i]);
                green += (int)(kernel[i * 3 + 1] * this.matrix[i]);
                blue += (int)(kernel[i * 3 + 2] * this.matrix[i]);
            }

            r = this.ClampByte(red);
            g = this.ClampByte(green);
            b = this.ClampByte(blue);
        }
    }
}
