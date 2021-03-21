namespace HalftoneFx.Views
{
    using GUI;
    using GUI.Controls;

    using HalftoneFx.Presenters;
    using HalftoneFx.UI;

    using System.Drawing;

    public class WorkspaceView : IWorkspaceView
    {
        private readonly UIManager ui;

        private UIPictureBox pictureBox;

        private UIStatusBar statusBar;

        private UIPictureOptionsPanel pictureOptions;

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
            var layoutBuilder = new UILayoutBuilder(this.ui);

            this.pictureBox = this.ui.NewPictureBox(string.Empty);
            this.pictureBox.NewPopupMenu(string.Empty,
                UIPictureBoxPopupMenuItems.Get(this.pictureBox));

            layoutBuilder
                .Translate(new PointF(100, 50))
                .Begin<UIPanel>()
                    .Label("TEST")
                    .Label("ASDA")
                .End()
                .SameLine()
                .Begin<UIPanel>()
                    .Label("TEST")
                    .Label("ASDA")
                    .Label("ASDAD")
                    .Button("CLICK")
                .End()
                .Begin<UIPanel>()
                .End();


            this.statusBar = this.ui.NewStatusBar(string.Empty);
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
