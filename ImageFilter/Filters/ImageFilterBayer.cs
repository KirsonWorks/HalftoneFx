namespace ImageFilter
{
    public class ImageFilterBayer : ImageFilterNoKernel
    {
        private float[] Table = new float[16] 
        {
            0.1250f, 1.0000f, 0.1875f, 0.8125f, 
            0.6250f, 0.3750f, 0.6875f, 0.4375f, 
            0.2500f, 0.8750f, 0.0625f, 0.9375f, 
            0.7500f, 0.5000f, 0.5625f, 0.3125f
        };

        public ImageFilterBayer()
        {
            this.MaxValue = 1;
        }

        public override bool HasEffect() => true;///this.Value != 0;

        public override void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
            var i = y % 4 * 4 + x % 4;
            var m = (byte)((r + g + b) / 3);
            r = g = b = (byte)(m < (Table[i] * 255) ? 0 : 255);
        }
    }
}
