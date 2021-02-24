using System;

namespace Halftone
{
    public enum ShapePatternType
    {
        Square = 0,
        Circle,
        Dithering4x4,
    }

    public static class ShapePatternFactory
    {
        public static IShapePattern GetPattern(ShapePatternType type)
        {
            switch (type)
            {
                case ShapePatternType.Square:
                    return new ShapePatternSquare();

                case ShapePatternType.Circle:
                    return new ShapePatternCircle();

                case ShapePatternType.Dithering4x4:
                    return new ShapePatternDithering4x4();

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
