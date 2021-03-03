namespace Halftone
{
    using System;
    using System.Drawing;

    public class GridPatternCheckerboardEnumerator : GridPatternEnumeratorBase
    {
        public GridPatternCheckerboardEnumerator(int width, int height, int cellSize) 
            : base((int)Math.Ceiling((float)width / 2), height, cellSize)
        {
        }

        protected override Point GetCurrent()
        {
            var odd = (this.Position / this.CellsX % 2 != 0) ? 1 : 0;
            var x = ((this.Position % this.CellsX * 2) + odd) * this.CellSize;
            var y = this.Position / this.CellsX * this.CellSize;

            return new Point(x, y);
        }
    }
}
