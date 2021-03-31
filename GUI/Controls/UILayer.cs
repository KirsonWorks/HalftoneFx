namespace GUI.Controls
{
    public class UILayer : UIControl
    {
        public UILayer() : base()
        {
            this.HandleMouseEvents = false;
            this.Anchors = UIAnchors.All;
        }

        protected override void DoParentChanged()
        {
            if (this.Parent is UIControl parent)
            {
                this.Size = parent.Size;
            }
        }
    }
}
