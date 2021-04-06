namespace ImageFilter
{
    public class ImageFilterGrayscale : ImageFilterNoKernel
    {
        public ImageFilterGrayscale()
        {
            this.MaxValue = 1;
        }

        public override bool HasEffect() => this.Value != 0;

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            r = g = b = this.ClampByte((r * 19595 + g * 38470 + b * 7471) >> 16);
        }
    }
}
