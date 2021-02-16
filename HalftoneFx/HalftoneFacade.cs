﻿namespace Halftone
{
    using ImageFilter;

    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;

    public class GenerateDoneEventArgs : EventArgs
    {
        public Bitmap Image { get; set; }
    }

    public class HalftoneFacade
    {
        private readonly ImageFilterComplex filter = new ImageFilterComplex();

        private CancellationTokenSource cancelationToken = null;

        private Task task = null;

        public HalftoneFacade()
        {
            this.filter.Add(Filter.Grayscale, new ImageFilterGrayscale());
            this.filter.Add(Filter.Negative, new ImageFilterNegative());
            this.filter.Add(Filter.Brightness, new ImageFilterBrightness());
            this.filter.Add(Filter.Contrast, new ImageFilterContrast());
            this.filter.Add(Filter.Quantization, new ImageFilterQuantization());

            this.filter.OnValueChanged += (s, e) => this.OnPropertyChanged(s, e);
        }

        public event EventHandler OnPropertyChanged = delegate { };

        public event EventHandler<GenerateDoneEventArgs> OnImageAvailable = delegate { };

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

        public Bitmap Generate(Bitmap source, CancellationToken token)
        {
            var options = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };

            return ImageFilterPass.GetFiltered(source, this.filter, options);
        }

        public Bitmap Generate(Bitmap source)
        {
            return this.Generate(source, CancellationToken.None);
        }

        public async void GenerateAsync(Bitmap source, int delay = 10)
        {
            if (this.task != null && !this.task.IsCompleted)
            {
                this.cancelationToken.Cancel();
                //this.task.Wait();
            }

            this.cancelationToken = new CancellationTokenSource();
            var token = this.cancelationToken.Token;

            try
            {
                this.task = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await Task.Delay(delay, token);

                            var result = this.Generate(source, token);

                            if (!token.IsCancellationRequested)
                            {
                                this.OnImageAvailable.Invoke(this, new GenerateDoneEventArgs { Image = result });
                            }
                        }
                        catch
                        {
                        }
                    });

                await task;
            }
            catch
            {

            }
        }
    }

    public static class Filter
    {
        public const int Grayscale = 1;

        public const int Negative = 2;

        public const int Brightness = 3;

        public const int Contrast = 4;

        public const int Quantization = 5;
    }
}
