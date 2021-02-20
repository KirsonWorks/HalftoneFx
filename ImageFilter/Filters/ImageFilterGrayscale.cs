namespace ImageFilter
{
    public class ImageFilterGrayscale : ImageFilterNoKernel
    {
        public ImageFilterGrayscale()
        {
            this.MaxValue = 1;
        }

        public override bool HasEffect() => this.Value != 0;

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel)
        {
            r = g = b = this.ClampByte((int)(r * 0.2989 + g * 0.5870 + b * 0.1140));
        }
    }
}
