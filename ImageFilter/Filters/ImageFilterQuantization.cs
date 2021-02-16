namespace ImageFilter
{
    public class ImageFilterQuantization : ImageFilterBase, IImageFilter
    {
        public static int Min = 1;

        public static int Max = 255;

        public ImageFilterQuantization()
        {
            this.MinValue = Min;
            this.MaxValue = Max;
        }

        public bool HasEffect() => this.Value > this.MinValue; 

        public void RGB(ref byte r, ref byte g, ref byte b)
        {
            r = this.ClampByte((int)this.Snap(r, this.Value));
            g = this.ClampByte((int)this.Snap(g, this.Value));
            b = this.ClampByte((int)this.Snap(b, this.Value));
        }
    }
}
