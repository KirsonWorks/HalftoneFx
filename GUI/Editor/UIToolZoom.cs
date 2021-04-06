namespace KWUI.Editor
{
    using KWUI;
    using KWUI.Common;
    using KWUI.Helpers;

    using System;
    using System.Drawing;

    public class UIToolZoom
    {
        private const float DefaultScale = 1.0f;

        private const float DefaultScaleIntensity = 0.2f;

        private const float DefaultScaleMin = 0.03f;

        private const float DefaultScaleMax = 12.8f;

        public float Scale { get; private set; } = DefaultScale;

        public float ScaleIntensity { get; private set; } = DefaultScaleIntensity;

        public float ScaleMin { get; set; } = DefaultScaleMin;

        public float ScaleMax { get; set; } = DefaultScaleMax;

        public UIControl Control { get; set; }

        public SizeF OriginalSize { get; set; }

        public bool IsScaled => Math.Abs(this.Scale - DefaultScale) > float.Epsilon;

        public void Reset()
        {
            this.Scale = DefaultScale;
            this.Zoom();
        }

        public void Zoom()
        {
            this.Scale = UIMath.Clamp(this.Scale, this.ScaleMin, this.ScaleMax);

            if (this.Control != null)
            {
                this.Control.Width = (float)Math.Round(this.OriginalSize.Width * this.Scale);
                this.Control.Height = (float)Math.Round(this.OriginalSize.Height * this.Scale);
            }
        }

        public void Zoom(float scale)
        {
            this.Scale = scale;
            this.Zoom();
        }

        public void Zoom(SizeF size)
        {
            var a = size.Aspect();
            var b = this.OriginalSize.Aspect();
            var sizeA = a > b ? size.Height : size.Width;
            var sizeB = a > b ? this.OriginalSize.Height : this.OriginalSize.Width;
            var scale = sizeA / Math.Max(1.0f, sizeB);
            this.Zoom(scale);
        }

        public void Zoom(int delta)
        {
            delta = Math.Sign(delta);
            this.Scale *= (float)Math.Exp(this.ScaleIntensity * delta);
            this.Zoom();
        }

        public void Zoom(int delta, PointF point)
        {
            if (this.Control != null)
            {
                var prevSize = this.Control.Size;
                var sr = this.Control.ScreenRect;
                point.X = UIMath.Clamp(point.X, sr.X, sr.Right);
                point.Y = UIMath.Clamp(point.Y, sr.Y, sr.Bottom);
                var pivotX = (point.X - sr.X) / sr.Width;
                var pivotY = (point.Y - sr.Y) / sr.Height;
                this.Zoom(delta);

                var deltaSize = prevSize - this.Control.Size;
                this.Control.SetPosition(sr.X + deltaSize.Width * pivotX, sr.Y + deltaSize.Height * pivotY);
            }
        }
    }
}
