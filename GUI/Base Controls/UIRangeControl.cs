namespace KWUI.BaseControls
{
    using KWUI.Common;
    using KWUI.Helpers;

    using System;
    using System.Drawing;

    [Flags]
    public enum UIRangeTextFlags
    {
        None,
        Decimal,
        PlusSign,
    }

    public enum UIRangeTextType
    {
        None,
        Caption,
        Value,
        Percent,
        Range,
    }

    public class UIRangeControl : UIControl
    {
        private readonly UIRange<float> range = new UIRange<float>();

        private float prevValue;

        private float? defaultValue;

        public UIRangeControl() : base()
        {
            this.Min = 0;
            this.Max = 100;
        }

        private float Interval => this.Max - this.Min;

        public event EventHandler OnChanging = delegate { };

        public event EventHandler OnChanged = delegate { };

        public float Value
        {
            get => this.range.Value;

            set
            {
                var step = Math.Min(this.Step, this.Interval);
                var newValue = UIMath.Snap(value, step);

                if (!this.defaultValue.HasValue)
                {
                    this.defaultValue = newValue;
                }

                if (!this.range.IsSameValue(newValue))
                {
                    this.range.Value = newValue;
                    this.OnChanging(this, EventArgs.Empty);
                }
            }
        }

        public float Min
        {
            get => this.range.Min;
            set => this.range.Min = value;
        }

        public float Max
        {
            get => this.range.Max;
            set => this.range.Max = value;
        }

        public float Step
        {
            get => this.range.Step;
            set => this.range.Step = value;
        }

        public float Percent
        {
            get
            {
                if (Math.Abs(this.Interval) > float.Epsilon)
                {
                    return (this.Value - this.Min) / this.Interval;
                }

                return 0;
            }

            set
            {
                this.Value = this.Min + this.Interval * UIMath.Clamp(value, 0.0f, 1.0f);
            }
        }

        public UIRangeTextType TextType { get; set; }

        public UIRangeTextFlags TextFlags { get; set; }

        public PointF TextAlign { get; set; } = UIAlign.Center;

        public string TextFormat { get; set; }

        public string[] Lookup { get; set; }

        public bool Vertical { get; set; }

        public bool Inverted { get; set; }

        public bool ReadOnly { get; set; }

        protected bool IsHovered { get; private set; }

        protected bool IsPressed { get; set; }

        protected void SetValueByPoint(PointF point)
        {
            float diff;
            float divider;

            var r = this.ScreenRect.Inflate(-this.Style.InnerShrink);

            if (this.Vertical)
            {
                diff = point.Y - r.Y;
                divider = r.Height;
            }
            else
            {
                diff = point.X - r.X;
                divider = r.Width;
            }

            this.Percent = this.Inverted ? (divider - diff) / divider : diff / divider;
        }

        protected override void DoMouseOverOut(UIMouseEventArgs e, bool isOver)
        {
            if (this.Enabled && !this.ReadOnly)
            {
                this.IsHovered = isOver;
            }

            base.DoMouseOverOut(e, isOver);
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            switch (e.EventType)
            {
                case UIMouseEventType.Down:
                    this.IsPressed = this.IsHovered && e.Button != UIMouseButtons.Middle;

                    if (this.IsPressed && e.Button == UIMouseButtons.Left)
                    {
                        this.prevValue = this.Value;
                        this.SetValueByPoint(e.Location);
                    }

                    break;

                case UIMouseEventType.Move:
                    if (this.IsPressed && e.Button == UIMouseButtons.Left)
                    {
                        this.SetValueByPoint(e.Location);
                    }

                    break;

                case UIMouseEventType.Up:
                    if (this.IsPressed)
                    {
                        this.IsPressed = false;

                        if (e.Button == UIMouseButtons.Left)
                        {
                            this.DoChanged();
                        }
                        else if (e.Button == UIMouseButtons.Right)
                        {
                            this.Reset(false);
                        }
                    }

                    break;

                case UIMouseEventType.Wheel:
                    if (this.IsHovered)
                    {
                        this.prevValue = this.Value;
                        this.Value += this.Step * (e.Delta / 120);
                        this.DoChanged();
                    }

                    break;
            }

            base.DoMouseInput(e);
        }

        protected override void DoReset()
        {
            var def = this.defaultValue ?? this.Value;

            if (!this.range.IsSameValue(def))
            {
                this.prevValue = this.Value;
                this.Value = def;
                this.DoChanged();
            }
        }

        protected virtual void DoChanged()
        {
            if (!this.range.IsSameValue(this.prevValue))
            {
                this.OnChanged(this, EventArgs.Empty);
            }
        }

        protected override void DoRender(Graphics graphics)
        {
            var text = string.Empty;
            var isDecimal = this.TextFlags.HasFlag(UIRangeTextFlags.Decimal);
            var sign = this.TextFlags.HasFlag(UIRangeTextFlags.PlusSign) && this.Value >= float.Epsilon ? "+" : string.Empty;

            switch (this.TextType)
            {
                case UIRangeTextType.None:
                    return;

                case UIRangeTextType.Caption:
                    if (this.Lookup != null)
                    {
                        var index = UIMath.Clamp((int)this.Value, 0, this.Lookup.Length - 1);
                        text = this.Lookup[index];
                    }
                    else
                    {
                        text = this.Caption;
                    }
                    
                    break;

                case UIRangeTextType.Value:
                    text = isDecimal ?
                           $"{sign}{(int)this.Value:D}" :
                           $"{sign}{this.Value:F}";
                    break;

                case UIRangeTextType.Percent:
                    text = isDecimal ?
                           $"{sign}{(int)(this.Percent * 100):D}%" :
                           $"{sign}{this.Percent,0:P}";
                    
                    break;

                case UIRangeTextType.Range:
                    text = isDecimal ?
                            $"{sign}{(int)this.Value:D}/{(int)this.Max:D}" :
                            $"{sign}{this.Value:F}/{this.Max:F}";
                    break;
            }

            if (!string.IsNullOrEmpty(this.TextFormat))
            {
                text = string.Format(this.TextFormat, text);
            }

            var textColor = this.Enabled ? this.Colors.Text : this.Colors.TextDisabled;
            graphics.DrawText(this.ScreenRect, this.Fonts.Default, textColor, this.TextAlign, false, false, text);
        }
    }
}
