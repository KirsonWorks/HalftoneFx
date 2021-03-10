namespace HalftoneFx.Presenters
{
    using HalftoneFx.Views;

    using System;

    public class HalftoneOptionsPresenter
    {
        private readonly HalftoneImage halftone;

        private readonly IHalftoneOptionsView view;

        public HalftoneOptionsPresenter(IHalftoneOptionsView view, HalftoneImage halftone)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.halftone = halftone ?? throw new ArgumentNullException(nameof(halftone));
            this.view.Presenter = this;
        }
    }
}
