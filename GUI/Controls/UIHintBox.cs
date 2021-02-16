namespace GUI.Controls
{
    using System.Diagnostics;

    public class UIHintBox : UIPanel
    {
        private readonly UILabel label;

        private readonly Stopwatch autoHideTime;

        private bool autoHide = true;

        private bool canShow = false;

        public bool AutoHide
        {
            get => this.autoHide;

            set
            {
                if (this.canShow && value && this.autoHide != value)
                {
                    this.autoHideTime.Restart();
                }

                this.autoHide = value;
            }
        }

        public int AutoHideInterval { get; set; } = 100;

        public override string Caption
        {
            get => this.label.Caption;

            set
            {
                this.label.Caption = value;
                this.canShow = !string.IsNullOrEmpty(value);
            }
        }

        public UIHintBox() : base()
        {
            this.Visible = false;
            this.AutoHide = true;
            this.AutoSize = true;
            this.ExtraSize = 5;

            this.label = this.NewLabel("label");
            this.label.Caption = "label";
            this.label.HandleEvents = false;

            this.autoHideTime = new Stopwatch();
        }

        protected override void DoChangeVisibility()
        {
            if (this.Visible)
            {
                if (this.canShow)
                {
                    this.autoHideTime.Restart();
                }
                else
                {
                    this.Visible = false;
                }
            }
        }

        protected override void DoProcess()
        {
            base.DoProcess();

            if (this.Visible && this.AutoHide && this.autoHideTime.ElapsedMilliseconds > this.AutoHideInterval)
            {
                this.autoHideTime.Stop();
                this.Visible = false;
            }
        }
    }
}