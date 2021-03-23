namespace Halftone
{
    using System.Drawing;

    public class GridPatternSquareEnumerator : GridPatternEnumeratorBase
    {
        public GridPatternSquareEnumerator(int width, int height, int cellSize)
            : base(width, height, cellSize)
        {
        }

        protected override Point GetCurrent()
        {
            var x = (this.Position % this.CellsX) * this.CellSize;
            var y = (this.Position / this.CellsX) * this.CellSize;
            return new Point(x, y);
        }
    }
}
