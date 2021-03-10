namespace HalftoneFx.Presenters
{
    using HalftoneFx.Views;

    using System;
    using System.Drawing;

    public class WorkspacePresenter
    {
        private readonly IWorkspaceView view;

        private readonly HalftoneImage halftone;

        public WorkspacePresenter(IWorkspaceView view)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.view.Presenter = this;

            this.halftone = new HalftoneImage();
            this.SetUp();
        }

        public void LoadPicture(Image picture)
        {
            this.halftone.Image = picture;
            this.view.SetPicture(picture);
        }

        private void SetUp()
        {
            this.LoadPicture(Properties.Resources.Logo);
        }
    }
}
