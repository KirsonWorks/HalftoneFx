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

                var height = sourceBits.Height;
                var stride = sourceBits.Stride;
                var widthInBytes = sourceBits.Width * channels;
                byte* sourcePointer = (byte*)sourceBits.Scan0;
                byte* destPointer = (byte*)destBits.Scan0;

                byte kernelSize = filter.GetKernelSize();
                var kernelOffsets = MakeKernelOffsets(kernelSize);

                try
                {
                    int linesCompleted = 0;

                    Parallel.For(0, height, options, (y) =>
                    {
                        byte* sourceLine = sourcePointer + y * stride;
                        byte* destLine = destPointer + y * stride;
                        byte[] kernel = new byte[kernelOffsets.Length * 3];

                        for (var x = 0; x < widthInBytes; x += channels)
                        {
                            for (var i = 0; i < kernelOffsets.Length; i++)
                            {
                                var px = x + kernelOffsets[i].X * channels;
                                var py = y + kernelOffsets[i].Y;
                                
                                px = Math.Min(Math.Max(0, px), widthInBytes - channels);
                                py = Math.Min(Math.Max(0, py), height - 1);

                                byte* line = sourcePointer + py * stride;
                               
                                kernel[i * 3] = line[px + 2];
                                kernel[i * 3 + 1] = line[px + 1];
                                kernel[i * 3 + 2] = line[px];
                            }

                            destLine[x] = sourceLine[x];
                            destLine[x + 1] = sourceLine[x + 1];
                            destLine[x + 2] = sourceLine[x + 2];

                            if (channels == 4)
                            {
                                destLine[x + 3] = sourceLine[x + 3];
                            }

                            filter.RGB(ref destLine[x + 2], ref destLine[x + 1], ref destLine[x], kernel);
                        }

                        var part =  Math.Max(1, height / 10);
                        var count = Interlocked.Increment(ref linesCompleted);

                        if (count % part == 0)
                        {
                            progressChanged?.Invoke((float)count / height);
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

        private static Point[] MakeKernelOffsets(byte kernelSize)
        {
            var offset = new Point();
            var offsets = new Point[kernelSize * kernelSize];
            
            for (var i = 0; i < offsets.Length; i++)
            {
                offset.X = i % kernelSize - kernelSize / 2;
                offset.Y = i / kernelSize - kernelSize / 2;
                offsets[i] = offset;
            }

            return offsets;
        }
    }
}
