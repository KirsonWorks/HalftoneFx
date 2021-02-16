namespace GUI.Controls
{
    using System.Drawing;

    public class UILayer : UIControl
    {
        public UILayer() : base()
        {
            this.HandleEvents = false;
        }

        protected override void DoParentResize(SizeF deltaSize)
        {
            if (this.Parent is UIManager parent)
            {
                this.Size = parent.Size;
            }
        }
    }
}
