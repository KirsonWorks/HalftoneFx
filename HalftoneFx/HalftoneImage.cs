
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
            this.OnFilterPropertyChanged += (s, e) => this.Generate();
            this.OnHalftonePropertyChanged += (s, e) => this.Generate();
            this.OnDownsamplingPropertyChanged += (s, e) =>
            {
                this.editable = this.original.Downsampling(this.DownsamplingLevel);
                this.thumbnail = this.editable.Thumbnail(this.ThumbnailSize);
                this.Generate();
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

                if (this.HasThumbnail)
                {
                    this.thumbnail = this.editable.Thumbnail(this.ThumbnailSize);
                }
            }
        }

        public bool HasThumbnail { get; set; }

        public int ThumbnailSize { get; set; } = 200;

        public async Task Generate()
        {
            if (this.HasThumbnail)
            {
                var newThumbnail = this.Generate(this.thumbnail, ImageGenerationFlags.Filtering);
                this.OnThumbnailAvailable?.Invoke(this, new GenerateDoneEventArgs { Image = newThumbnail });
            }

            await this.GenerateAsync(this.editable, ImageGenerationFlags.Filtering | ImageGenerationFlags.Halftoning, 500);
        }
    }
}
