namespace Common
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Collections.Generic;

    public static class ImageHelper
    {
        public static Bitmap Resize(this Image image, int width, int height, InterpolationMode interpolation)
        {
            var result = new Bitmap(width, height);
            var rect = new Rectangle(0, 0, width, height);

            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.InterpolationMode = interpolation;

                using (var attrs = new ImageAttributes())
                {
                    attrs.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attrs);
                }
            }

            return result;
        }

        public static Bitmap Resize(this Image image, float percent, InterpolationMode interpolation)
        {
            return image.Resize((int)(image.Width * percent), (int)(image.Height * percent), interpolation);
        }

        public static Bitmap Thumbnail(this Image image, int size)
        {
            if ((image.Width > size || image.Height > size) && size > 0)
            {
                var mode = InterpolationMode.Default;
                var aspect = (float)image.Width / image.Height;

                if (aspect >= 1.0f)
                {
                    return image.Resize(size, (int)(size / aspect), mode);
                }

                return image.Resize((int)(size * aspect), size, mode);
            }

            return (Bitmap)image;
        }

        public static Bitmap Downsampling(this Image image, int level)
        {
            if (level > 1)
            {
                var width = Math.Max(8, image.Width / level);
                var height = Math.Max(8, image.Height / level);
                var sample = image.Resize(width, height, InterpolationMode.Default);
                return sample.Resize(image.Width, image.Height, InterpolationMode.Default);
            }

            return new Bitmap(image);
        }

        public static void SaveAs(this Image image, string path)
        {
            var formats = new Dictionary<string, ImageFormat>
            { 
                { ".png", ImageFormat.Png },
                { ".jpg", ImageFormat.Jpeg },
                { ".jpeg", ImageFormat.Jpeg },
                { ".bmp", ImageFormat.Bmp },
                { ".gif", ImageFormat.Gif },
                { ".tif", ImageFormat.Tiff },
                { ".wmf", ImageFormat.Wmf }
            };

            var ext = Path.GetExtension(path).ToLower();

            if (formats.ContainsKey(ext))
            {
                image.Save(path, formats[ext]);
            }
            else
            {
                throw new Exception("File format not support");
            }
        }
    }
}
