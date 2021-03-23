namespace GUI
{
    using System.Drawing;

    public class UILayoutOptions
    {
        public static UILayoutOptions Default = new UILayoutOptions
        {
            Indent = 15.0f,
            Margin = new PointF(10, 10),
            Spacing = new SizeF(5, 5),
            CellWidth = 90.0f,
            CellHeight = 23.0f
        };

        public float Indent { get; set; }

        public PointF Margin { get; set; }

        public SizeF Spacing { get; set; }

        public float CellWidth { get; set; }

        public float CellHeight { get; set; }
    }
}
