namespace ImageFilter
{
    public class ImageFilterSaturation : ImageFilterNoKernel
    {
        private int factor = 0;

        public ImageFilterSaturation()
            : base()
        {
            this.MinValue = 0;
            this.MaxValue = 200;
            this.Value = 100;
        }

        public override bool HasEffect() => this.Value != 100;

        public override void Prepare()
        {
            this.factor = 65536 * this.Value / 100;
        }

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            int lum = r * 19595 + g * 38470 + b * 7471;
            int lumMask = lum >> 16;

            b = this.ClampByte(lum + (b - lumMask) * this.factor >> 16);
            g = this.ClampByte(lum + (g - lumMask) * this.factor >> 16);
            r = this.ClampByte(lum + (r - lumMask) * this.factor >> 16);
        }
    }
}
