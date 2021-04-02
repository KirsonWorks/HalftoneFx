using System.Drawing;
using System.Collections.Generic;

namespace KWUI
{
    using ColorPair = KeyValuePair<string, Color>;

    public class UIColors
    {
        private readonly Stack<ColorPair> storedColors = new Stack<ColorPair>();

        public Color Background { get; set; }

        public Color Text { get; set; }

        public Color TextDisabled { get; set; }

        public Color Frame { get; set; }

        public Color FrameHovered { get; set; }

        public Color FrameActive { get; set; }

        public Color FrameDisabled { get; set; }

        public Color Border { get; set; }

        public Color BorderVolume { get; set; }

        public Color Container { get; set; }

        public Color Window { get; set; }

        public Color WindowBar { get; set; }

        public Color WindowCaption { get; set; }

        public Color WindowShadow { get; set; }

        public Color Button { get; set; }

        public Color ButtonHovered { get; set; }

        public Color ButtonActive { get; set; }

        public Color ButtonDisabled { get; set; }

        public Color ButtonChecked { get; set; }

        public Color CheckMark { get; set; }

        public Color ProgressBar { get; set; }

        public Color SliderGrab { get; set; }

        public Color SliderGrabActive { get; set; }

        public Color Line { get; set; }

        public Color StatusBar { get; set; }

        public void SetColor(string name, Color value)
        {
            var property = this.GetType().GetProperty(name);

            if (property != null)
            {
                property.SetValue(this, value);
            }
        }

        public Color GetColor(string name)
        {
            var property = this.GetType().GetProperty(name);

            if (property != null)
            {
                return (Color)property.GetValue(this);
            }

            return Color.Empty;
        }

        public void PushColor(string name, Color value)
        {
            var storedColor = this.GetColor(name);
            this.storedColors.Push(new ColorPair(name, storedColor));
            this.SetColor(name, value);
        }

        public void PushColors(IEnumerable<ColorPair> values)
        {
            foreach (var pair in values)
            {
                this.PushColor(pair.Key, pair.Value);
            }
        }

        public void PopColors()
        {
            while (this.storedColors.Count > 0)
            {
                var pair = this.storedColors.Pop();
                this.SetColor(pair.Key, pair.Value);
            }
        }
    }

    public class UIFonts
    {
        public Font Small { get; set; } = new Font("Microsoft Sans Serif", 6);

        public Font Default { get; set; } = new Font("Microsoft Sans Serif", 8);

        public Font Header { get; set; } = new Font("Microsoft Sans Serif", 10);

        public Font Description { get; set; } = new Font("Microsoft Sans Serif", 9);
    }

    public class UIShapes
    {
        public PointF[] CheckMark = new PointF[]
        {
            new PointF(0.05f, 0.4f),
            new PointF(0.25f, 0.4f),
            new PointF(0.45f, 0.675f),
            new PointF(0.75f, 0.05f),
            new PointF(0.95f, 0.05f),
            new PointF(0.475f, 0.95f),
        };

        public PointF[] Cross = new PointF[]
        {
            new PointF(0.1f, 0.1f),
            new PointF(0.3f, 0.1f),
            new PointF(0.5f, 0.4f),
            new PointF(0.7f, 0.1f),
            new PointF(0.9f, 0.1f),
            new PointF(0.6f, 0.5f),
            new PointF(0.9f, 0.9f),
            new PointF(0.7f, 0.9f),
            new PointF(0.5f, 0.6f),
            new PointF(0.3f, 0.9f),
            new PointF(0.1f, 0.9f),
            new PointF(0.4f, 0.5f),
        };

        public PointF[] Upperscore = new PointF[]
        {
            new PointF(0.1f, 0.1f),
            new PointF(0.9f, 0.1f),
            new PointF(0.9f, 0.3f),
            new PointF(0.1f, 0.3f),
        };

        public PointF[] Square = new PointF[]
        {
            new PointF(0.1f, 0.1f),
            new PointF(0.9f, 0.1f),
            new PointF(0.9f, 0.9f),
            new PointF(0.1f, 0.9f),
        };
    }

    public class UIStyle
    {
        public int Padding { get; set; } = 5;

        public int Rounding { get; set; } = 3;

        public int Spacing { get; set; } = 3;

        public int WindowRounding { get; set; } = 5;

        public float WindowBarSize { get; set; } = 24;

        public float WindowShadowSize { get; set; } = 3.5f;

        public float ToggleSize { get; set; } = 20;

        public float SliderGrabSize { get; set; } = 15;

        public float InnerShrink { get; set; } = 3.5f;

        public UIFonts Fonts { get; } = new UIFonts();

        public UIColors Colors { get; } = new UIColors();

        public UIShapes Shapes { get; } = new UIShapes();

        public UIStyle()
        {
            this.StyleColorsDark();
        }

        public void StyleColorsLight()
        {

        }

        public void StyleColorsDark()
        {
            this.Colors.Background = Color.FromArgb(13, 16, 19);

            this.Colors.Text = Color.FromArgb(255, 242, 244, 249);
            this.Colors.TextDisabled = Color.FromArgb(255, 91, 107, 119);

            this.Colors.Frame = Color.FromArgb(255, 51, 64, 74);
            this.Colors.FrameHovered = Color.FromArgb(255, 31, 51, 71);
            this.Colors.FrameActive = Color.FromArgb(255, 23, 31, 36);
            this.Colors.FrameDisabled = Color.Gray;

            this.Colors.Border = Color.FromArgb(255, 20, 26, 31);
            this.Colors.BorderVolume = Color.FromArgb(10, 255, 255, 255);

            this.Colors.Container = Color.FromArgb(255, 38, 46, 56); 
           
            this.Colors.Button = Color.FromArgb(255, 51, 63, 73);
            this.Colors.ButtonHovered = Color.FromArgb(255, 71, 143, 255);
            this.Colors.ButtonActive = Color.FromArgb(255, 15, 135, 249);
            this.Colors.ButtonDisabled = Color.Gray;
            this.Colors.ButtonChecked = Color.Goldenrod;

            this.Colors.CheckMark = Color.ForestGreen;

            this.Colors.Window = Color.FromArgb(255, 28, 38, 43);
            this.Colors.WindowBar = Color.FromArgb(255, 23, 31, 36);
            this.Colors.WindowCaption = Color.FromArgb(255, 242, 244, 249);
            this.Colors.WindowShadow = Color.FromArgb(100, 0, 0, 0);

            this.Colors.SliderGrab = Color.FromArgb(255, 71, 143, 255);
            this.Colors.SliderGrabActive = Color.Crimson;

            this.Colors.ProgressBar = Color.Crimson;
            this.Colors.StatusBar = Color.Black;
            this.Colors.Line = Color.Orange;
        }
    }
}
