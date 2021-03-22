using System;

namespace ImageFilter
{
    public interface IImageFilter
    {
        int Value { get; set; }

        int MinValue { get; set; }

        int MaxValue { get; set; }

        bool HasEffect();

        byte GetKernelSize();

        void Prepare();

        void RGB(ref byte r, ref byte g, ref byte b, byte[] kernel, int x, int y);
    }
}
