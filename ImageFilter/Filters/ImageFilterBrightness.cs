namespace ImageFilter
{
    public class ImageFilterBrightness : ImageFilterNoKernel
    {
        public const int Min = -150;

        public const int Max = 100;

        public ImageFilterBrightness()
        {
            this.MinValue = Min;
            this.MaxValue = Max;
        }

        public override bool HasEffect() => this.Value != 0;

        public override void RGB(ref byte r, ref byte g, ref byte b)
        {
            r = this.ClampByte(r + this.Value);
            g = this.ClampByte(g + this.Value);
            b = this.ClampByte(b + this.Value);
        }
    }
}
