namespace KWUI.Controls
{
    using System;
    using System.Collections.Generic;
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

        public static T NewNode<T>(this UIControl parent) where T : UINode
        {
            var node = Activator.CreateInstance<T>();
            node.Name = Name(typeof(T));
            node.Parent = parent;
            return node;
        }

        public static object NewNode(this UIControl parent, Type type)
        {
            var node = (UINode)Activator.CreateInstance(type);
            node.Name = Name(type);
            node.Parent = parent;
            return node;
        }

        public static UIWindow NewWindow(this UIManager parent, string caption = null)
        {
            var window = NewNode<UIWindow>(parent);
            window.Caption = string.IsNullOrEmpty(caption) ? string.Empty : caption;
            return window;
        }

        public static UILayer NewLayer(this UIManager parent)
        {
            return NewNode<UILayer>(parent);
        }

        public static UIPanel NewPanel(this UIControl parent)
        {
            return NewNode<UIPanel>(parent);
        }

        public static UILabel NewLabel(this UIControl parent, string caption = null)
        {
            var control = NewNode<UILabel>(parent);
            control.Caption = string.IsNullOrEmpty(caption) ? string.Empty : caption;
            return control;
        }

        public static UIImage NewImage(this UIControl parent, Image image = null)
        {
            var control = NewNode<UIImage>(parent);
            control.Image = image;
            return control;
        }

        public static UIImage NewImage(this UIControl parent, string filename = null)
        {
            var image = string.IsNullOrWhiteSpace(filename) ? null : Image.FromFile(filename);
            return parent.NewImage(image);
        }

        public static UIButton NewButton(this UIControl parent, string caption = null)
        {
            var control = NewNode<UIButton>(parent);
            control.Caption = string.IsNullOrEmpty(caption) ? string.Empty : caption;
            return control;
        }

        public static UICheckBox NewCheckBox(this UIControl parent, string caption = null)
        {
            var control = NewNode<UICheckBox>(parent);
            control.Caption = string.IsNullOrEmpty(caption) ? string.Empty : caption;
            return control;
        }

        public static UIRadioButton NewRadioButton(this UIControl parent, UIButtonGroup group,
            string caption = null)
        {
            var control = NewNode<UIRadioButton>(parent);
            control.Group = group;
            control.Caption = string.IsNullOrEmpty(caption) ? string.Empty : caption;
            return control;
        }

        public static UIToolButton NewToolButton(this UIControl parent, float size = 24, PointF[] shape = null)
        {
            var control = NewNode<UIToolButton>(parent);
            control.SetSize(size, size);
            control.Shape = shape;
            return control;
        }

        public static UIToolButton NewToolButton(this UIControl parent, float size = 24, Image icon = null)
        {
            var control = NewNode<UIToolButton>(parent);
            control.SetSize(size, size);
            control.Icon = icon;
            return control;
        }

        public static UIProgressBar NewProgressBar(this UIControl parent)
        {
            return NewNode<UIProgressBar>(parent);
        }

        public static UISlider NewSlider(this UIControl parent)
        {
            return NewNode<UISlider>(parent);
        }

        public static UIStatusBar NewStatusBar(this UIControl parent)
        {
            return NewNode<UIStatusBar>(parent);
        }

        public static UIPictureBox NewPictureBox(this UIControl parent)
        {
            return NewNode<UIPictureBox>(parent);
        }

        public static UIColorBox NewColorBox(this UIControl parent)
        {
            return NewNode<UIColorBox>(parent);
        }

        public static UIPopupMenu NewPopupMenu(this UIControl parent,
            IEnumerable<UIPopupMenuItem> items = null)
        {
            if (parent.Root is UIControl root)
            {
                var menu = NewNode<UIPopupMenu>(root);
                menu.Items.AddRange(items);
                parent.PopupControl = menu;
                return menu;
            }

            return null;
        }
    }
}
