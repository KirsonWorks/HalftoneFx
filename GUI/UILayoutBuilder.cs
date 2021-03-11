namespace GUI
{
    using GUI.Controls;
    using GUI.BaseControls;

    using System;
    using System.Drawing;
    using System.Collections;
    using System.Collections.Generic;
    using GUI.Helpers;

    public class UILayoutBuilder
    {
        private readonly UIManager manager;

        private readonly UIPositioner positioner;

        private UIControl control = null;

        public UILayoutBuilder(UIManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.positioner = new UIPositioner(manager.LayoutOptions ?? UILayoutOptions.Default);
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
            control = this.Control as T;
            return this;
        }

        public UILayoutBuilder Caption(string text)
        {
            this.Control.Caption = text;
            return this;
        }

        public UILayoutBuilder Hint(string text)
        {
            this.Control.HintText = text;
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

        public UILayoutBuilder Add<T>()
            where T : UIControl
        {
            this.Control = UIFactory.NewNode<T>(this.Container, string.Empty);
            return this;
        }

        public UILayoutBuilder Begin<T>(PointF? pos = null)
            where T: UIControl
        {
            this.positioner.Push(pos);
            this.Control = this.Container.NewNode<T>(string.Empty);
            this.Container = this.control;
            this.positioner.Reset();
            return this;
        }

        public UILayoutBuilder End()
        {
            if (this.Container.Parent == null)
            {
                this.control = this.Container = this.manager;
                return this;
            }

            this.Container.Size = this.positioner.Pop();
            this.control = this.Container = this.Container.Parent as UIControl;
            return this;
        }

        /*
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
            this.Container.Size = this.positioner.OverallSize;
            this.control = this.Container = this.manager;
            this.positioner.Reset();
            return this;
        }
        */

        public UILayoutBuilder Label(string caption)
        {
            var label = this.Container.NewLabel(string.Empty, caption);
            label.TextAlign = UIAlign.LeftMiddle;
            this.Control = label;
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

        public UILayoutBuilder Slider(int value, int min, int max)
        {
            var slider = this.Container.NewSlider(string.Empty);
            slider.Setup(min, max, 1, value);
            slider.TextType = UIRangeTextType.Caption;
            this.Control = slider;
            return this;
        }

        public UILayoutBuilder SliderFloat(float value, float min, float max, float step,
            UIRangeTextFlags flags = UIRangeTextFlags.None)
        {
            var slider = this.Container.NewSlider(string.Empty);
            slider.Setup(min, max, step, value);
            slider.TextType = UIRangeTextType.Value;
            slider.TextFlags = flags;
            this.Control = slider;
            return this;
        }

        public UILayoutBuilder SliderInt(int value, int min, int max, int step,
            UIRangeTextFlags flags = UIRangeTextFlags.None)
        {
            var slider = this.Container.NewSlider(string.Empty);
            slider.Setup(min, max, step, value);
            slider.TextType = UIRangeTextType.Value;
            slider.TextFlags = UIRangeTextFlags.Decimal | flags;
            this.Control = slider;
            return this;
        }

        public UILayoutBuilder Image(float width, float height, Image img = null, bool center = false)
        {
            var image = this.Container.NewImage(string.Empty, img);
            image.Center = center;
            image.Stretch = true;
            image.BorderSize = 1;

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

        public UILayoutBuilder TextFormat(string value)
        {
            if (this.Control is UIRangeControl range)
            {
                range.TextFormat = value;
            }

            return this;
        }
    }

    public class UILayoutOptions
    {
        public static UILayoutOptions Default = new UILayoutOptions
        {
            Indent = 15.0f,
            Margin = new PointF(10, 10),
            Spacing = new SizeF(5, 5),
            CellWidth = 90.0f,
            CellHeight = 23.0f
        };

        public float Indent { get; set; }

        public PointF Margin { get; set; }

        public SizeF Spacing { get; set; }

        public float CellWidth { get; set; }

        public float CellHeight { get; set; }
    }

    public class UIPositioner
    {
        private static PointF DefaultAlign = new PointF(0.0f, 0.5f);

        private readonly UILayoutOptions options;

        private PointF align;

        private bool isSameLine;

        private int indentCount;

        private bool stretching;

        private PointF Pos = PointF.Empty;

        private SizeF lastCellSize = SizeF.Empty;

        private SizeF customCellSize = SizeF.Empty;

        private Stack<PointF> stack = new Stack<PointF>();

        public UIPositioner(UILayoutOptions options)
        {
            this.options = options;
            this.Reset();
        }

        public SizeF OverallSize { get; private set; }

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
            this.customCellSize = new SizeF(Math.Max(1.0f, value), this.customCellSize.Height);
            //this.customColWidth = Math.Max(1.0f, value);
        }

        public void Tall(float value)
        {
            this.customCellSize = new SizeF(this.customCellSize.Width, Math.Max(1.0f, value));
            //this.customRowHeight = Math.Max(1.0f, value);
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
                var offset = this.lastCellSize + this.options.Spacing;
                this.Pos += new SizeF(offset.Width, -offset.Height);
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

                float width;
                float height = this.customCellSize.Height >= 1.0f ? this.customCellSize.Height : this.options.CellHeight;

                if (this.customCellSize.Width >= 1.0f)
                {
                    control.SetSize(this.customCellSize.Width, height);
                    width = this.customCellSize.Width;
                }
                else
                {
                    control.SetSize(this.options.CellWidth, height);
                    width = control.Width;
                    height = control.Height;
                }


                if (this.isSameLine)
                {
                    height = Math.Max(height, this.lastCellSize.Height);
                }
                else
                {
                    this.Pos.X += this.options.Margin.X;
                }

                this.Pos.X += this.indentCount * this.options.Indent;
                var x = this.Pos.X + (width - control.Width) * this.align.X;
                var y = this.Pos.Y + (height - control.Height) * this.align.Y;
                this.Pos.Y += height + this.options.Spacing.Height;

                var size = new SizeF(
                    this.Pos.X + width + this.options.Margin.X,
                    this.Pos.Y + this.options.Margin.Y - this.options.Spacing.Height);

                this.OverallSize = this.OverallSize.Max(size);
                this.lastCellSize = new SizeF(width, height);

                control.SetPosition(x, y);
                this.NextRow();
            }
        }

        public void Reset(PointF? pos = null, SizeF? size = null)
        {
            this.Pos = pos.HasValue ? pos.Value : this.options.Margin;
            this.OverallSize = size.HasValue ? size.Value : SizeF.Empty;

            this.NextRow();
            this.isSameLine = true;
        }

        public void Push(PointF? pos)
        {
            this.Reset(pos);
            this.stack.Push(this.Pos);
        }

        public SizeF Pop()
        {
            if (this.stack.Count == 0)
            {
                return SizeF.Empty;
            }

            var size = this.OverallSize;
            var pos = this.stack.Pop();
            var overall = this.OverallSize.Max(
                new SizeF(
                    pos.X + size.Width + this.options.Margin.X,
                    pos.Y + size.Height + this.options.Margin.Y - this.options.Spacing.Height));

            this.Reset(new PointF(pos.X, pos.Y + size.Height + options.Spacing.Height), overall);
            return size;
        }

        private void NextRow()
        {
            this.indentCount = 0;
            this.isSameLine = false;
            this.stretching = false;
            this.align = DefaultAlign;
            this.customCellSize = SizeF.Empty;
        }
    }
}
