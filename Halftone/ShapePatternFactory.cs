using System;

namespace Halftone
{
    public enum ShapePatternType
    {
        Square = 0,
        Circle,
        Test,
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

                case ShapePatternType.Test:
                    return new ShapePatternTest();

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
