namespace HalftoneFx.Views
{
    using GUI;
    using GUI.Controls;

    using HalftoneFx.Presenters;

    using System.Drawing;

    public class WorkspaceView : IWorkspaceView
    {
        private readonly UIManager ui;

        private UIPictureBox pictureBox;

        private UIStatusBar statusBar;

        public WorkspaceView(UIManager ui)
        {
            this.ui = ui;
            this.SetUp();
        }
        
        public WorkspacePresenter Presenter { get; set; }

        public void SetPicture(Image picture)
        {
            this.pictureBox.Image = picture;
            this.pictureBox.OptimalView();
        }

        private void SetUp()
        {
            this.pictureBox = this.ui.NewPictureBox();
            this.statusBar = this.ui.NewStatusBar();
        }

        private void OnUINotification(object sender, UINotificationEventArgs e)
        {
            switch (e.What)
            {
                case UINotification.MouseOver:
                    if (sender is UIControl control)
                    {
                        this.statusBar.Caption = control.HintText;
                    }

                    break;

                case UINotification.MouseOut:
                    this.statusBar.Caption = string.Empty;
                    break;
            }
        }
    }
}
