namespace HalftoneFx
{
    using Helpers;
    using Halftone;
    using ImageFilter;

    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;

    public class GenerateDoneEventArgs : EventArgs
    {
        public Bitmap Image { get; set; }
    }

    public class ProgressChangedEventArgs : EventArgs
    {
        public float Percent { get; set; }
    }
    
    [Flags]
    public enum ImageGenerationFlags
    {
        Downsampling = 1,
        Filtering = 2,
        Halftoning = 4
    }

    public class HalftoneGenerator
    {
        private readonly ImageFilterComplex filter = new ImageFilterComplex();

        private readonly Halftone halftone = new Halftone();

        private CancellationTokenSource cancelationToken = null;

        private int downsamplingLevel = 1;

        private Task task = null;

        public HalftoneGenerator()
        {
            this.filter.Add(nameof(Smoothing), new ImageFilterGaussian5x5());
            this.filter.Add(nameof(Grayscale), new ImageFilterGrayscale());
            this.filter.Add(nameof(Negative), new ImageFilterNegative());
            this.filter.Add(nameof(Brightness), new ImageFilterBrightness());
            this.filter.Add(nameof(Contrast), new ImageFilterContrast());
            this.filter.Add(nameof(Quantization), new ImageFilterQuantization());
            
            this.filter.OnPropertyChanged += (s, e) => this.OnFilterPropertyChanged(s, e);
            this.halftone.OnPropertyChanged += (s, e) => this.OnHalftonePropertyChanged(s, e);
        }

        public event EventHandler OnFilterPropertyChanged = delegate { };

        public event EventHandler OnHalftonePropertyChanged = delegate { };

        public event EventHandler OnDownsamplingPropertyChanged = delegate { };

        public event EventHandler<GenerateDoneEventArgs> OnImageAvailable = delegate { };

        public event EventHandler<ProgressChangedEventArgs> OnProgress = delegate { };

        public bool Grayscale
        {
            get => this.filter[nameof(Grayscale)] == 1;
            set => this.filter[nameof(Grayscale)] = Convert.ToInt32(value);
        }

        public bool Negative
        {
            get => this.filter[nameof(Negative)] == 1;
            set => this.filter[nameof(Negative)] = Convert.ToInt32(value);
        }

        public int Brightness
        {
            get => this.filter[nameof(Brightness)];
            set => this.filter[nameof(Brightness)] = value;
        }

        public int Contrast
        {
            get => this.filter[nameof(Contrast)];
            set => this.filter[nameof(Contrast)] = value;
        }

        public int Quantization
        {
            get => this.filter[nameof(Quantization)];
            set => this.filter[nameof(Quantization)] = value;
        }

        public bool Smoothing
        {
            get => this.filter[nameof(Smoothing)] == 1;
            set => this.filter[nameof(Smoothing)] = Convert.ToInt32(value);
        }

        public int DownsamplingLevel
        {
            get => this.downsamplingLevel;

            set
            {
                this.downsamplingLevel = Math.Min(Math.Max(0, value), 32);
                this.OnDownsamplingPropertyChanged(this, EventArgs.Empty);
            }
        }

        public int HalftoneSize
        {
            get => this.halftone.Size;
            set => this.halftone.Size = value;
        }

        public Bitmap Generate(Bitmap source, ImageGenerationFlags flags, CancellationToken token)
        {
            var parallelOpt = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };

            var img = source;

            if (flags.HasFlag(ImageGenerationFlags.Downsampling) && this.DownsamplingLevel > 1)
            {
                img = img.Downsampling(this.DownsamplingLevel);
            }

            if (flags.HasFlag(ImageGenerationFlags.Filtering))
            {
                img = ImageFilterPass.GetFiltered(img, this.filter,
                (percent) =>
                {
                    this.OnProgress.Invoke(this, new ProgressChangedEventArgs { Percent = percent });
                }, parallelOpt);
            }

            if (flags.HasFlag(ImageGenerationFlags.Halftoning))
            {
                img = this.halftone.Generate(img);
            }

            return img;
        }

        public Bitmap Generate(Bitmap source, ImageGenerationFlags flags)
        {
            return this.Generate(source, flags, CancellationToken.None);
        }

        public async Task GenerateAsync(Bitmap source, ImageGenerationFlags flags, int delay = 10)
        {
            if (this.task != null && !this.task.IsCompleted)
            {
                this.cancelationToken.Cancel();
            }

            this.cancelationToken = new CancellationTokenSource();
            var token = this.cancelationToken.Token;

            this.task = Task.Run(
                async () =>
                {
                    try
                    {
                        await Task.Delay(delay, token);

                        var result = this.Generate(source, flags, token);

                        if (!token.IsCancellationRequested)
                        {
                            this.OnImageAvailable.Invoke(this, new GenerateDoneEventArgs { Image = result });
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch
                    {
                        throw;
                    }
                });

            await task;
        }
    }
}
