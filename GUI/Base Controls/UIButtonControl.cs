namespace KWUI.BaseControls
{
    using System;
    using System.Drawing;

    public abstract class UIButtonControl : UIControl
    {
        private UIButtonGroup group;

        private bool isChecked = false;

        public event EventHandler OnChanged = delegate { };

        protected bool IsHovered { get; set; }

        protected bool IsPressed { get; set; }

        public virtual bool ToggleMode { get; set; }

        public UIMouseButtons ActionMouseButton { get; set; } = UIMouseButtons.Left;

        public bool Checked
        {
            get => this.isChecked;

            set
            {
                if (this.ToggleMode)
                {
                    if (this.Group != null)
                    {
                        if (value)
                        {
                            this.Group.CheckedButton = this;
                        }
                    }
                    else
                    {
                        this.SetChecked(value);
                    }
                }
            }
        }
        public UIButtonGroup Group
        {
            get => this.group;

            set
            {
                this.group?.Remove(this);
                this.group = value;
                this.group?.Add(this);
            }
        }

        internal void SetChecked(bool value)
        {
            //if (this.isChecked != value)
            //{
                this.isChecked = value;
                this.OnChanged?.Invoke(this, EventArgs.Empty);
            //}
        }

        protected Color GetFgColor()
        {
            return this.Enabled ? this.Colors.Text : this.Colors.TextDisabled;
        }

        protected Color GetStateColor(Color normal, Color pressed, Color hovered,
            Color @checked, Color disabled)
        {
            if (this.Enabled)
            {
                if (this.Checked)
                {
                    return @checked;
                }
                else if (this.IsPressed)
                {
                    return pressed;
                }
                else if (this.IsHovered)
                {
                    return hovered;
                }
            }
            else
            {
                return disabled;
            }

            return normal;
        }

        protected override void DoMouseClick(UIMouseEventArgs e)
        {
            if (e.Button == this.ActionMouseButton)
            {
                base.DoMouseClick(e);
            }
        }

        protected override void DoMouseInput(UIMouseEventArgs e)
        {
            if (e.Button == this.ActionMouseButton && e.EventType != UIMouseEventType.Move)
            {
                this.IsPressed = e.EventType == UIMouseEventType.Down;

                if (!this.IsPressed && this.IsHovered && this.ToggleMode)
                {
                    this.Checked = !this.Checked;
                }
            }

            base.DoMouseInput(e);
        }

        protected override void DoMouseOverOut(UIMouseEventArgs e, bool isOver)
        {
            if (this.Enabled)
            {
                this.IsHovered = isOver;
            }

            base.DoMouseOverOut(e, isOver);
        }

        protected override void DoFree()
        {
            this.Group = null;
        }
    }
}
