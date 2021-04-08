﻿namespace HalftoneFx
{
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
        private readonly ImageFilterComplex filters = new ImageFilterComplex();

        private readonly HalftoneArtist halftone = new HalftoneArtist();

        private CancellationTokenSource cancelationToken = null;

        private Task<Bitmap> task = null;

        public HalftoneGenerator()
        {
            this.Smoothing = new ImageFilterGaussian5x5();
            this.filters.Add(this.Smoothing);

            this.Negative = new ImageFilterNegative();
            this.filters.Add(this.Negative);

            this.Brightness = new ImageFilterBrightness();
            this.filters.Add(this.Brightness);

            this.Contrast = new ImageFilterContrast();
            this.filters.Add(this.Contrast);

            this.Saturation = new ImageFilterSaturation();
            this.filters.Add(this.Saturation);

            this.Quantization = new ImageFilterQuantization();
            this.filters.Add(this.Quantization);

            this.Dithering = new ImageFilterDithering();
            this.filters.Add(this.Dithering);

            this.Palettes = new ImageFilterPalettes();
            this.filters.Add(this.Palettes);

            // For testing
            this.Palettes.AddPalette(new[] {
                Color.FromArgb(0x081820),
                Color.FromArgb(0x346856),
                Color.FromArgb(0x88c070),
                Color.FromArgb(0xe0f8d0)});

            this.filters.OnPropertyChanged += (s, e) => this.OnFilterPropertyChanged(s, e);
            this.halftone.OnPropertyChanged += (s, e) => this.OnHalftonePropertyChanged(s, e);
        }

        public event EventHandler OnFilterPropertyChanged = delegate { };

        public event EventHandler OnHalftonePropertyChanged = delegate { };

        public event EventHandler<GenerateDoneEventArgs> OnImageAvailable = delegate { };

        public event EventHandler<ProgressChangedEventArgs> OnProgress = delegate { };

        public IImageFilter Smoothing { get; private set; }

        public IImageFilter Negative { get; private set; }

        public IImageFilter Brightness { get; private set; }

        public IImageFilter Contrast { get; private set; }

        public IImageFilter Saturation { get; private set; }

        public IImageFilter Quantization { get; private set; }

        public IImageFilter Dithering { get; private set; }

        public ImageFilterPalettes Palettes { get; private set; }

        public int GridType
        {
            get => this.halftone.GridType;
            set => this.halftone.GridType = value;
        }

        public int ShapeType
        {
            get => this.halftone.ShapeType;
            set => this.halftone.ShapeType = value;
        }

        public int ShapeSizeBy
        {
            get => this.halftone.ShapeSizeBy;
            set => this.halftone.ShapeSizeBy = value;
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

        public Image CustomPattern
        {
            get => this.halftone.CustomPattern;
            set => this.halftone.CustomPattern = value;
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
                img = ImageFilterPass.GetFiltered(img, this.filters,
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
                this.task.Wait();
            }

            this.cancelationToken = new CancellationTokenSource();
            var token = this.cancelationToken.Token;

            this.task = Task.Run(
                async () =>
                {
                    try
                    {
                        await Task.Delay(delay, token).ConfigureAwait(false);

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

            return await task.ConfigureAwait(false);
        }
    }
}
