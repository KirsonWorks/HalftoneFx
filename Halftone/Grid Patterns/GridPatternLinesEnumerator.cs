namespace Halftone
{
    using System.Drawing;

    public class GridPatternLinesEnumerator : GridPatternEnumeratorBase
    {
        public GridPatternLinesEnumerator(int width, int height, int cellSize) 
            : base(width, height / 2, cellSize)
        {
        }

        protected override Point GetCurrent()
        {
            var x = (this.Position % this.CellsX) * this.CellSize;
            var y = (this.Position / this.CellsX * 2) * this.CellSize;

            return new Point(x, y);
        }
    }
}
