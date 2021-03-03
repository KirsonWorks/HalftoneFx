namespace Halftone
{
    using System;
    using System.Collections.Generic;

    public enum GridPatternType
    {
        Square = 0,
        Hexagon,
        Checkerboard,
        Noise,
    }

    public static class GridPatternFactory
    {
        public static GridPatternEnumeratorBase GetPattern(GridPatternType type, int width, int height, int cellSize)
        {
            var patterns = new Dictionary<GridPatternType, Type>
            {
                { GridPatternType.Square, typeof(GridPatternSquareEnumerator) },
                { GridPatternType.Hexagon, typeof(GridPatternHexagonEnumerator) },
                { GridPatternType.Noise, typeof(GridPatternNoiseEnumerator) },
                { GridPatternType.Checkerboard, typeof(GridPatternCheckerboardEnumerator) },
            };

            return (GridPatternEnumeratorBase)Activator.CreateInstance(patterns[type], width, height, cellSize);
        }
    }
}
