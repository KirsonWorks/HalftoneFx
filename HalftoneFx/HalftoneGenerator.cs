namespace HalftoneFx
{
    using Helpers;
    using Halftone;
    using ImageFilter;

    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;

    [Flags]
    public enum ImageGenerationFlags
    {
        Filtering = 1,
        Halftoning = 2
    }

    public class GenerateDoneEventArgs : EventArgs
    {
        public ImageGenerationFlags Flags { get; set; }

        public Bitmap Image { get; set; }
    }

    public class ProgressChangedEventArgs : EventArgs
    {
        public float Percent { get; set; }
    }

    public class HalftoneGenerator
    {
        private readonly ImageFilterComplex filter = new ImageFilterComplex();

        private readonly Halftone halftone = new Halftone();

        private CancellationTokenSource cancelationToken = null;

        private Task<Bitmap> task = null;

        public HalftoneGenerator()
        {
            this.filter.Add(nameof(Smoothing), new ImageFilterGaussian5x5());
            this.filter.Add(nameof(Grayscale), new ImageFilterGrayscale());
            this.filter.Add(nameof(Negative), new ImageFilterNegative());
            this.filter.Add(nameof(Brightness), new ImageFilterBrightness());
            this.filter.Add(nameof(Contrast), new ImageFilterContrast());
            this.filter.Add(nameof(Quantization), new ImageFilterQuantization());
            this.filter.Add(nameof(Dithering), new ImageFilterDithering());
            
            this.filter.OnPropertyChanged += (s, e) => this.OnFilterPropertyChanged(s, e);
            this.halftone.OnPropertyChanged += (s, e) => this.OnHalftonePropertyChanged(s, e);
        }

        public event EventHandler OnFilterPropertyChanged = delegate { };

        public event EventHandler OnHalftonePropertyChanged = delegate { };

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

        public int Dithering
        {
            get => this.filter[nameof(Dithering)];
            set => this.filter[nameof(Dithering)] = value;
        }

        public int GridType
        {
            get => this.halftone.GridType;
            set => this.halftone.GridType = value;
        }

        public int PatternType
        {
            get => this.halftone.PatternType;
            set => this.halftone.PatternType = value;
        }

        public int ShapeSizing
        {
            get => this.halftone.ShapeSizing;
            set => this.halftone.ShapeSizing = value;
        }
            
        public int CellSize
        {
            get => this.halftone.CellSize;
            set => this.halftone.CellSize = value;
        }

        public float CellScale
        {
            get => this.halftone.CellScale;
            set => this.halftone.CellScale = value;
        }

        public bool HalftoneEnabled
        {
            get => this.halftone.Enabled;
            set => this.halftone.Enabled = value;
        }

        public bool TransparentBg
        {
            get => this.halftone.TransparentBg;
            set => this.halftone.TransparentBg = value;
        }

        public Bitmap Generate(Bitmap source, ImageGenerationFlags flags, CancellationToken token)
        {
            var parallelOpt = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount),
            };

            var img = new Bitmap(source);

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
                img = this.halftone.Generate(img, (percent) =>
                {
                    this.OnProgress.Invoke(this, new ProgressChangedEventArgs { Percent = percent });
                }, token);
            }

            return img;
        }

        public Bitmap Generate(Bitmap source, ImageGenerationFlags flags)
        {
            return this.Generate(source, flags, CancellationToken.None);
        }

        public async Task<Bitmap> GenerateAsync(Bitmap source, ImageGenerationFlags flags, int delay = 10)
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
                            this.OnImageAvailable.Invoke(this, new GenerateDoneEventArgs
                            { 
                                Flags = flags,
                                Image = new Bitmap(result),
                            });
                        }

                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }, token);

            return await task;
        }
    }
}
