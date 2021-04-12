namespace HalftoneFx.UI
{
    using KWUI;
    using KWUI.Controls;
    using System;
    using System.Drawing;

    public class UIPopupColorPicker : UIColorPicker<UIWindow>
    {
        public UIPopupColorPicker(UILayoutBuilder builder, Color color)
            : base(builder)
        {
            this.Color = color;
        }

        public event EventHandler OnClose = delegate { };

        public void Popup(PointF location)
        {
            location.Y -= this.Container.Height;
            this.Container.Popup(location);
        }

        protected override void BuildLayout(UILayoutBuilder builder)
        {
            this.Container.Features = UIWindowFeatures.Disposable;
            this.Container.OnClose += (s, e) => this.OnClose(s, e);
            base.BuildLayout(builder);
        }
    }
}
