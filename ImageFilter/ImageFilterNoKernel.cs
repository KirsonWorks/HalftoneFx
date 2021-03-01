namespace ImageFilter
{
    public abstract class ImageFilterNoKernel : ImageFilterBase, IImageFilter
    {
        public byte GetKernelSize() => 0;

        public virtual bool HasEffect() => false;

        public virtual void Prepare()
        {
        }

        public virtual void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y)
        {
        }
    }
}
