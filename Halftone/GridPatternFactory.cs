namespace Halftone
{
    using System;
    using System.Collections.Generic;

    public enum HalftoneGridType
    {
        Square = 0,
        Hexagon,
        Checkerboard,
        Lines,
        Noise,
        Max
    }

    public static class GridPatternFactory
    {
        public static GridPatternEnumeratorBase GetPattern(HalftoneGridType type, int width, int height, int cellSize)
        {
            var patterns = new Dictionary<HalftoneGridType, Type>
            {
                { HalftoneGridType.Square, typeof(GridPatternSquareEnumerator) },
                { HalftoneGridType.Hexagon, typeof(GridPatternHexagonEnumerator) },
                { HalftoneGridType.Noise, typeof(GridPatternNoiseEnumerator) },
                { HalftoneGridType.Checkerboard, typeof(GridPatternCheckerboardEnumerator) },
                { HalftoneGridType.Lines, typeof(GridPatternLinesEnumerator) },
            };

            var pattern = patterns.ContainsKey(type) ? patterns[type] : patterns[HalftoneGridType.Square];
            return (GridPatternEnumeratorBase)Activator.CreateInstance(pattern, width, height, cellSize);
        }
    }
}
