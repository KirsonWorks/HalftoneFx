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
                .Begin<UIPanel>()
                //.Button("100$")
                //.SameLine()
                //.Button("200$")
                .Begin<UIPanel>()
                .Button("TEST")
                .End()
                .SameLine()
                .Begin <UIPanel>()
                .PrintStack()
                .End()
                .Begin<UIPanel>()
                .End()
                /*.Begin<UIPanel>()
                .Button("HIT ME")
                .End()
                .Begin<UIPanel>()
                .End()
                */
                .End();

                /*
                .Begin<UIPanel>(new PointF(300, 300))
                .Label("LABEL")
                .Button("CLICK")
                .End();
                */

            /*
            layoutBuilder
                .Begin<UIPanel>(new PointF(100, 100))
                .Label("HOLY")
                .SameLine()
                .Indent(20)
                .Button("BUTTON")
                .End();
            */
            //layoutBuilder.BeginPanel

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
