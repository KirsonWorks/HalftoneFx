namespace Halftone
{
    using System;

    public enum GridPatternType
    {
        Square = 0,
        Hexagon,
    }

    public static class GridPatternFactory
    {
        public static GridPatternEnumeratorBase GetPattern(GridPatternType type, int width, int height, int cellSize)
        {
            switch (type)
            {
                case GridPatternType.Square:
                    return new GridPatternSquareEnumerator(width, height, cellSize);

                case GridPatternType.Hexagon:
                    return new GridPatternHexagonEnumerator(width, height, cellSize);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
