namespace ImageFilter
{
    public class ImageFilterGaussian5x5 : ImageFilterKernel
    {
        private static readonly float[] Kernel =  
        {
            1, 4, 6, 4, 1,
            4, 16, 24, 16, 4,
            6, 24, 36, 24, 6,
            4, 16, 24, 16, 4,
            1, 4, 6, 4, 1,
        };

        public ImageFilterGaussian5x5()
            : base(Kernel, 1.0f / 256.0f, 0)
        {
        }
    }
}
