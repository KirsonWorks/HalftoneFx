namespace ImageFilter
{
    public class ImageFilterQuantization : ImageFilterNoKernel
    {
        public const int Min = 1;

        public const int Max = 255;

        public ImageFilterQuantization()
        {
            this.MinValue = Min;
            this.MaxValue = Max;
        }

        public override bool HasEffect() => this.Value > this.MinValue;

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            r = this.ClampByte((int)this.Snap(r, this.Value));
            g = this.ClampByte((int)this.Snap(g, this.Value));
            b = this.ClampByte((int)this.Snap(b, this.Value));
        }
    }
}
