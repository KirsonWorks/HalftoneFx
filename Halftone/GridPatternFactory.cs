namespace Halftone
{
    using System;

    public enum GridPatternType
    {
        Grid = 0,
    }

    public static class GridPatternFactory
    {
        public static GridPatternEnumeratorBase GetPattern(GridPatternType type, int width, int height, int cellSize)
        {
            switch (type)
            {
                case GridPatternType.Grid:
                    return new GridPatternSquareEnumerator(width, height, cellSize);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
