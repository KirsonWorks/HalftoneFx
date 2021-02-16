using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalftoneFx.GFX
{
    public class ImageFilterNegative : ImageFilterBase, IImageFilter
    {
        public ImageFilterNegative()
        {
            this.MaxValue = 1;
        }

        public bool HasEffect() => this.Value != 0;

        public void RGB(ref byte r, ref byte g, ref byte b)
        {
            r = (byte)(255 - r);
            g = (byte)(255 - g);
            b = (byte)(255 - b);
        }
    }
}
