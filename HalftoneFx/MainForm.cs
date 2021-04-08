namespace HalftoneFx
{
    using HalftoneFx.UI;
    using HalftoneFx.Presenters;
    using HalftoneFx.Views;

    using System.Windows.Forms;
    using System.Drawing;
    using System;

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

            var c1 = Color.Red;
            var c2 = Color.FromArgb(0xff55ff);
            var c3 = Color.Blue;

            Console.WriteLine(c1.GetHue());
            Console.WriteLine(c2.GetHue());
            Console.WriteLine(c3.GetHue());
            
            Console.WriteLine();
            Console.WriteLine(c1.GetSaturation());
            Console.WriteLine(c2.GetSaturation());
            Console.WriteLine(c3.GetSaturation());

            Console.WriteLine();
            Console.WriteLine(c1.GetBrightness());
            Console.WriteLine(c2.GetBrightness());
            Console.WriteLine(c3.GetBrightness());
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
