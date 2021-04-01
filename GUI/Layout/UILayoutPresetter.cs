namespace GUI
{
    using System.Drawing;
    using System.Collections.Generic;

    public static class UILayoutPresetter
    {
        public static UIControl SetLayoutPreset(this UIControl control, UILayoutPreset preset, SizeF margin)
        {
            SetAnchors(control, preset);
            SetOffset(control, preset, margin);
            SetSize(control, preset, margin);
            return control;
        }

        public static UIControl SetLayoutPreset(this UIControl control, UILayoutPreset preset)
        {
            return control.SetLayoutPreset(preset, SizeF.Empty);
        }

        private static void SetAnchors(UIControl control, UILayoutPreset preset)
        {
            control.Anchors = UIAnchors.None;

            if (new List<UILayoutPreset>
            {
                UILayoutPreset.TopLeft,
                UILayoutPreset.BottomLeft,
                UILayoutPreset.CenterLeft,
                UILayoutPreset.TopWide,
                UILayoutPreset.LeftWide,
                UILayoutPreset.BottomWide,
                UILayoutPreset.HCenterWide,
                UILayoutPreset.Wide,
            }
            .Contains(preset))
            {
                control.Anchors |= UIAnchors.Left;
            }

            if (new List<UILayoutPreset>
            {
                UILayoutPreset.TopLeft,
                UILayoutPreset.TopRight,
                UILayoutPreset.CenterTop,
                UILayoutPreset.TopWide,
                UILayoutPreset.LeftWide,
                UILayoutPreset.RightWide,
                UILayoutPreset.VCenterWide,
                UILayoutPreset.Wide,
            }
            .Contains(preset))
            {
                control.Anchors |= UIAnchors.Top;
            }

            if (new List<UILayoutPreset>
            {
                UILayoutPreset.TopRight,
                UILayoutPreset.BottomRight,
                UILayoutPreset.CenterRight,
                UILayoutPreset.TopWide,
                UILayoutPreset.RightWide,
                UILayoutPreset.BottomWide,
                UILayoutPreset.HCenterWide,
                UILayoutPreset.Wide,
            }
            .Contains(preset))
            {
                control.Anchors |= UIAnchors.Right;
            }

            if (new List<UILayoutPreset>
            {
                UILayoutPreset.BottomLeft,
                UILayoutPreset.BottomRight,
                UILayoutPreset.CenterBottom,
                UILayoutPreset.LeftWide,
                UILayoutPreset.RightWide,
                UILayoutPreset.BottomWide,
                UILayoutPreset.VCenterWide,
                UILayoutPreset.Wide,
            }
            .Contains(preset))
            {
                control.Anchors |= UIAnchors.Bottom;
            }
        }

        private static void SetOffset(UIControl control, UILayoutPreset preset, SizeF margin)
        {
            if (control.Parent is UIControl parent)
            {
                var size = control.Size;
                var parentSize = parent.ClientRect.Size;
                var offset = margin.ToPointF();

                if (new List<UILayoutPreset>
                {
                    UILayoutPreset.CenterTop,
                    UILayoutPreset.Center,
                    UILayoutPreset.CenterBottom,
                    UILayoutPreset.VCenterWide,
                }
                .Contains(preset))
                {
                    offset.X = (parentSize.Width - size.Width) / 2;
                }

                if (new List<UILayoutPreset>
                {
                    UILayoutPreset.CenterLeft,
                    UILayoutPreset.Center,
                    UILayoutPreset.CenterRight,
                    UILayoutPreset.HCenterWide,
                }
                .Contains(preset))
                {
                    offset.Y = (parentSize.Height - size.Height) / 2;
                }

                if (new List<UILayoutPreset>
                {
                    UILayoutPreset.TopRight,
                    UILayoutPreset.CenterRight,
                    UILayoutPreset.BottomRight,
                    UILayoutPreset.RightWide,
                }
                .Contains(preset))
                {
                    offset.X = parentSize.Width - size.Width - margin.Width;
                }

                if (new List<UILayoutPreset>
                {
                    UILayoutPreset.BottomLeft,
                    UILayoutPreset.CenterBottom,
                    UILayoutPreset.BottomRight,
                    UILayoutPreset.BottomWide,
                }
                .Contains(preset))
                {
                    offset.Y = parentSize.Height - size.Height - margin.Height;
                }

                control.SetPosition(offset);
            }
        }

        private static void SetSize(UIControl control, UILayoutPreset preset, SizeF margin)
        {
            if (control.Parent is UIControl parent)
            {
                var size = control.Size;
                var parentSize = parent.ClientRect.Size;

                if (new List<UILayoutPreset>
                {
                    UILayoutPreset.TopWide,
                    UILayoutPreset.HCenterWide,
                    UILayoutPreset.BottomWide,
                    UILayoutPreset.Wide,
                }
                .Contains(preset))
                {
                    size.Width = parentSize.Width - margin.Width * 2;
                }

                if (new List<UILayoutPreset>
                {
                    UILayoutPreset.LeftWide,
                    UILayoutPreset.VCenterWide,
                    UILayoutPreset.RightWide,
                    UILayoutPreset.Wide,
                }
                .Contains(preset))
                {
                    size.Height = parentSize.Height - margin.Height * 2;
                }

                control.SetSize(size);
            }
        }
    }
}
