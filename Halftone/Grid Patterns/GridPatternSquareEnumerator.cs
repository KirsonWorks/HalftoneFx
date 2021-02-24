namespace Halftone
{
    using System;
    using System.Drawing;

    public class GridPatternSquareEnumerator : GridPatternEnumeratorBase
    {
        private readonly int xCells;

        private readonly int yCells;

        private readonly int count;

        public GridPatternSquareEnumerator(int width, int height, int cellSize)
            : base(width, height, cellSize)
        {
            this.xCells = Math.Max(1, (int)Math.Ceiling((float)width / cellSize));
            this.yCells = Math.Max(1, (int)Math.Ceiling((float)height / cellSize));
            this.count = this.xCells * yCells;
        }

        public override bool MoveNext()
        {
            if (this.Position < this.count - 1)
            {
                this.Position++;
                return true;
            }

            return false;
        }

        protected override Point GetCurrent()
        {
            var x = (this.Position % xCells) * this.CellSize;
            var y = (this.Position / xCells) * this.CellSize;
            return new Point(x, y);
        }
    }
}
