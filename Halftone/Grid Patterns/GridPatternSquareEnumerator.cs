namespace Halftone
{
    using System;
    using System.Drawing;

    public class GridPatternSquareEnumerator : GridPatternEnumeratorBase
    {
        private readonly int xCells;

        private readonly int yCells;

        public GridPatternSquareEnumerator(int width, int height, int cellSize)
            : base(width, height, cellSize)
        {
            this.xCells = Math.Max(1, (int)Math.Ceiling((float)width / cellSize));
            this.yCells = Math.Max(1, (int)Math.Ceiling((float)height / cellSize));
            this.CellCount = this.xCells * this.yCells;
        }

        protected override Point GetCurrent()
        {
            var x = (this.Position % xCells) * this.CellSize;
            var y = (this.Position / xCells) * this.CellSize;
            return new Point(x, y);
        }
    }
}
