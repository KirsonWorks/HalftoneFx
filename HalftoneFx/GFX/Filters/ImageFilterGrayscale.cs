namespace HalftoneFx.GFX
{
    public class ImageFilterGrayscale : ImageFilterBase, IImageFilter
    {
        public ImageFilterGrayscale()
        {
            this.MaxValue = 1;
        }

        public bool HasEffect() => this.Value != 0;

        public void RGB(ref byte r, ref byte g, ref byte b)
        {
            r = g = b = this.ClampByte((int)(r * 0.2989 + g * 0.5870 + b * 0.1140));
        }
    }
}
