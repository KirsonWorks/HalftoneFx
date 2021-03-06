namespace HalftoneFx
{
    using GUI;
    using GUI.Controls;
    using GUI.BaseControls;
    using HalftoneFx.Editor;

    using Common;
    using Halftone;

    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private readonly UIWinForms ui = new UIWinForms();

        private readonly UIPictureBox pictureBox = new UIPictureBox();

        private readonly UIStatusBar statusBar = new UIStatusBar();

        private readonly HalftoneImage image = new HalftoneImage { ThumbnailSize = 300 };

        private readonly UILabel labelSize;

        private readonly UILabel labelZoom;

        private readonly UIProgressBar progress;

        private readonly UIImage customPattern;

        public MainForm()
        {
            this.InitializeComponent();
            this.Text = $"{Application.ProductName} v{Application.ProductVersion}";

            this.ui.Container = this;
            this.ui.OnNotification += this.OnUINotification;

            this.pictureBox.Name = "picture-box";
            this.pictureBox.Parent = this.ui;
            this.pictureBox.Size = this.ClientSize;
            this.pictureBox.OnZoomChanged += OnPictureBoxZoomChanged;

            this.statusBar = this.ui.NewStatusBar("status-bar");

            var pm = new UIPopupMenu();
            pm.Parent = this.ui;

            pm.AddItem("ZOOM IN");
            pm.AddItem("ZOOM OUT");
            pm.AddItem("RESET VIEW");

            this.pictureBox.PopupControl = pm;

            var builder = new UILayoutBuilder(this.ui, UILayoutStyle.Default);

            // Like a bullshit.
            builder.BeginPanel(20, 45)
                   .Label("PICTURE").TextColor(Color.Gold)
                   .Button("LOAD").Hint("Load picture from a file").Click(this.OnOpenPictureDialog)
                   .SameLine()
                   .Button("SAVE").Hint("Save picture to a file").Click(this.OnSavePictureDialog)
                   .CheckBox("SMOOTHING").Hint("On/Off Smoothing filter").Changed(this.OnSmoothingChanged)
                   .CheckBox("GRAYSCALE").Hint("On/Off Grayscale filter").Changed(this.OnGrayscaleChanged)
                   .CheckBox("NEGATIVE").Hint("On/Off Negative filter").Changed(this.OnNegativeChanged)
                   .Label("BRIGHTNESS")
                   .Wide(90)
                   .SliderInt(0, -150, 100, 1, UIRangeTextFlags.PlusSign).Hint("Brightness filter").Changing(this.OnBrightnessChanging)
                   .Label("CONTRAST")
                   .Wide(90)
                   .SliderInt(0, -50, 100, 1, UIRangeTextFlags.PlusSign).Hint("Contrast filter").Changing(this.OnContrastChanging)
                   .Label("QUANTIZATION")
                   .Wide(90)
                   .SliderInt(1, 1, 255, 1).Hint("Quantization filter").Changing(this.OnQuantizationChanging)
                   .Label("DITHERING")
                   .Wide(90)
                   .Slider(0, 0, 3).Caption("None").Hint("Dithering").Changing(this.OnDitheringChanging)
                   .Label("SIZE: 0x0").Ref(ref labelSize)
                   .Label("ZOOM: 100%").Hint("Click for reset zoom or fit to screen").Ref(ref labelZoom)
                   .Click((s, e) => this.pictureBox.ResetZoom())
                   .Wide(90)
                   .Progress(0.0f, 1.0f, 0.1f).Ref(ref progress)
                   .EndPanel();

            builder.BeginPanel(140, 45)
                   .CheckBox("HALFTONE", this.image.HalftoneEnabled).TextColor(Color.Gold).Changed(this.OnHalftoneEnabledChanged)
                   .Label("GRID TYPE")
                   .Wide(90)
                   .Slider(0, 0, (int)HalftoneGridType.Max - 1).Caption("Square").Changing(this.OnGridTypeChanging)
                   .Label("SHAPE TYPE")
                   .Wide(90)
                   .Slider(0, 0, (int)HalftoneShapeType.Max - 1).Caption("Square").Changing(this.OnPatternTypeChanging)
                   .Label("CUSTOM")
                   .Button("LOAD").Click(this.OnOpenPatternDialog)
                   .SameLine()
                   .Button("CLEAR").Click(this.OnClearPattern)
                   .Image(90, 90, Properties.Resources.Imageholder, true).Ref(ref customPattern)
                   .Label("SIZE BY")
                   .Wide(90)
                   .Slider(0, 0, (int)HalftoneShapeSizing.Max - 1).Caption("None").Changing(this.OnShapeSizingChanging)
                   .Label("CELL SIZE")
                   .Wide(90)
                   .SliderInt(this.image.CellSize, 2, 64, 1).TextFormat("{0}px").Changing(this.OnCellSizeChanging)
                   .Label("CELL SCALE")
                   .Wide(90)
                   .SliderFloat(this.image.CellScale, 0.5f, 3.0f, 0.05f).Changing(this.OnCellScaleChanging)
                   .CheckBox("TRANSP. BG", this.image.TransparentBg).Changed(this.OnTransparentBgChanged)
                   .EndPanel();

            this.statusBar.BringToFront();

            this.image.OnImageAvailable += this.OnImageAvailable;
            this.image.OnThumbnailAvailable += this.OnThumbnailAvailable;

            this.image.OnProgress += (s, e) =>
            {
                this.progress.Value = e.Percent;
                this.Invalidate();
            };

            this.SetPicture(Properties.Resources.Logo);
            this.pictureBox.Zoom(0.5f);
        }

        private void SetPicture(Image picture)
        {
            this.image.Image = this.pictureBox.Image = picture;
            this.pictureBox.OptimalView();

            this.labelSize.Caption = $"SIZE: {picture.Width}x{picture.Height}";
        }

        private void LoadPictureFromFile(string path)
        {
            try
            {
                var image = Image.FromFile(path);
                this.SetPicture(image);
            }
            catch
            {
                MessageBox.Show("Can't load the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void SavePictureToFile(string path)
        {
            try
            {
                this.pictureBox.Image.SaveAs(path);
            }
            catch
            {
                MessageBox.Show("Can't save the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void OnOpenPictureDialog(object sender, EventArgs e)
        {
            if (this.openPictureDialog.ShowDialog() == DialogResult.OK)
            {
                this.LoadPictureFromFile(this.openPictureDialog.FileName);
            }
        }

        private void OnSavePictureDialog(object sender, EventArgs e)
        {
            if (this.savePictureDialog.ShowDialog() == DialogResult.OK)
            {
                this.SavePictureToFile(this.savePictureDialog.FileName);
            }
        }

        private void OnOpenPatternDialog(object sender, EventArgs e)
        {
            if (this.openPictureDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var pattern = Image.FromFile(this.openPictureDialog.FileName)
                                       .Resize(64, 64, InterpolationMode.Default);

                    this.customPattern.Image = pattern;
                    this.image.CustomPattern = new Bitmap(pattern);
                }
                catch
                {
                    MessageBox.Show("Can't load the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void OnClearPattern(object sender, EventArgs e)
        {
            this.customPattern.Image = Properties.Resources.Imageholder;
            this.image.CustomPattern = null;
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

        private void OnImageAvailable(object sender, GenerateDoneEventArgs e)
        {
            this.pictureBox.Image = e.Image;
            this.statusBar.Caption = "Done.";
            this.Invalidate();
            GC.Collect();
        }

        private void OnThumbnailAvailable(object sender, GenerateDoneEventArgs e)
        {
            this.pictureBox.Image = e.Image;
            this.Invalidate();
        }

        private void OnPictureBoxZoomChanged(object sender, EventArgs e)
        {
            this.labelZoom.Caption = $"ZOOM: {this.pictureBox.Scale:P0}";
        }

        private void OnBrightnessChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.Brightness = (int)slider.Value;
        }

        private void OnContrastChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.Contrast = (int)slider.Value;
        }

        private void OnQuantizationChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.Quantization = (int)slider.Value;
        }

        private void OnNegativeChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.Negative = checkbox.Checked;
        }

        private void OnGrayscaleChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.Grayscale = checkbox.Checked;
        }

        private void OnSmoothingChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.Smoothing = checkbox.Checked;
        }

        private void OnDitheringChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            var value = (int)slider.Value;
            var dimension = 1 << value;
            slider.Caption = dimension > 1 ? $"{dimension}x{dimension}" : "None";
            this.image.Dithering = value;
        }

        private void OnHalftoneEnabledChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.HalftoneEnabled = checkbox.Checked;
        }

        private void OnCellSizeChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.CellSize = (int)slider.Value;
        }

        private void OnCellScaleChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            this.image.CellScale = slider.Value;
        }

        private void OnGridTypeChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            var types = Enum.GetNames(typeof(HalftoneGridType));
            var value = (int)slider.Value;
            slider.Caption = types[value];
            this.image.GridType = value;
        }

        private void OnPatternTypeChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            var types = Enum.GetNames(typeof(HalftoneShapeType));
            var value = (int)slider.Value;
            slider.Caption = types[value];
            this.image.ShapeType = value;
        }

        private void OnShapeSizingChanging(object sender, EventArgs e)
        {
            var slider = sender as UISlider;
            var types = Enum.GetNames(typeof(HalftoneShapeSizing));
            var value = (int)slider.Value;
            slider.Caption = types[value];
            this.image.ShapeSizing = value;
        }

        private void OnTransparentBgChanged(object sender, EventArgs e)
        {
            var checkbox = sender as UICheckBox;
            this.image.TransparentBg = checkbox.Checked;
        }

        private void OnFormDragEnter(object sender, DragEventArgs e)
        {
            // And need to check the file extension.
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        private void OnFormDragDrop(object sender, DragEventArgs e)
        {
            var filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            this.LoadPictureFromFile(filenames[0]);
        }
    }
}
