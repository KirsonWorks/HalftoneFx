using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Helpers
{
    using System.Drawing;

    public static class RectFHelper
    {
        public static RectangleF Inflate(this RectangleF rect, float value)
        {
            var newRect = rect;
            newRect.Inflate(value, value);
            return newRect;
        }
    }
}
