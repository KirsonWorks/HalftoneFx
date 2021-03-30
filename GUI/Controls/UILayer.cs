namespace GUI.Controls
{
    using System.Drawing;

    public class UILayer : UIControl
    {
        public UILayer() : base()
        {
            this.HandleMouseEvents = false;
        }

        protected override void DoParentResize(SizeF deltaSize)
        {
            this.UpdateSize();
        }

        protected override void DoParentChanged()
        {
            this.UpdateSize();
        }

        private void UpdateSize()
        {
            if (this.Parent is UIManager parent)
            {
                this.Size = parent.Size;
            }
        }
    }
}
