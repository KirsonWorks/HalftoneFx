using System;
using System.Collections.Generic;

namespace Halftone
{
    public enum HalftoneShapeType
    {
        Square = 0,
        Circle,
        Dithering4x4,
        Max
    }

    public static class ShapePatternFactory
    {
        public static IShapePattern GetPattern(HalftoneShapeType type)
        {
            var patterns = new Dictionary<HalftoneShapeType, Type>
            {
                { HalftoneShapeType.Square, typeof(ShapePatternSquare) },
                { HalftoneShapeType.Circle, typeof(ShapePatternCircle) },
                { HalftoneShapeType.Dithering4x4, typeof(ShapePatternDithering4x4) },
            };

            var pattern = patterns.ContainsKey(type) ? patterns[type] : patterns[HalftoneShapeType.Square];
            return (IShapePattern)Activator.CreateInstance(pattern);
        }
    }
}
