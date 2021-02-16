namespace ImageFilter
{
    public interface IImageFilter
    {
        bool HasEffect();

        void RGB(ref byte r, ref byte g, ref byte b);
    }
}
