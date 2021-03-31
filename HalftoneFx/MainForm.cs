namespace HalftoneFx
{
    using HalftoneFx.UI;
    using HalftoneFx.Presenters;
    using HalftoneFx.Views;

    using System.Windows.Forms;
    using GUI.Controls;

    public partial class MainForm : Form
    {
        private readonly UIWinForms ui;

        private readonly WorkspacePresenter workspace;

        public MainForm()
        {
            this.InitializeComponent();
            this.Text = $"{Application.ProductName} v{Application.ProductVersion}";

            this.ui = new UIWinForms()
            {
                Container = this,
            };

            this.workspace = new WorkspacePresenter(new WorkspaceView(this.ui));

            
            var wnd = this.ui.NewNode<UIWindow>(string.Empty);
            wnd.Caption = "Window Title";
            wnd.Anchors = GUI.UIAnchors.Right;
            wnd.SetSize(200, 200);
            wnd.SetPosition((this.ClientSize.Width - wnd.Width) / 2, (this.ClientSize.Height - wnd.Height) / 2);
            wnd.Show();
        }

        private void OnFormDragEnter(object sender, DragEventArgs e)
        {
            // And need to check the file extension.
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ?
                DragDropEffects.All : DragDropEffects.None;
        }

        private void OnFormDragDrop(object sender, DragEventArgs e)
        {
            var filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            this.workspace.LoadPictureFromFile(filenames[0]);
        }
        
    }
}
