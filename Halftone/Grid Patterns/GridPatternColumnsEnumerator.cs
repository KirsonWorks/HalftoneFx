using System.Drawing;

namespace Halftone
{
    public class GridPatternColumnsEnumerator : GridPatternEnumeratorBase
    {
        public GridPatternColumnsEnumerator(int width, int height, int cellSize) 
            : base(width / 2, height, cellSize)
        {
        }

        protected override Point GetCurrent()
        {
            var x = (this.Position % this.CellsX * 2) * this.CellSize + (this.CellSize / 2);
            var y = (this.Position / this.CellsX) * this.CellSize;

            return new Point(x, y);
        }
    }
}
