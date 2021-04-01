namespace GUI
{
    using System.Collections.Generic;
    using System.Drawing;

    public partial class UIControl : UINode
    {
        private static UIStyle style = new UIStyle();

        private Dictionary<string, Color> customColors = new Dictionary<string, Color>();

        protected UIStyle Style => style;

        protected UIColors Colors => this.Style.Colors;

        protected UIFonts Fonts => this.Style.Fonts;

        public UIControl CustomColor(string name, Color value)
        {
            System.Diagnostics.Debug.Assert(!this.customColors.ContainsKey(name));
            this.customColors.Add(name, value);
            return this;
        }
    }
}
