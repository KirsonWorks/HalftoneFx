namespace HalftoneFx
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openPictureDialog = new System.Windows.Forms.OpenFileDialog();
            this.savePictureDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // openPictureDialog
            // 
            this.openPictureDialog.Filter = resources.GetString("openPictureDialog.Filter");
            // 
            // savePictureDialog
            // 
            this.savePictureDialog.DefaultExt = "png";
            this.savePictureDialog.Filter = resources.GetString("savePictureDialog.Filter");
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 606);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HalftoneFx";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnFormDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnFormDragEnter);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openPictureDialog;
        private System.Windows.Forms.SaveFileDialog savePictureDialog;
    }
}

