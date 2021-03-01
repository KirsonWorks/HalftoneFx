namespace Halftone
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class GridPatternEnumeratorBase : IEnumerator<Point>
    {
        public GridPatternEnumeratorBase(int width, int height, int cellSize)
        {
            this.Width = width;
            this.Height = height;
            this.CellSize = cellSize;
            this.CellsX = Math.Max(1, (int)Math.Ceiling((float)width / cellSize));
            this.CellsY = Math.Max(1, (int)Math.Ceiling((float)height / cellSize));
            this.CellCount = this.CellsX * this.CellsY;
        }
        public int CellCount { get; protected set; }

        public int Position { get; private set; } = -1;

        protected int Width { get; set; }

        protected int Height { get; set; }

        protected int CellsX { get; set; }

        protected int CellsY { get; set; }

        protected int CellSize { get; set; }

        object IEnumerator.Current => throw new NotImplementedException();

        public Point Current => this.GetCurrent();

        public virtual void Dispose()
        {
        }

        public virtual bool MoveNext()
        {
            if (this.Position < this.CellCount - 1)
            {
                this.Position++;
                return true;
            }

            return false;
        }

        public virtual void Reset()
        {
            this.Position = -1;
        }

        protected virtual Point GetCurrent()
        {
            throw new NotImplementedException();
        }
    }
}
