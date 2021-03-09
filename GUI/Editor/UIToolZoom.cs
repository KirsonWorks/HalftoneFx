namespace GUI.Editor
{
    using GUI;
    using GUI.Common;
    using GUI.Helpers;

    using System;
    using System.Drawing;

    public class UIToolZoom
    {
        private const float DefaultScale = 1.0f;

        private const float DefaultStep = 0.1f;

        private const float DefaultScaleMin = 0.03f;

        private const float DefaultScaleMax = 12.8f;

        private const float StepDivider = 5.0f;

        public float Scale { get; private set; } = DefaultScale;

        public float Step { get; private set; } = DefaultStep;

        public float ScaleMin { get; set; } = DefaultScaleMin;

        public float ScaleMax { get; set; } = DefaultScaleMax;

        public UIControl Control { get; set; }

        public SizeF OriginalSize { get; set; }

        public bool IsScaled => Math.Abs(this.Scale - DefaultScale) > float.Epsilon;

        public void Reset()
        {
            this.Scale = DefaultScale;
            this.Step = DefaultStep;
            this.Zoom();
        }

        public void Zoom()
        {
            this.Scale = UIMath.Clamp(this.Scale, this.ScaleMin, this.ScaleMax);

            if (this.Control != null)
            {
                this.Control.Width = this.OriginalSize.Width * this.Scale;
                this.Control.Height = this.OriginalSize.Height * this.Scale;
            }
        }

        public void Zoom(float scale)
        {
            this.Scale = scale;
            this.Step = this.Scale / StepDivider;
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
            this.Scale += delta * this.Step;
            this.Step = this.Scale / StepDivider;

            this.Zoom();
        }

        public void Zoom(int delta, PointF point)
        {
            if (this.Control != null)
            {
                var prevSize = this.Control.Size;
                var pos = this.Control.ScreenPosition;
                var pivotX = (point.X - pos.X) / this.Control.Width;
                var pivotY = (point.Y - pos.Y) / this.Control.Height;
                this.Zoom(delta);

                var deltaSize = prevSize - this.Control.Size;
                this.Control.SetPosition(pos.X + deltaSize.Width * pivotX, pos.Y + deltaSize.Height * pivotY);
            }
        }
    }
}
