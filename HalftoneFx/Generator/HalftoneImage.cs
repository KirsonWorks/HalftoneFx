
namespace HalftoneFx
{
    using Common;

    using System;
    using System.Drawing;
    using System.Threading.Tasks;

    public class HalftoneImage : HalftoneGenerator
    {
        private Image original;

        private Bitmap filtered, thumbnail;

        public HalftoneImage(ColorPalettes palettes)
            : base(palettes)
        {
            this.OnFilterPropertyChanged += async (s, e) =>
            {
                await this.GenerateFilteredAsync(200);
            };

            this.OnHalftonePropertyChanged += async (s, e) =>
            {
                await this.GenerateHalftoneAsync(100);
            };
        }

        public event EventHandler<GenerateDoneEventArgs> OnThumbnailAvailable = delegate { };

        public Image Image
        {
            get => this.original;

            set
            {
                this.original?.Dispose();
                this.filtered?.Dispose();

                GC.Collect();

                this.original = new Bitmap(value);
                this.filtered = new Bitmap(value);

                this.Thumbnail = this.original;

                Task.Run(async () => await this.GenerateFilteredAsync(200)).ConfigureAwait(false);
            }
        }

        public Image Filtered
        {
            get => this.filtered;
        }

        public Image Thumbnail
        {
            get => this.thumbnail;

            private set
            {
                this.thumbnail?.Dispose();
                this.thumbnail = value.Thumbnail(this.ThumbnailSize);
            }
        }

        public int ThumbnailSize { get; set; } = 300;

        private Image GenerateThumbnail(ImageGenerationFlags flags)
        {   
            var newThumbnail = this.Generate(this.thumbnail, flags);

            this.OnThumbnailAvailable?.Invoke(this, new GenerateDoneEventArgs
            {
                Flags = flags,
                Image = newThumbnail,
            });

            return newThumbnail;
        }

        private async Task GenerateFilteredAsync(int delay)
        {
            try
            {
                if (this.ThumbnailSize > 0)
                {
                    var newThumbnail = this.GenerateThumbnail(ImageGenerationFlags.Filtering);

                    if (this.thumbnail == this.original)
                    {
                        this.filtered = new Bitmap(newThumbnail);
                        await this.GenerateHalftoneAsync(100).ConfigureAwait(false);
                        return;
                    }
                }

                await this.GenerateAsync((Bitmap)this.original, ImageGenerationFlags.Filtering, delay)
                        .ContinueWith(async (task) =>
                        {
                            if (task.Result != null)
                            {
                                this.filtered = task.Result;
                                await this.GenerateHalftoneAsync(100).ConfigureAwait(false);
                            }
                        },
                        TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously)
                        .ConfigureAwait(false);
            }
            catch
            {
            }
        }

        private async Task GenerateHalftoneAsync(int delay)
        {
            try
            {
                await this.GenerateAsync(this.filtered, ImageGenerationFlags.Halftoning, delay);
            }
            catch
            {
            }
        }
    }
}
