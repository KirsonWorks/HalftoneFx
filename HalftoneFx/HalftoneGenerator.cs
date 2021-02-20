namespace HalftoneFx
{
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

    public class HalftoneGenerator
    {
        private readonly Halftone halftone = new Halftone();

        private readonly ImageFilterComplex filter = new ImageFilterComplex();

        private CancellationTokenSource cancelationToken = null;

        private Task task = null;

        public HalftoneGenerator()
        {
            this.filter.Add(Filter.Smoothing, new ImageFilterGaussian5x5());
            this.filter.Add(Filter.Grayscale, new ImageFilterGrayscale());
            this.filter.Add(Filter.Negative, new ImageFilterNegative());
            this.filter.Add(Filter.Brightness, new ImageFilterBrightness());
            this.filter.Add(Filter.Contrast, new ImageFilterContrast());
            this.filter.Add(Filter.Quantization, new ImageFilterQuantization());
            
            this.filter.OnValueChanged += (s, e) => this.OnPropertyChanged(s, e);
            this.halftone.OnPropertyChanged += (s, e) => this.OnPropertyChanged(s, e);
        }

        public event EventHandler OnPropertyChanged = delegate { };

        public event EventHandler<GenerateDoneEventArgs> OnImageAvailable = delegate { };

        public event EventHandler<ProgressChangedEventArgs> OnProgressChanged = delegate { };

        public bool Grayscale
        {
            get => this.filter[Filter.Grayscale] == 1;
            set => this.filter[Filter.Grayscale] = Convert.ToInt32(value);
        }

        public bool Negative
        {
            get => this.filter[Filter.Negative] == 1;
            set => this.filter[Filter.Negative] = Convert.ToInt32(value);
        }

        public int Brightness
        {
            get => this.filter[Filter.Brightness];
            set => this.filter[Filter.Brightness] = value;
        }

        public int Contrast
        {
            get => this.filter[Filter.Contrast];
            set => this.filter[Filter.Contrast] = value;
        }

        public int Quantization
        {
            get => this.filter[Filter.Quantization];
            set => this.filter[Filter.Quantization] = value;
        }

        public bool Smoothing
        {
            get => this.filter[Filter.Smoothing] == 1;
            set => this.filter[Filter.Smoothing] = Convert.ToInt32(value);
        }

        public int HalftoneSize
        {
            get => this.halftone.Size;
            set => this.halftone.Size = value;
        }

        public Bitmap Generate(Bitmap source, bool ignoreHalftone, CancellationToken token)
        {
            var options = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };

            var img = ImageFilterPass.GetFiltered(source, this.filter, 
                (percent) =>
                {
                    this.OnProgressChanged.Invoke(this, new ProgressChangedEventArgs { Percent = percent });
                }, options);

            return ignoreHalftone ? img : this.halftone.Generate(img);
        }

        public Bitmap Generate(Bitmap source, bool ignoreHalftone)
        {
            return this.Generate(source, ignoreHalftone, CancellationToken.None);
        }

        public async void GenerateAsync(Bitmap source, int delay = 10)
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

                        var result = this.Generate(source, false, token);

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

    public static class Filter
    {
        public const int Smoothing = 1;

        public const int Grayscale = 2;

        public const int Negative = 3;

        public const int Brightness = 4;

        public const int Contrast = 5;

        public const int Quantization = 6;
    }
}
