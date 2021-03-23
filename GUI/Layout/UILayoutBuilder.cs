namespace GUI
{
    using GUI.Controls;
    using GUI.BaseControls;

    using System;
    using System.Drawing;
    using System.Collections.Generic;

    public class UILayoutBuilder
    {
        private readonly UIManager manager;

        private readonly Stack<UILayoutWorkbench> stack = new Stack<UILayoutWorkbench>();

        private UILayoutWorkbench workbench;

        private UIControl control = null;

        public UILayoutBuilder(UIManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.workbench = new UILayoutWorkbench(manager.LayoutOptions);
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
                    this.control.Parent = this.Container;
                    this.workbench.Set(value);
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
            this.workbench.Indent(count);
            return this;
        }

        public UILayoutBuilder SameLine()
        {
            this.workbench.SameLine();
            return this;
        }

        public UILayoutBuilder Wide(float value)
        {
            this.workbench.Wide(value);
            return this;
        }

        public UILayoutBuilder WideCenter(float value)
        {
            this.workbench.Wide(value);
            this.workbench.Align(UIAlign.Center);
            return this;
        }

        public UILayoutBuilder WideRight(float value)
        {
            this.workbench.Wide(value);
            this.workbench.Align(UIAlign.RightMiddle);
            return this;
        }

        public UILayoutBuilder Stretch(float value = 0.0f)
        {
            this.workbench.Stretch(value);
            return this;
        }

        public UILayoutBuilder Tall(float value = 0.0f)
        {
            this.workbench.Tall(value);
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

        public UILayoutBuilder Translate(PointF location)
        {
            this.workbench.Translate(location);
            return this;
        }

        public UILayoutBuilder Translate(float x, float y)
        {
            return this.Translate(new PointF(x, y));
        }

        public UILayoutBuilder Begin(UIControl container, PointF? location = null)
        {
            this.workbench.Align(PointF.Empty);

            if (location.HasValue)
            {
                this.workbench.Translate(location.Value);
            }

            this.Control = container;
            this.Container = this.control;

            this.stack.Push(this.workbench);
            this.workbench = new UILayoutWorkbench(this.manager.LayoutOptions);
            return this;
        }

        public UILayoutBuilder Begin<T>(PointF? location = null)
            where T: UIControl
        {
            var container = this.Container.NewNode<T>(string.Empty);
            this.Begin(container, location);
            return this;
        }

        public UILayoutBuilder End()
        {
            if (this.Container.Parent == null)
            {
                this.control = this.Container = this.manager;
                return this;
            }

            var size = this.workbench.OverallSize;

            if (!size.IsEmpty)
            {
                this.Container.Size = size;
            }

            var previous = this.workbench;
            this.workbench = this.stack.Pop();
            this.workbench.NextLine(previous);

            this.control = this.Container = this.Container.Parent as UIControl;
            return this;
        }

        public UILayoutBuilder Label(string caption, bool autoSize = true)
        {
            var label = this.Container.NewLabel(string.Empty, caption);
            label.AutoSize = autoSize;
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

        public UILayoutBuilder Slider(int value, int min = 0, int max = 100)
        {
            var slider = this.Container.NewSlider(string.Empty);
            slider.Setup(min, max, 1, value);
            slider.TextType = UIRangeTextType.Caption;
            this.Control = slider;
            return this;
        }

        public UILayoutBuilder Slider(int value, string[] lookup)
        {
            if (lookup == null)
            {
                throw new ArgumentNullException(nameof(lookup));
            }

            var slider = this.Container.NewSlider(string.Empty);
            slider.Setup(0, lookup.Length - 1, 1, value);
            slider.TextType = UIRangeTextType.Caption;
            slider.Lookup = lookup;
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

            this.workbench.Stretch(width);
            this.workbench.Tall(height);
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
}
