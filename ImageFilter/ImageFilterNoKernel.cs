namespace ImageFilter
{
    public abstract class ImageFilterNoKernel : ImageFilterBase, IImageFilter
    {
        public byte GetKernelSize() => 0;

        public virtual bool HasEffect() => false;

        public virtual void RGB(ref byte r, ref byte g, ref byte b)
        {
        }

        public virtual void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel)
        {
            this.RGB(ref r, ref g, ref b);
        }
    }
}
