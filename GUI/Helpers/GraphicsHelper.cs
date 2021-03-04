namespace GUI.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public static class GraphicsHelper
    {
        private static readonly PointF[] CheckMarkPath = new PointF[] 
        {
            new PointF(0.15f, 0.5f),
            new PointF(0.3f, 0.5f),
            new PointF(0.45f, 0.65f),
            new PointF(0.7f, 0.2f),
            new PointF(0.85f, 0.2f),
            new PointF(0.45f, 0.8f),
        };

        public static readonly Graphics GraphicsDummy = Graphics.FromImage(new Bitmap(1, 1));

        public static GraphicsPath GetRectPath(this Graphics graphics, RectangleF rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            int diameter = cornerRadius * 2;
            var arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));

            if (cornerRadius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // TL
            arc.X = rect.Left;
            path.AddArc(arc, 180, 90);

            // TR
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // BR 
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // BL
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static GraphicsPath GetClipPath(this Graphics graphics, RectangleF rect, int cornerRadius)
        {
            return graphics.GetRectPath(rect.Inflate(-0.5F), cornerRadius);
        }

        public static void DrawRect(this Graphics graphics, RectangleF rect, Color color, int cornerRadius, float shrink = 0)
        {
            if (shrink != 0)
            {
                var percent = 1.0f - shrink * 2.0f / Math.Min(rect.Width, rect.Height);
                cornerRadius = (int)Math.Round(cornerRadius * percent);
                rect = rect.Inflate(-shrink);
            }

            using (var brush = new SolidBrush(color))
            using (var path = graphics.GetRectPath(rect, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawBorder(this Graphics graphics, RectangleF rect, Color color, int cornerRadius, float shrink = 0)
        {
            if (shrink != 0)
            {
                rect = rect.Inflate(-shrink);
            }

            using (var pen = new Pen(color))
            using (var path = graphics.GetRectPath(rect, cornerRadius))
            {
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                graphics.DrawPath(pen, path);
                graphics.PixelOffsetMode = PixelOffsetMode.Default;
            }
        }

        public static void DrawBorderVolume(this Graphics graphics, RectangleF rect, Color color, int cornerRadius, float shrink = 0, float offset = 1F)
        {
            if (rect.IsEmpty || color.IsEmpty || color.A == 0)
            {
                return;
            }

            var pathL = new GraphicsPath();
            var pathR = new GraphicsPath();

            if (shrink != 0)
            {
                rect = rect.Inflate(-shrink);
            }

            if (cornerRadius <= 0)
            {
                pathL.AddLine(rect.X + offset, rect.Bottom - offset, rect.X + offset, rect.Y + offset);
                pathL.AddLine(rect.X + offset, rect.Y + offset, rect.Right - offset, rect.Y + offset);

                pathR.AddLine(rect.Right + offset, rect.Y + offset, rect.Right + offset, rect.Bottom + offset);
                pathR.AddLine(rect.Right + offset, rect.Bottom + offset, rect.X + offset, rect.Bottom + offset);
            }
            else
            {
                int diameter = cornerRadius * 2;
                var size = new SizeF(diameter, diameter);
                var arc = new RectangleF(rect.Location, size)
                {
                    X = rect.Left + offset,
                    Y = rect.Bottom - diameter - offset
                };

                pathL.AddArc(arc, 145, 35);

                arc.Y = rect.Y + offset;
                pathL.AddArc(arc, 180, 90);

                arc.X = rect.Right - diameter - offset;
                pathL.AddArc(arc, 270, 35);

                arc.X = rect.Right - diameter + offset;
                pathR.AddArc(arc, 325, 35);

                arc.Y = rect.Bottom - diameter + offset;
                pathR.AddArc(arc, 0, 90);

                arc.X = rect.Left + offset;
                pathR.AddArc(arc, 90, 35);
            }

            using (var pen = new Pen(color))
            {
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                graphics.DrawPath(pen, pathL);
                graphics.DrawPath(pen, pathR);
                graphics.PixelOffsetMode = PixelOffsetMode.Default;
            }

            pathL.Dispose();
            pathR.Dispose();
        }

        public static void DrawFrame(this Graphics graphics, RectangleF rect, Color bgColor, Color borderColor, int cornerRadius)
        {
            var hasBorder = !borderColor.IsEmpty && borderColor.A > 0;

            if (!bgColor.IsEmpty && bgColor.A > 0)
            {
                graphics.DrawRect(rect, bgColor, cornerRadius, 0.5F);
            }

            if (hasBorder)
            {
                graphics.DrawBorder(rect, borderColor, cornerRadius);
            }
        }

        public static void DrawCheckMark(this Graphics graphics, RectangleF rect, Color color)
        {
            if (!color.IsEmpty && color.A > 0)
            {
                using (var path = new GraphicsPath())
                using (var brush = new SolidBrush(color))
                {
                    path.AddLines(CheckMarkPath);

                    var matrix = new Matrix();
                    matrix.Translate(rect.X, rect.Y);
                    matrix.Scale(rect.Width, rect.Height);
                    path.Transform(matrix);
                    graphics.FillPath(brush, path);
                }
            }
        }

        public static RectangleF DrawText(this Graphics graphics, RectangleF rect, Font font, Color color, PointF align, bool autoSize, bool wordWrap, string text)
        {
            // CHANGE ME!

            if (rect.IsEmpty || color.IsEmpty || string.IsNullOrWhiteSpace(text))
            {
                return rect;
            }

            var size = SizeF.Empty;

            using (var format = new StringFormat(StringFormat.GenericTypographic))
            {
                if (!autoSize)
                {
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

                    if (!wordWrap)
                    {
                        format.FormatFlags = StringFormatFlags.NoWrap;
                    }

                    size = graphics.MeasureString(text, font, rect.Size, format);

                    if (!align.IsEmpty)
                    {
                        var oldPos = rect.Location;
                        rect.X += (rect.Width * align.X) - (size.Width * align.X);
                        rect.Y += (rect.Height * align.Y) - (size.Height * align.Y);
                        rect.Width += oldPos.X - rect.X;
                        rect.Height += oldPos.Y - rect.Y;
                    }
                }
                else
                {
                    size = graphics.MeasureString(text, font);
                    rect.Y += (rect.Height * 0.5F) - (size.Height * 0.5F);
                    rect.Size = size;
                }

                using (var brush = new SolidBrush(color))
                {
                    graphics.DrawString(text, font, brush, rect, format);
                }
            }

            return rect;
        }

        public static SizeF DrawText(this Graphics graphics, RectangleF rect, Font font, Color color, string text)
        {
            if (string.IsNullOrEmpty(text) || rect.IsEmpty)
            {
                return SizeF.Empty;
            }

            var size = graphics.MeasureString(text, font, 0, StringFormat.GenericTypographic);

            using (var brush = new SolidBrush(color))
            {
                graphics.DrawString(text, font, brush, rect.Location);
            }

            return size;
        }

        public static SizeF StringSize(string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return SizeF.Empty;
            }

            return GraphicsDummy.MeasureString(text, font, 0, StringFormat.GenericTypographic);
        }
    }
}
