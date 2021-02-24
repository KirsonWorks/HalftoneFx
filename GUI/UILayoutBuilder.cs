namespace GUI
{
    using GUI.Controls;
    using GUI.BaseControls;

    using System;
    using System.Drawing;

    public class UILayoutBuilder
    {
        private readonly UIManager manager;

        private readonly UIPositioner positioner;

        private UIControl control = null;

        public UILayoutBuilder(UIManager manager, UILayoutStyle style)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));

            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            this.positioner = new UIPositioner(style);
            this.control = this.Container = manager;
        }

        public UIControl Container { get; private set; } = null;

        public UIControl Control
        {
            get => this.control;

            set
            {
                if (value == null)
                {
                    throw new Exception("Current control cannot be null");
                }

                if (this.control != value)
                {
                    this.control = value;
                    this.positioner.Set(value);
                }
            }
        }

        public UILayoutBuilder Name(string name)
        {
            this.control.Name = name;
            return this;
        }

        public UILayoutBuilder Ref<T>(ref T control) 
            where T: UIControl
        {
            control = this.control as T;
            return this;
        }

        public UILayoutBuilder Hint(string text)
        {
            this.control.HintText = text;
            return this;
        }

        public UILayoutBuilder Indent(int count = 1)
        {
            this.positioner.Indent(count);
            return this;
        }

        public UILayoutBuilder SameLine()
        {
            this.positioner.SameLine();
            return this;
        }

        public UILayoutBuilder Wide(float value)
        {
            this.positioner.Wide(value);
            return this;
        }

        public UILayoutBuilder WideCenter(float value)
        {
            this.positioner.Wide(value);
            this.positioner.Align(UIAlign.Center);
            return this;
        }

        public UILayoutBuilder WideRight(float value)
        {
            this.positioner.Wide(value);
            this.positioner.Align(UIAlign.RightMiddle);
            return this;
        }

        public UILayoutBuilder Stretch(float value = 0.0f)
        {
            this.positioner.Stretch(value);
            return this;
        }

        public UILayoutBuilder Tall(float value = 0.0f)
        {
            this.positioner.Tall(value);
            return this;
        }

        public UILayoutBuilder TextColor(Color color)
        {
            this.Control.CustomColor("Text", color);
            return this;
        }

        public UILayoutBuilder Click(EventHandler handler)
        {
            this.Control.OnMouseClick += handler;
            return this;
        }

        public UILayoutBuilder Changing(EventHandler handler)
        {
            var ev = this.Control.GetType().GetEvent("OnChanging");
            ev?.AddEventHandler(this.Control, handler);
            return this;
        }

        public UILayoutBuilder Changed(EventHandler handler)
        {
            var ev = this.Control.GetType().GetEvent("OnChanged");
            ev?.AddEventHandler(this.Control, handler);
            return this;
        }

        public UILayoutBuilder BeginPanel(float x, float y)
        {
            if (this.Container != this.manager)
            {
                throw new Exception("Сannot create a nested panel");
            }

            this.positioner.Reset(x, y);
            this.Control = this.Container.NewPanel(string.Empty);
            this.Container = this.control;
            this.positioner.Reset();

            return this;
        }

        public UILayoutBuilder EndPanel()
        {
            this.Container.Size = this.positioner.MaxSize;
            this.control = this.Container = this.manager;
            this.positioner.Reset();
            return this;
        }

        public UILayoutBuilder Label(string caption)
        {
            this.Control = this.Container.NewLabel(string.Empty, caption);
            return this;
        }

        public UILayoutBuilder Button(string caption)
        {
            this.Control = this.Container.NewButton(string.Empty, caption);
            return this;
        }

        public UILayoutBuilder CheckBox(string caption, bool value = false)
        {
            var checkbox = this.Container.NewCheckBox(string.Empty, caption);
            checkbox.Checked = value;
            this.Control = checkbox;
            return this;
        }

        public UILayoutBuilder Slider(float value, float min, float max, float step)
        {
            var slider = this.Container.NewSlider(string.Empty);
            slider.Setup(min, max, step, value);
            this.Control = slider;
            return this;
        }

        public UILayoutBuilder SliderInt(int value, int min, int max, int step)
        {
            var slider = this.Container.NewSlider(string.Empty);
            slider.Setup(min, max, step, value);
            slider.TextType = UIRangeTextType.Value;
            slider.TextFlags = UIRangeTextFlags.Decimal | UIRangeTextFlags.PlusSign;
            this.Control = slider;
            return this;
        }

        public UILayoutBuilder Image(float width, float height, Image img = null, bool center = false)
        {
            var image = this.Container.NewImage(string.Empty, img);
            image.Center = center;

            this.positioner.Stretch(width);
            this.positioner.Tall(height);
            this.Control = image;
            return this;
        }

        public UILayoutBuilder Progress(float min, float max, float step)
        {
            var progress = this.Container.NewProgressBar(string.Empty);
            progress.Setup(min, max, step, 0.0f);
            this.Control = progress;
            return this;
        }
    }

    public class UILayoutStyle
    {
        public static UILayoutStyle Default = new UILayoutStyle
        {
            Indent = 15.0f,
            Margin = new PointF(10, 10),
            Spacing = new PointF (5, 5),
            CellWidth = 80.0f,
            RowHeight = 23.0f
        };

        public float Indent { get; set; }

        public PointF Margin { get; set; }

        public PointF Spacing { get; set; }

        public float CellWidth { get; set; }

        public float RowHeight { get; set; }
    }

    public class UIPositioner
    {
        private static PointF DefaultAlign = new PointF(0.0f, 0.5f);

        private readonly UILayoutStyle style;

        private PointF align;

        private bool isSameLine;

        private int indentCount;

        private bool stretching;

        private float nextX, nextY;

        private float lastWidth, lastHeight, customColWidth, customRowHeight;

        public UIPositioner(UILayoutStyle style)
        {
            this.style = style;
            this.Reset();
        }

        public float MaxWidth { get; private set; }

        public float MaxHeight { get; private set; }

        public SizeF MaxSize
        {
            get => new SizeF(this.MaxWidth, this.MaxHeight);
        }

        public void Align(PointF value)
        {
            this.align = value;
        }

        public void Indent(int count)
        {
            this.indentCount = Math.Max(0, count);
        }

        public void Wide(float value)
        {
            this.customColWidth = Math.Max(1.0f, value);
        }

        public void Tall(float value)
        {
            this.customRowHeight = Math.Max(1.0f, value);
        }

        public void Stretch(float value = 0.0f)
        {
            this.stretching = true;
            this.Wide(value);
        }

        public void SameLine()
        {
            if (!this.isSameLine)
            {
                this.isSameLine = true;
                this.nextX += this.lastWidth + this.style.Spacing.X;
                this.nextY -= this.lastHeight + this.style.Spacing.Y;
            }
        }

        public void Set(UIControl control)
        {
            if (control != null)
            {
                if (this.stretching)
                {
                    control.AutoSize = false;
                }

                if (!this.isSameLine)
                {
                    this.nextX = this.style.Margin.X;
                }

                float width;
                float height = this.customRowHeight >= 1.0f ? this.customRowHeight : this.style.RowHeight;

                if (this.customColWidth >= 1.0f)
                {
                    control.SetSize(this.customColWidth, height);
                    width = this.customColWidth;
                }
                else
                {
                    control.SetSize(this.style.CellWidth, height);
                    width = control.Width;
                    height = control.Height;
                }
                
                this.nextX += this.indentCount * this.style.Indent;
                var x = this.nextX + (width * this.align.X) - (control.Width * this.align.X);
                var y = this.nextY + (height * this.align.Y) - (control.Height * this.align.Y);
                this.nextY += height + this.style.Spacing.Y;
                this.MaxWidth = Math.Max(this.MaxWidth, this.nextX + width + this.style.Margin.X);
                this.MaxHeight = Math.Max(this.MaxHeight, this.nextY + this.style.Margin.Y - this.style.Spacing.Y);
                this.lastWidth = width;
                this.lastHeight = height;

                control.SetPosition(x, y);
                this.Default();
            }
        }

        public void Reset(float x = 0.0f, float y = 0.0f)
        {
            this.MaxWidth = 0.0f;
            this.MaxHeight = 0.0f;

            this.nextX = x > 0.0f ? x : this.style.Margin.X;
            this.nextY = y > 0.0f ? y : this.style.Margin.Y;
            
            this.Default();
            this.isSameLine = true;
        }

        private void Default()
        {
            this.indentCount = 0;
            this.isSameLine = false;
            this.stretching = false;
            this.align = DefaultAlign;
            this.customColWidth = 0.0f;
            this.customRowHeight = 0.0f;
        }
    }
}
