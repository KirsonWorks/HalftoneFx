namespace ImageFilter
{
    public interface IImageFilter
    {
        bool HasEffect();

        byte GetKernelSize();

        void RGB(ref byte r, ref byte g, ref byte b);
    }
}
