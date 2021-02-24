using System;
using System.Drawing;

namespace Halftone
{
    public class GridPatternHexagonEnumerator : GridPatternEnumeratorBase
    {
        private readonly int xCells;

        private readonly int yCells;

        public GridPatternHexagonEnumerator(int width, int height, int cellSize)
            : base(width, height, cellSize)
        {
            this.xCells = Math.Max(1, (int)Math.Ceiling((float)(width + cellSize / 2) / cellSize));
            this.yCells = Math.Max(1, (int)Math.Ceiling((float)height / cellSize));
            this.CellCount = this.xCells * this.yCells;
        }

        protected override Point GetCurrent()
        {
            var odd = this.Position / xCells % 2 != 0;
            var offset = odd ? -this.CellSize / 2 : 0;
            var x = (this.Position % this.xCells) * this.CellSize + offset;
            var y = (this.Position / this.xCells) * this.CellSize;
            return new Point(x, y);
        }


    }
}
