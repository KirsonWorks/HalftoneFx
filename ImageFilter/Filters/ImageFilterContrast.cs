namespace ImageFilter
{
    public class ImageFilterContrast : ImageFilterNoKernel
    {
        public const int Min = -50;

        public const int Max = 100;

        private float factor = 0;

        public ImageFilterContrast()
        {
            this.MinValue = Min;
            this.MaxValue = Max;
            this.Value = 0;
        }

        public override bool HasEffect() => this.Value != 0;

        public override void Prepare()
        {
            this.factor = (259.0f * (this.Value + 255.0f)) / (255.0f * (259.0f - this.Value));
        }

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            r = this.ClampByte((int)(this.factor * (r - 128) + 128));
            g = this.ClampByte((int)(this.factor * (g - 128) + 128));
            b = this.ClampByte((int)(this.factor * (b - 128) + 128));
        }
    }
}
