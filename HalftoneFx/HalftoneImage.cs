
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
                var newThumbnail = this.Generate(this.thumbnail, ImageGenerationFlags.Filtering);
                this.OnThumbnailAvailable?.Invoke(this, new GenerateDoneEventArgs
                { 
                    Flags = ImageGenerationFlags.Filtering,
                    Image = newThumbnail,
                });

                await this.GenerateAsync(this.editable, ImageGenerationFlags.Filtering, 200)
                          .ContinueWith((task) =>
                          {
                                if (task.Result != null)
                                {
                                    this.filtered = task.Result;
                                }
                          },
                          TaskContinuationOptions.OnlyOnRanToCompletion |
                          TaskContinuationOptions.ExecuteSynchronously);
            };

            this.OnHalftonePropertyChanged += async (s, e) =>
            {
                await this.GenerateAsync(this.filtered, ImageGenerationFlags.Halftoning, 100);
            };
            
            this.OnDownsamplingPropertyChanged += (s, e) =>
            {
                this.GenerateAsync(this.filtered, ImageGenerationFlags.Downsampling, 200);
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
                this.filtered = new Bitmap(value);
                this.editable = new Bitmap(value);

                if (this.ThumbnailSize > 0)
                {
                    this.thumbnail = this.editable.Thumbnail(this.ThumbnailSize);
                }
            }
        }

        public int ThumbnailSize { get; set; } = 200;
    }
}
