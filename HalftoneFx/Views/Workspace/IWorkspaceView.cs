namespace HalftoneFx.Views
{
    using HalftoneFx.Presenters;

    using System.Drawing;

    public interface IWorkspaceView : IView<WorkspacePresenter>
    {
        void SetPicture(Image image);
    }
}
