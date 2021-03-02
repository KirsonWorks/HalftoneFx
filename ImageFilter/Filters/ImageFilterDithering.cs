namespace ImageFilter
{
    using Common;

    public class ImageFilterDithering : ImageFilterNoKernel
    {
        private BayerMatrix matrix;

        public ImageFilterDithering()
        {
            this.MaxValue = 3;
            this.matrix = new BayerMatrix(0);
        }

        public override bool HasEffect() => this.Value > 0;

        public override void Prepare()
        {
            this.matrix = new BayerMatrix(this.Value);
        }

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            var value = this.matrix[x, y];

            r = this.StepByte(r, value);
            g = this.StepByte(g, value);
            b = this.StepByte(b, value);
        }
    }
}
