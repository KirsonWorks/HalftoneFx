namespace ImageFilter
{
    public class ImageFilterNegative : ImageFilterNoKernel
    {
        public ImageFilterNegative()
        {
            this.MaxValue = 1;
        }

        public override bool HasEffect() => this.Value != 0;

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            r = (byte)(255 - r);
            g = (byte)(255 - g);
            b = (byte)(255 - b);
        }
    }
}
