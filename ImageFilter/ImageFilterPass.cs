namespace ImageFilter
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Threading.Tasks;

    public class ImageFilterPass
    {
        public static Bitmap GetFiltered(Bitmap original, IImageFilter filter, Action<float> progressChanged, ParallelOptions options)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (!filter.HasEffect())
            {
                progressChanged?.Invoke(1.0f);
                return new Bitmap(original);
            }

            unsafe
            {
                var pixelFormat = original.PixelFormat;
                var channels = Image.GetPixelFormatSize(pixelFormat) / 8;
                var rect = new Rectangle(0, 0, original.Width, original.Height);
                
                var destImage = new Bitmap(original.Width, original.Height, pixelFormat);
                var sourceBits = original.LockBits(rect, ImageLockMode.ReadOnly, pixelFormat);
                var destBits = destImage.LockBits(rect, ImageLockMode.WriteOnly, pixelFormat);

                if (channels < 3)
                {
                    throw new Exception("Pixel format not support");
                }

                var stride = sourceBits.Stride;
                var widthInBytes = sourceBits.Width * channels;
                byte* sourcePointer = (byte*)sourceBits.Scan0;
                byte* destPointer = (byte*)destBits.Scan0;

                try
                {
                    int rowsCompleted = 0;

                    Parallel.For(0, sourceBits.Height, options, (y) =>
                    {
                        byte kernelSize = filter.GetKernelSize();

                        if (kernelSize >= 3)
                        {
                            byte[] kernel = new byte[kernelSize * kernelSize * 3];

                            for (var kx = 0; kx < kernelSize; kx++)
                            {
                                byte* line = sourcePointer + y * stride;

                                for (var ky = 0; ky < kernelSize; ky++)
                                {

                                }
                            }
                        }
                        else
                        {
                            byte* sourceLine = sourcePointer + y * stride;
                            byte* destLine = destPointer + y * stride;

                            for (var x = 0; x < widthInBytes; x += channels)
                            {
                                destLine[x] = sourceLine[x];
                                destLine[x + 1] = sourceLine[x + 1];
                                destLine[x + 2] = sourceLine[x + 2];

                                if (channels == 4)
                                {
                                    destLine[x + 3] = sourceLine[x + 3];
                                }

                                filter.RGB(ref destLine[x], ref destLine[x + 1], ref destLine[x + 2]);
                            }
                        }

                        var part = sourceBits.Height / 10;
                        var count = Interlocked.Increment(ref rowsCompleted);

                        if (count % part == 0)
                        {
                            progressChanged?.Invoke((float)count / sourceBits.Height);
                        }
                    });
                }
                finally
                {
                    original?.UnlockBits(sourceBits);
                    destImage?.UnlockBits(destBits);
                }

                return destImage;
            }
        }

        public static Bitmap GetFiltered(Bitmap original, IImageFilter filter)
        {
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            return GetFiltered(original, filter, null, options);
        }
    }
}
