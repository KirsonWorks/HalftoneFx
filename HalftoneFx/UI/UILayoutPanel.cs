namespace HalftoneFx.UI
{
    using GUI;
    using GUI.Controls;

    using System;

    public abstract class UILayoutPanel : UIPanel
    {
        public UILayoutPanel(UILayoutBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Begin(this);
            this.BuildLayout(builder);
            builder.End();
        }

        protected abstract void BuildLayout(UILayoutBuilder builder);
    }
}
