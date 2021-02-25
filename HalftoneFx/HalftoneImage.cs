
namespace HalftoneFx
{
    using Helpers;

    using System;
    using System.Drawing;
    using System.Threading.Tasks;

    public class HalftoneImage : HalftoneGenerator
    {
        private Image original;

        private Bitmap editable, thumbnail;

        public HalftoneImage()
        {
            this.OnFilterPropertyChanged += (s, e) => this.GenerateAsync(true, 500);
            this.OnHalftonePropertyChanged += (s, e) => this.GenerateAsync(false, 100);
            
            this.OnDownsamplingPropertyChanged += (s, e) =>
            {
                this.editable = this.original.Downsampling(this.DownsamplingLevel);
                this.thumbnail = this.editable.Thumbnail(this.ThumbnailSize);
                this.GenerateAsync(true, 500);
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
                this.thumbnail?.Dispose();

                this.original = value;
                this.editable = new Bitmap(value);

                if (this.ThumbnailSize > 0)
                {
                    this.thumbnail = this.editable.Thumbnail(this.ThumbnailSize);
                }
            }
        }


        public int ThumbnailSize { get; set; } = 200;

        public async Task GenerateAsync(bool genThumbnail, int delay)
        {
            if (this.ThumbnailSize > 0 && genThumbnail)
            {
                var newThumbnail = this.Generate(this.thumbnail, ImageGenerationFlags.Filtering);
                this.OnThumbnailAvailable?.Invoke(this, new GenerateDoneEventArgs { Image = newThumbnail });
            }

            await this.GenerateAsync(this.editable, ImageGenerationFlags.Filtering | ImageGenerationFlags.Halftoning, delay);
        }
    }
}
