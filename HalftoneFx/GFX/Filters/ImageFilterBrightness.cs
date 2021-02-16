using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HalftoneFx.GFX
{
    public class ImageFilterBrightness : ImageFilterBase, IImageFilter
    {
        public static int Min = -150;

        public static int Max = 100;

        public ImageFilterBrightness()
        {
            this.MinValue = Min;
            this.MaxValue = Max;
        }

        public bool HasEffect() => this.Value != 0;

        public void RGB(ref byte r, ref byte g, ref byte b)
        {
            r = this.ClampByte(r + this.Value);
            g = this.ClampByte(g + this.Value);
            b = this.ClampByte(b + this.Value);
        }
    }
}
