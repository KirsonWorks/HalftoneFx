namespace ImageFilter
{
    public interface IImageFilter
    {
        bool HasEffect();

        byte GetKernelSize();

        void Prepare();

        void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y);
    }
}
