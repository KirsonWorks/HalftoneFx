namespace HalftoneFx.Views
{
    using KWUI;
    using KWUI.Controls;

    using HalftoneFx.Presenters;
    using HalftoneFx.UI;

    using System.Drawing;
    using System.Windows.Forms;

    public class WorkspaceView : IWorkspaceView
    {
        private readonly UIManager ui;
        
        private WorkspacePresenter presenter;

        private UIPictureBox pictureBox;

        private UIStatusBar statusBar;

        private PictureOptionsView pictureOptions;

        private HalftoneOptionsView halftoneOptions;

        public WorkspaceView(UIManager ui)
        {
            this.ui = ui;
            this.BuildLayout();
        }
        
        public WorkspacePresenter Presenter
        { 
            get => this.presenter;

            set
            {
                this.presenter = value;
                this.pictureOptions.Presenter = value;
                this.halftoneOptions.Presenter = value;
            }
        }
        public void SetUp()
        {
            this.pictureOptions.SetUp();
            this.halftoneOptions.SetUp();
        }

        public void SetPicture(Image picture)
        {
            this.pictureBox.Image = picture;
            this.pictureBox.OptimalView();
            this.pictureOptions.ValueForSize(picture.Size);
        }

        public void UpdatePicture(Image picture)
        {
            this.pictureBox.Image = picture;
            this.ui.Refresh();
        }

        public void SetProgress(float percent)
        {
            this.pictureOptions.ValueForProgress(percent);
            this.ui.Refresh();
        }

        public void SetPattern(Image image)
        {
            this.halftoneOptions.ValueForCustomPattern(image);
            this.ui.Refresh();
        }

        public void UpdatePalettes()
        {
            this.pictureOptions.LookupForPalette(this.Presenter.Palettes.GetNames());
        }

        public void Error(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void BuildLayout()
        {
            var layoutBuilder = new UILayoutBuilder(this.ui);

            this.pictureBox = this.ui.NewPictureBox();
            this.pictureBox.SetLayoutPreset(UILayoutPreset.Wide);

            this.pictureBox.OnZoomChanged += (s, e) =>
                this.pictureOptions.ValueForZoom(this.pictureBox.Scale);

            this.pictureBox.NewPopupMenu(UIPictureBoxPopupMenuItems.Get(this.pictureBox));

            layoutBuilder.Translate(25, 25);
            this.pictureOptions = new PictureOptionsView(layoutBuilder);

            layoutBuilder.Translate(150, 25);
            this.halftoneOptions = new HalftoneOptionsView(layoutBuilder);

            this.statusBar = this.ui.NewStatusBar();
            this.statusBar.ShowControlHints = true;
        }
    }
}
