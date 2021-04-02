namespace HalftoneFx.UI
{
    using KWUI;

    using System;

    public abstract class UILayoutContainer<T>
        where T : UIControl
    {
        private readonly T container;

        public UILayoutContainer(UILayoutBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder
                .Begin<T>()
                .Ref(ref container);

            this.BuildLayout(builder);
            builder.End();
        }

        public T Container => this.container;

        protected abstract void BuildLayout(UILayoutBuilder builder);
    }
}
