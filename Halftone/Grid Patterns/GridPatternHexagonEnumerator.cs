using System;
using System.Drawing;

namespace Halftone
{
    public class GridPatternHexagonEnumerator : GridPatternEnumeratorBase
    {
        public GridPatternHexagonEnumerator(int width, int height, int cellSize)
            : base(width + cellSize / 2, height, cellSize)
        {
        }

        protected override Point GetCurrent()
        {
            var odd = this.Position / this.CellsX % 2 != 0;
            var offset = odd ? -this.CellSize / 2 : 0;
            var x = (this.Position % this.CellsX) * this.CellSize + offset;
            var y = (this.Position / this.CellsX) * this.CellSize;

            return new Point(x, y);
        }
    }
}
