namespace Halftone
{
    using System;
    using System.Drawing;

    public class GridPatternNoiseEnumerator : GridPatternEnumeratorBase
    {
        private readonly Random rand = new Random();

        public GridPatternNoiseEnumerator(int width, int height, int cellSize) 
            : base(width, height, cellSize)
        {
        }

        protected override Point GetCurrent()
        {
            var x = (this.Position % this.CellsX) * this.CellSize;
            var y = (this.Position / this.CellsX) * this.CellSize;
            var n = this.CellSize / 4;
            
            return new Point(x + rand.Next(-n, n), y + rand.Next(-n, n));
        }
    }
}
