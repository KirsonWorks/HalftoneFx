namespace GUI
{
    using GUI.BaseControls;
    using System.Collections.Generic;

    public class UIButtonGroup
    {
        private UIButtonControl checkedButton;

        private readonly List<UIButtonControl> buttons = new List<UIButtonControl>();

        public IEnumerable<UIButtonControl> Buttons => this.buttons;

        public UIButtonControl CheckedButton
        {
            get => this.checkedButton;

            set
            {
                if (this.checkedButton != value)
                {
                    if (this.checkedButton != null)
                    {
                        this.checkedButton.SetChecked(false);
                    }

                    this.checkedButton = value;
                    this.checkedButton.SetChecked(true);
                }
            }
        }

        public void Add(UIButtonControl button)
        {
            if (!this.buttons.Contains(button))
            {
                this.buttons.Add(button);
            }
        }

        public void Remove(UIButtonControl button)
        {
            this.buttons.Remove(button);
        }
    }
}
