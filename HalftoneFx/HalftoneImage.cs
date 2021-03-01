
namespace HalftoneFx
{
    using Helpers;

    using System;
    using System.Drawing;
    using System.Threading.Tasks;

    public class HalftoneImage : HalftoneGenerator
    {
        private Image original;

        private Bitmap editable, filtered, thumbnail;

        public HalftoneImage()
        {
            this.OnFilterPropertyChanged += async (s, e) =>
            {
                await this.GenerateFilteredAsync();
            };

            this.OnHalftonePropertyChanged += async (s, e) =>
            {
                await this.GenerateHalftoneAsync();
            };

            this.OnDownsamplingPropertyChanged += async (s, e) =>
            {
                //this.editable = 
                await this.GenerateAsync((Bitmap)this.original, ImageGenerationFlags.Downsampling, 200);
            };
        }

        public event EventHandler<GenerateDoneEventArgs> OnThumbnailAvailable = delegate { };

        public Image Image
        {
            get => this.original;

            set
            {
                this.original?.Dispose();
                this.editable?.Dispose();
                this.filtered?.Dispose();
                this.thumbnail?.Dispose();

                this.original = new Bitmap(value);
                this.editable = new Bitmap(value);
                this.filtered = new Bitmap(value);

                if (this.ThumbnailSize > 0)
                {
                    this.thumbnail = this.editable.Thumbnail(this.ThumbnailSize);
                }

                this.GenerateFilteredAsync();
            }
        }

        public int ThumbnailSize { get; set; } = 200;

        private void GenerateThumbnail(ImageGenerationFlags flags)
        {
            var newThumbnail = this.Generate(this.thumbnail, flags);

            this.OnThumbnailAvailable?.Invoke(this, new GenerateDoneEventArgs
            {
                Flags = ImageGenerationFlags.Filtering,
                Image = newThumbnail,
            });
        }

        private async Task GenerateFilteredAsync()
        {
            this.GenerateThumbnail(ImageGenerationFlags.Filtering);

            await this.GenerateAsync(this.editable, ImageGenerationFlags.Filtering, 200)
                    .ContinueWith(async (task) =>
                    {
                        if (task.Result != null)
                        {
                            this.filtered = task.Result;
                            await this.GenerateHalftoneAsync();
                        }
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
        }

        private async Task GenerateHalftoneAsync()
        {
            await this.GenerateAsync(this.filtered, ImageGenerationFlags.Halftoning, 100);
        }
    }
}
