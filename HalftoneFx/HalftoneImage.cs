
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
            this.OnFilterPropertyChanged += (s, e) => this.GenerateAsync((Bitmap)this.original, ImageGenerationFlags.Filtering, 500);
            this.OnHalftonePropertyChanged += (s, e) => this.GenerateAsync(this.editable, ImageGenerationFlags.Halftoning, 100);
            
            this.OnDownsamplingPropertyChanged += (s, e) =>
            {
                //this.editable = this.original.Downsampling(this.DownsamplingLevel);
                //this.thumbnail = this.editable.Thumbnail(this.ThumbnailSize);
                //this.GenerateAsync(false, 500);
            };

            this.OnImageAvailable += DoImageAvailable;
        }

        private void DoImageAvailable(object sender, GenerateDoneEventArgs e)
        {
            if (e.Flags.HasFlag(ImageGenerationFlags.Filtering))
            {
                this.editable = e.Image;

                if (this.HalftoneEnabled)
                {
                    this.GenerateAsync(this.editable, ImageGenerationFlags.Halftoning);
                }
            }
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

        /*
        public async Task GenerateAsync(bool genThumbnail, int delay)
        {   
            if (this.ThumbnailSize > 0 && genThumbnail)
            {
                var newThumbnail = this.Generate(this.thumbnail, ImageGenerationFlags.Filtering);
                this.OnThumbnailAvailable?.Invoke(this, new GenerateDoneEventArgs { Image = newThumbnail });
            }
            
            await this.GenerateAsync(this.editable, flags, delay);
        }
        */
    }
}
