namespace GUI.Controls
{
    using System;
    using System.Drawing;

    public static partial class UIFactory
    {
        private static int nameCounter = 0;

        public static string Name(Type type)
        {
            nameCounter++;
            var name = type.Name.Replace("UI", "");
            return $"{name}{nameCounter}";
        }

        public static T NewNode<T>(this UIControl parent, string name) where T : UINode
        {
            var node = Activator.CreateInstance<T>();
            node.Name = string.IsNullOrWhiteSpace(name) ? Name(typeof(T)) : name;
            node.Parent = parent;
            return node;
        }

        public static object NewNode(this UIControl parent, Type type, string name)
        {
            var node = (UINode)Activator.CreateInstance(type);
            node.Name = string.IsNullOrWhiteSpace(name) ? Name(type) : name;
            node.Parent = parent;
            return node;
        }

        public static UIWindow NewWindow(this UIManager parent, string name, string caption = "")
        {
            var window = NewNode<UIWindow>(parent, name);
            window.Caption = string.IsNullOrEmpty(caption) ? name : caption;
            return window;
        }

        public static UILayer NewLayer(this UIManager parent, string name)
        {
            return NewNode<UILayer>(parent, name);
        }

        public static UIPanel NewPanel(this UIControl parent, string name)
        {
            var control = NewNode<UIPanel>(parent, name);
            return control;
        }

        public static UILabel NewLabel(this UIControl parent, string name, string caption = "")
        {
            var control = NewNode<UILabel>(parent, name);
            control.Caption = string.IsNullOrEmpty(caption) ? name : caption;
            return control;
        }

        public static UIImage NewImage(this UIControl parent, string name, Image image = null)
        {
            var control = NewNode<UIImage>(parent, name);
            control.Image = image;
            return control;
        }

        public static UIImage NewImage(this UIControl parent, string name, string filename = "")
        {
            var image = string.IsNullOrWhiteSpace(filename) ? null : Image.FromFile(filename);
            return parent.NewImage(name, image);
        }

        public static UIHorizontalLine NewHorizontalLine(this UIControl parent, string name)
        {
            return NewNode<UIHorizontalLine>(parent, name);
        }

        public static UIButton NewButton(this UIControl parent, string name, string caption = "")
        {
            var control = NewNode<UIButton>(parent, name);
            control.Caption = string.IsNullOrEmpty(caption) ? name : caption;
            return control;
        }

        public static UICheckBox NewCheckBox(this UIControl parent, string name, string caption = "")
        {
            var control = NewNode<UICheckBox>(parent, name);
            control.Caption = string.IsNullOrEmpty(caption) ? name : caption;
            return control;
        }

        public static UIRadioButton NewRadioButton(this UIControl parent, string name, UIButtonGroup group, string caption = "")
        {
            var control = NewNode<UIRadioButton>(parent, name);
            control.Group = group;
            control.Caption = string.IsNullOrEmpty(caption) ? name : caption;
            return control;
        }

        public static UIProgressBar NewProgressBar(this UIControl parent, string name)
        {
            return NewNode<UIProgressBar>(parent, name);
        }

        public static UISlider NewSlider(this UIControl parent, string name)
        {
            return NewNode<UISlider>(parent, name);
        }
    }
}
